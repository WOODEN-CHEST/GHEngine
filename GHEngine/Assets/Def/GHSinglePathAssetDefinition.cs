using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets.Def;

public abstract class GHSinglePathAssetDefinition : AssetDefinition
{
    // Fields.
    public AssetPath TargetPath { get; private init; }


    // Constructors.
    public GHSinglePathAssetDefinition(AssetType type, string name, AssetPath path) : base(type, name)
    {
        TargetPath = path;
    }
}
