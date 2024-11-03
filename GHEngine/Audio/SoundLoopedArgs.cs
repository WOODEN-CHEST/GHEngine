using GHEngine.Audio.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Audio;

public class SoundLoopedArgs : EventArgs
{
    // Fields.
    public IPreSampledSoundInstance Sound { get; private init; }


    // Constructors.
    public SoundLoopedArgs(IPreSampledSoundInstance sound)
    {
        Sound = sound ?? throw new ArgumentNullException(nameof(sound));
    }
}