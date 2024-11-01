using GHEngine.Audio.Source;
using GHEngine.Collections;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Diagnostics;


namespace GHEngine.Audio;


public class GHAudioEngine : IAudioEngine
{
    // Fields.
    public WaveFormat WaveFormat { get; private init; }
    public int AudioLatency { get; private init; }
    public int MaxSounds
    {
        get
        {
            lock (this)
            {
                return _maxSounds;
            }
        }
        set
        {
            lock (this)
            {
                _maxSounds = Math.Max(0, value);
            }
        }
    }

    public ISoundInstance[] Sounds
    {
        get
        {
            lock (this)
            {
                return _sounds.ToArray();
            }
        }
    }

    public float Volume
    {
        get
        {
            lock (this)
            {
                return _volume;
            }
        }
        set
        {
            lock (this)
            {
                _volume = Math.Clamp(value, 0f, 1f);
            }
        }
    }

    public int SoundCount
    {
        get
        {
            lock (this)
            {
                return _sounds.Count;
            }
        }
    }

    public TimeSpan ExecutionTime
    {
        get 
        {
            lock (this)
            {
                return _executionTime;
            }
        }
    }

    public int SamplesPerSecond => WaveFormat.SampleRate * WaveFormat.Channels;


    // Private static fields.
    private readonly WasapiOut _outputDevice;

    private float _volume = 1f;
    private int _maxSounds = 128;
    private readonly DiscreteTimeList<ISoundInstance> _sounds = new();
    private float[] _soundBuffer;

    private TimeSpan _executionTime;
    private readonly Stopwatch _executionMeasurer = new();



    // Constructors.
    public GHAudioEngine(int targetAudioLatency)
    {
        AudioLatency = targetAudioLatency;
        try
        {
            _outputDevice = new(AudioClientShareMode.Shared, true, targetAudioLatency);
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


    // Inherited methods.
    public void AddSoundInstance(ISoundInstance sound)
    {
        lock (_sounds)
        {
            _sounds.Add(sound);
        }
    }

    public void RemoveSoundInstance(ISoundInstance sound)
    {
        lock (_sounds)
        {
            _sounds.Remove(sound);
        }
    }

    public void ClearSounds()
    {
        lock (_sounds)
        {
            _sounds.Clear();
        }
    }

    public void Dispose()
    {
        lock (this)
        {
            _outputDevice?.Dispose();
        }
    }

    public int Read(float[] buffer, int offset, int count)
    {
        try
        {

            EnsureBuffer(count);

            float TargetVolume = Volume;
            lock (_sounds)
            {
                _sounds.ApplyChanges(); 
            }

            _executionMeasurer.Reset();
            _executionMeasurer.Start();
            ReadSounds(TargetVolume, buffer, offset, count);
            _executionMeasurer.Stop();

            lock (this)
            {
                _executionTime = _executionMeasurer.Elapsed;
            }
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
}