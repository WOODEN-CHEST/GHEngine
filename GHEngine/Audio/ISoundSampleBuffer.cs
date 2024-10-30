using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Audio;

public interface ISoundSampleBuffer
{
    // Fields.
    WaveFormat Format { get; }
    float this[int index] { get; }
    float this[double index] { get; }
}