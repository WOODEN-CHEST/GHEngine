using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets;

public class SoundDefinition : AssetDefinition
{
    // Constructors.
    public SoundDefinition(string name) : base(AssetType.Sound, name) { }
}