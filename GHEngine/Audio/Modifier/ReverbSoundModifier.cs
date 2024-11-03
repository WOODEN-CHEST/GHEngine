using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Audio.Modifier;

public class ReverbSoundModifier : ISoundModifier
{
    // Static fields.
    public const double DURATION_SECONDS_DEFAULT = 0.5d;
    public const double DURATION_SECONDS_MIN = 0d;
    public const double DURATION_SECONDS_MAX = 10d;

    public const float VOLUME_DEFAULT = 0.4f;
    public const float VOLUME_MAX = 0f;
    public const float VOLUME_MIN = 10_000f;

    public const int DELAY_STEPS_DEFAULT = 32;
    public const int DELAY_STEPS_MIN = 0;
    public const int DELAY_STEPS_MAX = 10_000;


    // Fields.
    public TimeSpan Duration
    {
        get => TimeSpan.FromSeconds(_durationSeconds);
        set => _durationSeconds = Math.Clamp(value.TotalSeconds,
            DURATION_SECONDS_MIN, DURATION_SECONDS_MAX);
    }

    public float Volume
    {
        get => _volume;
        set => _volume = float.IsNaN(value) ? VOLUME_DEFAULT : Math.Clamp(value, VOLUME_MIN, VOLUME_MAX);
    }

    public int DelaySteps
    {
        get => _delaySteps;
        set => _delaySteps = Math.Clamp(value, DELAY_STEPS_MIN, DELAY_STEPS_MAX);
    }


    // Private fields.
    private double _durationSeconds = DURATION_SECONDS_DEFAULT;
    private float _volume = VOLUME_DEFAULT;
    

    private WaveFormat? _previousFormat;
    private int _samplesToStore;
    private int _delaySteps = DELAY_STEPS_DEFAULT;
    private readonly List<float> _previousSamples = new();
    private float[] _bufferCopy = Array.Empty<float>();


    // Private methods.
    private void InitializeBuffers(WaveFormat targetFormat)
    {
        _previousSamples.Clear();
        _samplesToStore = (int)Math.Ceiling(targetFormat.Channels * targetFormat.SampleRate * _durationSeconds);
        _previousSamples.EnsureCapacity(_samplesToStore);
    }

    private void SetBufferCopy(float[] bufferToCopy)
    {
        if (_bufferCopy.Length < bufferToCopy.Length)
        {
            _bufferCopy = new float[bufferToCopy.Length];
        }
        Array.Copy(bufferToCopy, _bufferCopy, bufferToCopy.Length);
    }

    private float GetSample(float[] buffer, int index)
    {
        if ((index < 0) && (_previousSamples.Count > 0))
        {
            int IndexInList = _previousSamples.Count - 1 + index;
            return IndexInList < 0 ? 0f : _previousSamples[IndexInList];
        }
        if ((index >= 0) && (index < buffer.Length))
        {
            return buffer[index];
        }
        return 0f;
    }

    private bool AddReverb(float[] buffer, int count, WaveFormat targetFormat, float[] bufferCopy)
    {
        bool WasAnyChangeMade = false;
        int SamplesBetweenSteps = (int)(targetFormat.SampleRate * targetFormat.Channels * _durationSeconds / _delaySteps);

        for (int Index = 0; (Index < buffer.Length) && (Index < count); Index++)
        {
            for (int StepIndex = 0; StepIndex < _delaySteps; StepIndex++)
            {
                int Step = StepIndex + 1;
                float Sample = GetSample(bufferCopy, Index - (Step * SamplesBetweenSteps)) * (1f / (Step)) * Volume;
                WasAnyChangeMade |= Sample != 0f;
                buffer[Index] += Sample;
            }
        }
        return WasAnyChangeMade;
    }

    private void RecordSound(float[] buffer, int count)
    {
        int RemainingCapacity = _samplesToStore - _previousSamples.Count;
        if (RemainingCapacity - count < 0)
        {
            _previousSamples.RemoveRange(0, count - RemainingCapacity);
        }
        
        for (int i = 0; i < count; i++)
        {
            _previousSamples.Add(buffer[i]);
        }
    }


    // Methods.
    public void ClearSavedSamples()
    {
        _previousSamples.Clear();
    }


    // Inherited methods.
    public bool Modify(float[] buffer, int count, WaveFormat targetFormat)
    {
        if (!targetFormat.Equals(_previousFormat))
        {
            InitializeBuffers(targetFormat);
        }

        SetBufferCopy(buffer);

        bool WasAnyChangeMade = AddReverb(buffer, count, targetFormat, _bufferCopy);
        RecordSound(_bufferCopy, count);

        _previousFormat = targetFormat;
        return WasAnyChangeMade;
    }
}