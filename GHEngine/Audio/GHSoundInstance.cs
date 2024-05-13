﻿using NAudio.Dsp;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Tests.Audio;

namespace GHEngine.Audio;

public class GHSoundInstance : ISoundInstance
{
    // Static fields.
    public const float PAN_LEFT = -1f;
    public const float PAN_MIDDLE = 0f;
    public const float PAN_RIGHT = 1f;
    public const float VOLUME_MIN = 0f;
    public const float VOLUME_MAX = 1000f;
    public const double SPEED_MIN = -10;
    public const double SPEED_DEFAULT = 1d;
    public const double SPEED_MAX = 10d;


    // Fields.
    public ISound Sound { get; private init; }

    public TimeSpan CurrentTime
    {
        get
        {
            lock (this)
            {
                return TimeSpan.FromSeconds(_properties.Index / Sound.Format.SampleRate);
            }
        }
        set
        {
            lock (this)
            {
                double Sample = value.Seconds * Sound.Format.SampleRate;
                _properties.Index = Math.Clamp(Sample, 0d, Sound.SampleCount);
            }
        }
    }

    public SoundInstanceState State
    {
        get
        {
            lock (this) { return _properties.State; }
        }
        set
        {
            lock (this) { _properties.State = value; }
        }
    }

    public bool IsLooped
    {
        get
        {
            lock (this) { return _properties.IsLooped; }
        }
        set
        {
            lock (this) { _properties.IsLooped = value; }
        }
    }

    public float Volume
    {
        get
        {
            lock (this) { return _properties.Volume; }
        }
        set
        {
            lock (this)
            {
                _properties.Volume = Math.Clamp(value, VOLUME_MIN, VOLUME_MAX);
            }
        }
    }

    public float Pan
    {
        get 
        {
            lock (this) { return _properties.Pan; }
        }
        set
        {
            lock (this)
            {
                _properties.Pan = Math.Clamp(value, PAN_LEFT,  PAN_RIGHT);
            }
        }
    }

    public double Speed
    {
        get
        {
            lock (this) { return _properties.Speed; }
        }
        set
        {
            lock (this)
            {
                _properties.Speed = Math.Clamp(value, SPEED_MIN, SPEED_MAX);
            }
        }
    }

    public int? LowPassFrequency
    {
        get
        {
            lock (this) { return _properties.LowPassCutoffFrequency; }
        }
        set
        {
            lock (this)
            {
                _properties.LowPassCutoffFrequency = value.HasValue ?
                    Math.Clamp(value!.Value, 0, Sound.Format.SampleRate / 2)
                    : null;
            }
        }
    }

    public int? HighPassFrequency
    {
        get
        {
            lock (this) { return _properties.HighPassCutoffFrequency; }
        }
        set
        {
            lock (this)
            {
                _properties.HighPassCutoffFrequency = value.HasValue ?
                    Math.Clamp(value!.Value, 0, Sound.Format.SampleRate / 2)
                    : null;
            }
        }
    }

    public event EventHandler<SoundFinishedArgs>? SoundFinished;
    public event EventHandler<SoundLoopedArgs>? SoundLooped;


    // Private static fields.
    private const float FILTER_ORDER = 3f;


    // Private fields.
    private SoundInstanceProperties _properties;

    private BiQuadFilter[] _filters;
    private readonly BiQuadFilter _filterLeft = BiQuadFilter.LowPassFilter(0, 0, 0);
    private readonly BiQuadFilter _filterRight = BiQuadFilter.LowPassFilter(0, 0, 0);


    // Constructors.
    internal GHSoundInstance(ISound sound)
    {
        Sound = sound ?? throw new ArgumentNullException(nameof(sound));
        _properties = new(); 
        _filters = new BiQuadFilter[Sound.Format.Channels];
    }


