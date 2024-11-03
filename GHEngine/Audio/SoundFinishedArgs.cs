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
    public ISoundInstance Instance { get; private init; }


    // Constructors.
    public SoundFinishedArgs(ISoundInstance sound)
    {
        Instance = sound ?? throw new ArgumentNullException(nameof(sound));
    }
}