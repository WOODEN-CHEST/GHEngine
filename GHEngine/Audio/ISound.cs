using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Audio;

public interface ISound
{
    // Fields.
    WaveFormat Format { get; }
    TimeSpan Duration { get; }
    float[] Samples { get; }
    long SampleCount { get; }
    long SingleChannelSampleCount { get; }


    // Methods.
    ISoundInstance CreateInstance();
}