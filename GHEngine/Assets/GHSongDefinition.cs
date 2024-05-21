using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets;

public class GHSongDefinition : GHSinglePathAssetDefinition
{
    // Constructors.
    public GHSongDefinition(string name, string path) : base(AssetType.Song, name, path) { }
}