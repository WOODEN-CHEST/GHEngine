using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Audio.Source;

public class GHPreSampledSoundSampler : IPreSampledSoundSampler
{
    // Static fields.
    public const float SAMPLE_RATE_MAX = 192_000f;
    public const float SAMPLE_RATE_MIN = float.Epsilon;

    public const double SAMPLE_SPEED_DEFAULT = 1d;
    public const double SAMPLE_SPEED_MIN = -100_000d;
    public const double SAMPLE_SPEED_MAX = 100_000d;

    public const float VOLUME_DEFAULT = 1f;
    public const float VOLUME_MIN = 0;
    public const float VOLUME_MAX = 100_000f;


    // Fields.
    public float? CustomSampleRate
    {
        get
        {
            lock (this)
            {
                return _customSampleRate;
            }
        }
        set
        {
            lock (this)
            {
                if (!value.HasValue || float.IsNaN(value.Value))
                {
                    _customSampleRate = null;
                }
                else
                {
                    _customSampleRate = Math.Clamp(value.Value, SAMPLE_RATE_MIN, SAMPLE_RATE_MAX);
                }
            }
        }
    }

    public double SampleSpeed
    {
        get
        {
            lock (this)
            {
                return _sampleSpeed;
            }
        }
        set
        {
            lock (this)
            {
                _sampleSpeed = double.IsNaN(value) ? SAMPLE_SPEED_DEFAULT : Math.Clamp(value, SAMPLE_SPEED_MIN, SAMPLE_SPEED_MAX);
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
                _volume = float.IsNaN(value) ? VOLUME_DEFAULT : Math.Clamp(value, VOLUME_MIN, VOLUME_MAX);
            }
        }
    }


    // Private fields.
    private float? _customSampleRate = null;
    private double _sampleSpeed = SAMPLE_SPEED_DEFAULT;
    private float _volume = VOLUME_DEFAULT;

    public event EventHandler<SoundLoopedArgs>? SoundLooped;
    public event EventHandler<SoundFinishedArgs>? SoundFinished;


    // Private methods.
    private float GetSample(double index, int channelIndex, int channelCount, float[] buffer)
    {
        int LowerIndex = Math.Clamp((int)Math.Floor(index) * channelCount + channelIndex, 0, buffer.Length - 1);
        int UpperIndex = Math.Clamp((int)Math.Ceiling(index) * channelCount + channelIndex, 0, buffer.Length);

        float LowerSample =  buffer[LowerIndex];
        float UpperSample =  buffer[UpperIndex];
        float InterpolationAmount = (float)(index % 1d);

        return LowerSample + ((UpperSample - LowerSample) * InterpolationAmount);
    }

    private bool IsSamplingNeeded(IPreSampledSound sound, bool isLooped, double index)
    {
        return isLooped || ((index >= 0d) && (index < sound.ChannelSampleCount));
    }

    private double Sample(float[] buffer,
        double soundOffset, 
        int count, 
        bool isLooped,
        double rate,
        double speed,
        float volume,
        IPreSampledSound sound,
        WaveFormat targetFormat)
    {
        double ChannelIndex = soundOffset;
        int SingleChannelCount = count / sound.Format.Channels;
        int BufferIndex;
        double SampleRateRatio = rate / sound.Format.SampleRate;
        double AdjustedSpeed = speed * ((double)sound.Format.SampleRate / (double)targetFormat.SampleRate);
        double RateRange = 1 / SampleRateRatio;

        for (BufferIndex = 0; (BufferIndex < SingleChannelCount) && (BufferIndex < buffer.Length)
            && IsSamplingNeeded(sound, isLooped, ChannelIndex); BufferIndex++)
        {
            for (int SelectedChannel = 0; SelectedChannel < sound.Format.Channels; SelectedChannel++)
            {
                double LowerIndex = Math.Floor(ChannelIndex * SampleRateRatio) / SampleRateRatio;
                double UpperIndex = Math.Ceiling(ChannelIndex * SampleRateRatio) / SampleRateRatio;

                float LowerSample = GetSample(LowerIndex, SelectedChannel, sound.Format.Channels, sound.Samples);
                float UpperSample = GetSample(UpperIndex, SelectedChannel, sound.Format.Channels, sound.Samples);

                float InterpolationAmount = (float)((ChannelIndex - LowerIndex) / RateRange);

                buffer[(BufferIndex * sound.Format.Channels) + SelectedChannel] =
                    (LowerSample + (UpperSample - LowerSample) * InterpolationAmount) * volume;
            }

            ChannelIndex += AdjustedSpeed;
            if ((ChannelIndex < sound.ChannelSampleCount) && (ChannelIndex >= 0d))
            {
                continue;
            }

            if (!isLooped)
            {
                ChannelIndex = Math.Clamp(ChannelIndex, 0d, sound.ChannelSampleCount);
                SoundFinished?.Invoke(this, new(sound));
                break;
            }

            SoundLooped?.Invoke(this, new(sound));
            ChannelIndex = ChannelIndex > 0d ? (ChannelIndex % sound.ChannelSampleCount)
                : (sound.ChannelSampleCount - (Math.Abs(ChannelIndex) % sound.ChannelSampleCount));
        }

        for (; BufferIndex < SingleChannelCount; BufferIndex++)
        {
            buffer[BufferIndex] = 0f;
        }
        
        return ChannelIndex;
    }


    // Inherited methods.
    public double Sample(float[] buffer,
        double soundOffset, 
        int count, 
        bool isLooped,
        IPreSampledSound sound,
        WaveFormat targetFormat)
    {
        if (count % sound.Format.Channels != 0)
        {
            throw new AudioSampleException($"Invalid number of samples requested: {count}");
        }

        float? CustomSampleRateCur;
        double SampleSpeedCur;
        float VolumeCur;

        lock (this) // Do NOT retrieve these from properties, it will cause a deadlock, use private fields.
        {
            CustomSampleRateCur = _customSampleRate;
            SampleSpeedCur = _sampleSpeed;
            VolumeCur = _volume;
        }

        return Sample(buffer, Math.Max(soundOffset, 0d) % sound.ChannelSampleCount, count, isLooped,
            CustomSampleRateCur ?? sound.Format.SampleRate, SampleSpeedCur, VolumeCur, sound, targetFormat);
    }
}