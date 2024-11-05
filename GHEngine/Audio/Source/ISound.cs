using GHEngine.Assets;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Audio.Source;

public interface ISound
{
    // Fields.
    WaveFormat Format { get; }


    // Methods.
    public ISoundInstance CreateInstance();
}