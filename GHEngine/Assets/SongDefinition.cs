using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets;

public class SongDefinition : AssetDefinition
{
    // Constructors.
    public SongDefinition(string name) : base(AssetType.Song, name) { }
}