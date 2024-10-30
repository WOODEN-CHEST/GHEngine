using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets.Def;

public class GHSongDefinition : GHSinglePathAssetDefinition
{
    // Constructors.
    public GHSongDefinition(string name, AssetPath path) : base(AssetType.Song, name, path) { }
}