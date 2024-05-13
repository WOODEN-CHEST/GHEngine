using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace GHEngine.Audio;

public class GHSound : ISound
{
    // Public fields.
    public TimeSpan Length { get; private init; }


    // Fields.
    public float[] Samples { get; private set; }
    public long SampleCount => Samples.Length;
    public long SingleChannelSampleCount => Samples.Length / Format.Channels;
    public WaveFormat Format { get; private init; }
    public TimeSpan Duration { get; private init; }


    // Constructors.
    internal GHSound(float[] samples, WaveFormat format) 
    {
        Format = format ?? throw new ArgumentNullException(nameof(format));
        Samples = samples ?? throw new ArgumentNullException(nameof(samples));
        if (Samples.Length % format.Channels != 0)
        {
            throw new ArgumentException("Incorrect amount of samples for selected format." +
                $"Count: {samples.Length}, Format channel count: {format.Channels}", nameof(samples));
        }
        Duration = TimeSpan.FromSeconds((double)samples.Length / (double)format.Channels / (double)format.SampleRate);
    }


    // Methods.
    public ISoundInstance CreateInstance()
    {
        return new GHSoundInstance(this);
    }

    public void Dispose()
    {
        Samples = null!;
    }
}