    // Private methods.
    private void EnsureFilterCount()
    {
        if (_filters.Length != Sound.Format.Channels)
        {
            _filters = new BiQuadFilter[Sound.Format.Channels];
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void PanPass(float[] buffer, int count, float pan)
    {
        if (Sound.Format.Channels != 2)
        {
            throw new NotSupportedException("Panning not supported for non 2 channel sounds.");
        }

        const float MIN_MAIN_VOLUME = 0.5f;
        const float MAX_SECONDARY_VOLUME = 0.5f;
        const float MAX_VOLUME_BOOST = 0.4f;

        bool PannedToLeft = pan < PAN_MIDDLE;
        pan = PannedToLeft ? pan : (PAN_RIGHT - pan);
        float VolumeBoost = 1f + MAX_VOLUME_BOOST * (1f - pan);

        if (PannedToLeft)
        {
            float LeftVolumeInLeft = (MIN_MAIN_VOLUME + (MAX_SECONDARY_VOLUME * pan)) * VolumeBoost;
            float RightVolumeInLeft = VOLUME_MAX - LeftVolumeInLeft;
            float RightVolume = pan;

            for (int i = 0; i < count - 1; i += 2)
            {
                buffer[i] = (buffer[i] * LeftVolumeInLeft) + (buffer[i + 1] * RightVolumeInLeft);
                buffer[i + 1] *= RightVolume;
            }
        }
        else
        {
            float RightVolumeInRight = (MIN_MAIN_VOLUME + (MAX_SECONDARY_VOLUME * pan)) * VolumeBoost;
            float LeftVolumeInRight = VOLUME_MAX - RightVolumeInRight;
            float LeftVolume = pan;

            for (int i = 0; i < count - 1; i += 2)
            {
                buffer[i] *= LeftVolume;
                buffer[i + 1] = (buffer[i + 1] * RightVolumeInRight) + (buffer[i] * LeftVolumeInRight);
            }
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void BiQuadFilterPass(float[] buffer, int count)
    {
        if (count % _filters.Length != 0)
        {
            throw new ArgumentException("Invalid sample count requested, does not match with filter count.");
        }

        for (int BufferIndex = 0; BufferIndex < count;)
        {
            for (int FilterIndex = 0; (FilterIndex < _filters.Length) && (BufferIndex < count);
                FilterIndex++, BufferIndex++)
            {
                buffer[BufferIndex] = _filters[FilterIndex].Transform(buffer[BufferIndex]);
            }
        }
    }


    /* Reading. */
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private float GetSingleSample(double index, int channelIndex, bool isLooped)
    {
        double MaxIndex = Sound.SingleChannelSampleCount - 1d;
        double ClampedIndex = isLooped ? index % (Sound.SingleChannelSampleCount - 1d) : index;
        double LowerIndex = Math.Floor(ClampedIndex);
        double UpperIndex = Math.Ceiling(ClampedIndex);
        long IndexMultiplier = channelIndex + 1;

        float LowerSample = LowerIndex <= MaxIndex ? Sound.Samples[(long)LowerIndex * IndexMultiplier] : 0f;
        float UpperSample = UpperIndex <= MaxIndex ? Sound.Samples[(long)UpperIndex * IndexMultiplier] : 0f;

        return LowerSample + (UpperSample - LowerSample) * (float)(ClampedIndex % 1d);
    }

    private double ReadSamples(float[] buffer, double sourceIndex, int count, float volume, double speed, bool isLooped)
    {
        double SourceIndex = sourceIndex;
        for (int BufferIndex = 0; BufferIndex < count; SourceIndex += speed)
        {
            for (int ChannelIndex = 0; (ChannelIndex < Sound.Format.Channels) && (BufferIndex < count);
                ChannelIndex++, BufferIndex++)
            {
                buffer[BufferIndex] = GetSingleSample(SourceIndex, ChannelIndex, isLooped) * volume;
            }
        }

        return SourceIndex;
    }


    // Inherited methods.
    public void GetSamples(float[] buffer, int sampleCount)
    {
        SoundInstanceProperties CurrentProperties;
        lock (this) 
        {
            CurrentProperties = _properties;
        }

        try
        {
            ReadSamples(buffer, CurrentProperties.Index, sampleCount, CurrentProperties.Volume,
                CurrentProperties.Speed, CurrentProperties.IsLooped);

            if (CurrentProperties.Pan != PAN_MIDDLE)
            {
                PanPass(buffer, sampleCount, CurrentProperties.Pan);
            }
            if (CurrentProperties.LowPassCutoffFrequency != null)
            {
                EnsureFilterCount();
                for (int i = 0; i < _filters.Length; i++)
                {
                    _filters[i].SetLowPassFilter(Sound.Format.SampleRate,
                        CurrentProperties.LowPassCutoffFrequency.Value, FILTER_ORDER);
                }
                BiQuadFilterPass(buffer, sampleCount);
            }
            if (CurrentProperties.HighPassCutoffFrequency != null)
            {
                EnsureFilterCount();
                for (int i = 0; i < _filters.Length; i++)
                {
                    _filters[i].SetLowPassFilter(Sound.Format.SampleRate,
                        CurrentProperties.HighPassCutoffFrequency.Value, FILTER_ORDER);
                }
                BiQuadFilterPass(buffer, sampleCount);
            }
        }
        catch (IndexOutOfRangeException e)
        {
            throw new InvalidOperationException($"Corrupted audio data which caused index to become out of bounds. {e}");
        }
    }
}