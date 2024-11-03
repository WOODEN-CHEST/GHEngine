using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Audio.Modifier;

public interface ISoundModifier
{
    // Methods.
    bool Modify(float[] buffer, int count, WaveFormat targetFormat);
}