using NAudio.Dsp;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Audio.Modifier;

public class BiQuadSoundModifier : ISoundModifier
{
    // Static fields.
    public const float FILTER_ORDER = 3f;


    // Fields.
    public float Frequency
    {
        get => _frequency;
        set => _frequency = float.IsNaN(value) ? 0f : Math.Max(0f, value);
    }

    public BiQuadPassType PassType { get; set; }



    // Private fields.
    private float _frequency = 0;
    private WaveFormat? _previousFormat;
    private BiQuadFilter[]? _previousFilters;



    // Private methods.
    private BiQuadFilter[] GetFilters(WaveFormat format)
    {
        BiQuadFilter[] Filters = new BiQuadFilter[format.Channels];

        for (int i = 0; i < Filters.Length; i++)
        {
            Filters[i] = PassType switch
            {
                BiQuadPassType.Low => BiQuadFilter.LowPassFilter(format.SampleRate, _frequency, FILTER_ORDER),
                BiQuadPassType.High => BiQuadFilter.HighPassFilter(format.SampleRate, _frequency, FILTER_ORDER),
                _ => throw new EnumValueException(nameof(PassType), PassType)
            };
        }

        return Filters;
    }


    // Inherited methods.
    public bool Modify(float[] buffer, int count, WaveFormat targetFormat)
    {
        BiQuadFilter[] Filters = targetFormat.Equals(_previousFormat) ? _previousFilters! : GetFilters(targetFormat);

        int SamplesPerChannel = count / targetFormat.Channels;
        for (int Index = 0; Index < SamplesPerChannel; Index++)
        {
            int BaseIndex = Index * targetFormat.Channels;
            for (int SelectedChannel = 0; SelectedChannel < targetFormat.Channels; SelectedChannel++)
            {
                buffer[BaseIndex + SelectedChannel] = Filters[SelectedChannel].Transform(buffer[BaseIndex + SelectedChannel]);
            }
        }

        _previousFormat = targetFormat;
        _previousFilters = Filters;
        return false;
    }
}