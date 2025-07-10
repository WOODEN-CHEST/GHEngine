using GHEngine.Audio.Source;
using GHEngine.Collections;
using OpenTK.Audio.OpenAL;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace GHEngine.Audio;


public class GHAudioEngine : IAudioEngine
{
    // Static fields.
    public const float MIN_VOLUME = 0f;
    public const float MAX_VOLUME = 10_000f;
    public const float DEFAULT_VOLUME = 1f;
    public const int DEFAULT_LATENCY_MILLISECONDS = 5;
    public const int DEFAULT_BUFFER_COUNT = 4;
    public const bool DEFAULT_SHOULD_SLEEP = true;


    // Fields.
    public WaveFormat WaveFormat { get; } = new(48000, 2, sizeof(float) / 8);
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

    public int LatencyMilliseconds { get; private init; }
    public int BufferCount { get; private init; }
    public bool ShouldSleep { get; private init; }


    // Private fields.
    private readonly object _lockObject = new();
    private float _volume = 1f;
    private int _maxSounds = 128;
    private readonly DiscreteTimeCollection<Action> _scheduledActions = new();
    private readonly DiscreteTimeCollection<ISoundInstance> _sounds = new();
    private float[] _soundBuffer;

    private TimeSpan _executionTime;
    private readonly Stopwatch _executionMeasurer = new();

    private bool _isPlaying = false;




    // Constructors.
    public unsafe GHAudioEngine(int targetAudioLatencyMilis = DEFAULT_LATENCY_MILLISECONDS,
        int bufferCount = DEFAULT_BUFFER_COUNT,
        bool shouldSleep = DEFAULT_SHOULD_SLEEP)
    {
        if (targetAudioLatencyMilis <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(targetAudioLatencyMilis),
            "Audio latency must be a positive value.");
        }
        LatencyMilliseconds = targetAudioLatencyMilis;

        if (bufferCount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bufferCount),
            "Buffer count must be a positive value.");
        }
        BufferCount = bufferCount;

        ShouldSleep = shouldSleep;
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
            ISoundInstance TargetSound = _sounds[SoundIndex];
            TargetSound.GetSamples(_soundBuffer, count, WaveFormat);
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

    private AudioContext CreateContext()
    {
        ALDevice DefaultDevice = ALC.OpenDevice(null);
        ALContext SoundContext = ALC.CreateContext(DefaultDevice, new ALContextAttributes());
        ALC.MakeContextCurrent(SoundContext);

        int SourceHandle = AL.GenSource();

        int[] Buffers = new int[BufferCount];
        AL.GenBuffers(BufferCount, Buffers);

        ALFormat Format = ALFormat.StereoFloat32Ext;
        const int MILLISECONDS_IN_SECOND = 1000;
        int BufferSize = WaveFormat.SampleRate * LatencyMilliseconds / MILLISECONDS_IN_SECOND * WaveFormat.Channels;
        float[] FinalBuffer = new float[BufferSize];

        AudioContext InitializedContext = new(DefaultDevice, SoundContext, SourceHandle, Buffers, Format, FinalBuffer);

        InitBuffers(InitializedContext, 0, BufferCount);
        AL.SourcePlay(InitializedContext.SourceHandle);

        return InitializedContext;
    }

    private void DestroyContext(AudioContext context)
    {
        AL.SourceStop(context.SourceHandle);
        AL.DeleteBuffers(context.BufferHandles);
        ALC.CloseDevice(context.SoundDevice);
    }

    private void InitBuffers(AudioContext context, int startBufferIndex, int count)
    {
        float[] TargetBuffer = context.FinalBuffer;
        int BufferIndex = startBufferIndex;
        for (int i = 0; i < count; i++)
        {
            Read(TargetBuffer, 0, TargetBuffer.Length);

            int BufferHandle = context.BufferHandles[BufferIndex];
            AL.BufferData(BufferHandle, context.TargetFormat, TargetBuffer, WaveFormat.SampleRate);
            AL.SourceQueueBuffer(context.SourceHandle, BufferHandle);

            BufferIndex = (BufferIndex + 1) % BufferCount;
        }
    }

    private void FillDequeuedBuffers(AudioContext context, int dequeueCount)
    {
        float[] Buffer = context.FinalBuffer;
        for (int i = 0; i < dequeueCount; i++)
        {
            Read(Buffer, 0, Buffer.Length);
            int BufferHandle = AL.SourceUnqueueBuffer(context.SourceHandle);
            AL.BufferData(BufferHandle, context.TargetFormat, Buffer, WaveFormat.SampleRate);
            AL.SourceQueueBuffer(context.SourceHandle, BufferHandle);
        }
    }

    private void MainThreadTask()
    {
        AudioContext Context = CreateContext();

        while (_isPlaying)
        {
            _executionMeasurer.Restart();

            _sounds.ApplyChanges();
            lock (_lockObject)
            {
                _scheduledActions.ApplyChanges();
            }
            ExecuteScheduledActions();

            int ProcessedBufferCount = AL.GetSource(Context.SourceHandle, ALGetSourcei.BuffersProcessed);
            if (ProcessedBufferCount > 0)
            {
                FillDequeuedBuffers(Context, ProcessedBufferCount);
            }

            ALSourceState State = (ALSourceState)AL.GetSource(Context.SourceHandle, ALGetSourcei.SourceState);
            if (State == ALSourceState.Stopped)
            {
                AL.SourcePlay(Context.SourceHandle);
            }

            Thread.Sleep(TimeSpan.FromMilliseconds((double)LatencyMilliseconds / (double)BufferCount / 2d));
        }

        DestroyContext(Context);
    }

    private int Read(float[] buffer, int offset, int count)
    {
        try
        {
            EnsureBuffer(count);
            ReadSounds(_volume, buffer, offset, count);
            ClampBufferValues(buffer, offset, count);

            return count;
        }
        catch (Exception e)
        {
            throw new Exception($"Exception in audio engine! {e}");
        }
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
        Stop();
    }

    public void Start()
    {
        /* Originally I used NAudio, but to support linux, switched to OpenAL. This switch, however,
        * was done in a rush, so while this code isn't AI generated, it was made in by skimming
        * over OpenAL docs and asking the GTP bot questions. It's rough. */

        /* References: https://research.ncl.ac.uk/game/mastersdegree/workshops/audio/Sound%20Workshop.pdf 
        * https://indiegamedev.net/2020/02/15/the-complete-guide-to-openal-with-c-part-1-playing-a-sound/ 
        * https://opentk.net/api/index.html */

        if (_isPlaying)
        {
            return;
        }
        _isPlaying = true;

        Task.Factory.StartNew(MainThreadTask, CancellationToken.None,
            TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    public void Stop()
    {
        _isPlaying = false;
    }

    public void ScheduleAction(params Action[] actions)
    {
        ArgumentNullException.ThrowIfNull(actions, nameof(actions));
        if (actions.Length == 0)
        {
            return;
        }

        lock (_lockObject)
        {
            foreach (Action ScheduledAction in actions)
            {
                _scheduledActions.Add(ScheduledAction);
            }
        }
    }


    // Types.
    private record class AudioContext(
        ALDevice SoundDevice,
        ALContext SoundContext,
        int SourceHandle,
        int[] BufferHandles,
        ALFormat TargetFormat,
        float[] FinalBuffer);
}