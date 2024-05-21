using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets;

public abstract class GHSinglePathAssetDefinition : AssetDefinition
{
    // Fields.
    public string AssetPath { get; private init; } 

    // Constructors.
    public GHSinglePathAssetDefinition(AssetType type, string name, string path) : base(type, name)
    {
        AssetPath = path ?? throw new ArgumentNullException(nameof(path));
    }
}
