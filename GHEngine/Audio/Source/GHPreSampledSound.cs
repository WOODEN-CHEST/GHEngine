using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Audio.Source;

public class GHPreSampledSound : IPreSampledSound
{
    // Fields.
    public TimeSpan Duration { get; private init; }
    public float[] Samples { get; private init; }
    public int SampleCount => Samples.Length;
    public int ChannelSampleCount => Samples.Length / Format.Channels;
    public WaveFormat Format { get; private init; }


    // Constructors.
    public GHPreSampledSound(WaveFormat format, float[] samples)
    {
        Format = format ?? throw new ArgumentNullException(nameof(format));
        Samples = samples ?? throw new ArgumentNullException(nameof(samples));

        if (samples.Length % Format.Channels != 0)
        {
            throw new ArgumentException($"Sample count is not the same for every channel " +
                $"({samples.Length} samples, {format.Channels} channels)", nameof(samples));
        }
    }


    // Inehrited methods.
    public ISoundInstance CreateInstance()
    {
        return new GHPreSampledSoundInstance(this);
    }
}