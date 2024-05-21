using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets;

public class GHSoundDefinition : GHSinglePathAssetDefinition
{
    // Constructors.
    public GHSoundDefinition(string name, string path) : base(AssetType.Sound, name, path) { }
}