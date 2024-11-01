using GHEngine.Audio.Source;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Audio;

public class SoundFinishedArgs : EventArgs
{
    // Fields.
    public ISound Sound { get; private init; }


    // Constructors.
    public SoundFinishedArgs(ISound sound)
    {
        Sound = sound ?? throw new ArgumentNullException(nameof(sound));
    }
}