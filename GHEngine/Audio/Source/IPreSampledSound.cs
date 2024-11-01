using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Audio.Source;

public interface IPreSampledSound : ISound
{
    // Fields.
    TimeSpan Duration { get; }
    float[] Samples { get; }
    int SampleCount { get; }
    int ChannelSampleCount { get; }
}