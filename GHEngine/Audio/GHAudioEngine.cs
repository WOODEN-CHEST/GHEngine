using GHEngine.Audio.Source;
using GHEngine.Collections;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Diagnostics;


namespace GHEngine.Audio;


public class GHAudioEngine : IAudioEngine
{
    // Static fields.
    public const float MIN_VOLUME = 0f;
    public const float MAX_VOLUME = 10_000f;
    public const float DEFAULT_VOLUME = 1f;


    // Fields.
    public WaveFormat WaveFormat { get; private init; }
    public int AudioLatency { get; private init; }
    public int MaxSounds
    {
        get => _maxSounds;
        set
        {
            _maxSounds = Math.Max(0, value);
        }
    }

    public ISoundInstance[] Sounds => _sounds.ToArray();

    public float Volume
    {
        get => _volume;
        set => _volume = float.IsNaN(value) ? DEFAULT_VOLUME : Math.Clamp(value, MIN_VOLUME, MAX_VOLUME);
    }

    public int SoundCount => _sounds.Count;
    public TimeSpan ExecutionTime => _executionTime;

    public int SamplesPerSecond => WaveFormat.SampleRate * WaveFormat.Channels;


    // Private static fields.
    private readonly WasapiOut _outputDevice;

    private float _volume = 1f;
    private int _maxSounds = 128;
    private readonly DiscreteTimeCollection<Action> _scheduledActions = new();
    private readonly DiscreteTimeCollection<ISoundInstance> _sounds = new();
    private float[] _soundBuffer;

    private TimeSpan _executionTime;
    private readonly Stopwatch _executionMeasurer = new();



    // Constructors.
    public GHAudioEngine(int targetAudioLatencyMilis)
    {
        AudioLatency = targetAudioLatencyMilis;
        try
        {
            _outputDevice = new(AudioClientShareMode.Shared, true, targetAudioLatencyMilis);
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(_outputDevice.OutputWaveFormat.SampleRate,
                _outputDevice.OutputWaveFormat.Channels);
            _soundBuffer = Array.Empty<float>();
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to initialize audio engine! {e}");
        }
    }


    // Private methods.
    private void EnsureBuffer(int requestedSize)
    {
        if ((_soundBuffer == null) || (requestedSize > _soundBuffer.Length))
        {
            _soundBuffer = new float[requestedSize];
        }
    }

    private int FillBufferWithSilence(float[] buffer, int offset, int count)
    {
        for (int i = offset; i < (offset + count); i++)
        {
            buffer[i] = 0f;
        }
        return count;
    }

    private void ReadSounds(float volume, float[] buffer, int offset, int count)
    {
        FillBufferWithSilence(buffer, offset, count);

        for (int SoundIndex = 0; SoundIndex < _sounds.Count; SoundIndex++)
        {
            _sounds[SoundIndex].GetSamples(_soundBuffer, count, WaveFormat);
            for (int Target = offset, Source = 0; Target < (offset + count); Target++, Source++)
            {
                buffer[Target] += _soundBuffer[Source] * volume;
            }
        }
    }

    private void ClampBufferValues(float[] buffer, int offset, int count)
    {
        for (int i = offset; i < count + offset; i++)
        {
            buffer[i] = Math.Clamp(buffer[i], -1f, 1f);
        }
    }

    private void ExecuteScheduledActions()
    {
        foreach (Action ScheduledAction in _scheduledActions)
        {
            ScheduledAction.Invoke();
        }
        _scheduledActions.Clear();
    }

    private void OnSoundInstanceFinishEvent(object? sender, SoundFinishedArgs args)
    {
        RemoveSoundInstance(args.Instance);
    }


    // Inherited methods.
    public void AddSoundInstance(ISoundInstance sound)
    {
        if (_sounds.Count < _maxSounds)
        {
            _sounds.Add(sound);
            sound.SoundFinished += OnSoundInstanceFinishEvent;
        }
    }

    public void RemoveSoundInstance(ISoundInstance sound)
    {
        _sounds.Remove(sound);
        sound.SoundFinished -= OnSoundInstanceFinishEvent;
    }

    public void ClearSounds()
    {
         _sounds.Clear();
    }

    public void Dispose()
    {
        _outputDevice?.Dispose();
    }

    public int Read(float[] buffer, int offset, int count)
    {
        try
        {
            _executionMeasurer.Reset();
            _executionMeasurer.Start();

            EnsureBuffer(count);
            _sounds.ApplyChanges();

            lock (this)
            {
                _scheduledActions.ApplyChanges();
            }

            ExecuteScheduledActions();
            ReadSounds(_volume, buffer, offset, count);
            ClampBufferValues(buffer, offset, count);

            _executionMeasurer.Stop();
            _executionTime = _executionMeasurer.Elapsed;
            return count;
        }
        catch (Exception e)
        {
            _outputDevice.Dispose();
            throw new Exception($"Exception in audio engine! {e}");
        }
    }

    public void Start()
    {
        _outputDevice.Init(this);
        _outputDevice.Play();
    }

    public void Stop()
    {
        _outputDevice.Stop();
    }

    public void ScheduleAction(params Action[] actions)
    {
        ArgumentNullException.ThrowIfNull(actions, nameof(actions));
        if (actions.Length == 0)
        {
            return;
        }

        lock (this)
        {
            foreach (Action ScheduledAction in actions)
            {
                _scheduledActions.Add(ScheduledAction);
            }
        }
    }
}