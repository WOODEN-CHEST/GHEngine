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
             _sampleSpeed = double.IsNaN(value) ? SAMPLE_SPEED_DEFAULT : Math.Clamp(value, SAMPLE_SPEED_MIN, SAMPLE_SPEED_MAX);
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
             _volume = float.IsNaN(value) ? VOLUME_DEFAULT : Math.Clamp(value, VOLUME_MIN, VOLUME_MAX);
        }
    }


    // Private fields.
    private float? _customSampleRate = null;
    private double _sampleSpeed = SAMPLE_SPEED_DEFAULT;
    private float _volume = VOLUME_DEFAULT;


    // Private methods.
    private float GetSample(double index, int channelIndex, int channelCount, float[] samples)
    {
        int LowerIndex = Math.Clamp((int)Math.Floor(index) * channelCount + channelIndex, 0, samples.Length - 1);
        int UpperIndex = Math.Clamp((int)Math.Ceiling(index) * channelCount + channelIndex, 0, samples.Length - 1);

        float LowerSample =  samples[LowerIndex];
        float UpperSample =  samples[UpperIndex];
        float InterpolationAmount = (float)(index % 1d);

        return LowerSample + ((UpperSample - LowerSample) * InterpolationAmount);
    }

    private bool IsSamplingNeeded(IPreSampledSound sound, bool isLooped, double index)
    {
        return isLooped || ((index >= 0d) && (index < sound.ChannelSampleCount));
    }

    private PreSampledSoundResult SampleAudio(float[] buffer,
        double soundOffset, 
        int count, 
        bool isLooped,
        IPreSampledSound sound,
        WaveFormat targetFormat)
    {
        double ChannelIndex = soundOffset;
        int SingleChannelCount = count / sound.Format.Channels;
        int BufferIndex;
        double SampleRateRatio = (_customSampleRate ?? sound.Format.SampleRate) / sound.Format.SampleRate;
        double AdjustedSpeed = _sampleSpeed * ((double)sound.Format.SampleRate / (double)targetFormat.SampleRate);
        double RateRange = 1d / SampleRateRatio;
        PreSampleSoundFinishType FinishType = PreSampleSoundFinishType.None;

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
                    (LowerSample + (UpperSample - LowerSample) * InterpolationAmount) * _volume;
            }

            ChannelIndex += AdjustedSpeed;
            if ((ChannelIndex < sound.ChannelSampleCount) && (ChannelIndex >= 0d))
            {
                continue;
            }

            if (!isLooped)
            {
                ChannelIndex = Math.Clamp(ChannelIndex, 0d, sound.ChannelSampleCount);
                FinishType = PreSampleSoundFinishType.Finished;
                break;
            }

            FinishType = PreSampleSoundFinishType.Looped;
            ChannelIndex = ChannelIndex > 0d ? (ChannelIndex % sound.ChannelSampleCount)
                : (sound.ChannelSampleCount - (Math.Abs(ChannelIndex) % sound.ChannelSampleCount));
        }

        Array.Fill(buffer, 0f, BufferIndex * targetFormat.Channels, (SingleChannelCount - BufferIndex) * targetFormat.Channels);

        return new(ChannelIndex, FinishType);
    }


    // Inherited methods.
    public PreSampledSoundResult Sample(float[] buffer,
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

        double ClampedIndex = isLooped ? (Math.Max(0d, soundOffset) % sound.ChannelSampleCount)
            : Math.Clamp(soundOffset, 0d, sound.ChannelSampleCount);

        return SampleAudio(buffer, ClampedIndex, count, isLooped, sound, targetFormat);
    }
}