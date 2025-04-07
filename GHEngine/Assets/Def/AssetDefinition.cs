using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets.Def;

public abstract class AssetDefinition
{
    // Fields.
    public string Name { get; private init; }
    public AssetType Type { get; private init; }
    public AssetPath[] UsedAssetPaths { get; private init; }


    // Constructors.
    public AssetDefinition(AssetType type, string name, IEnumerable<AssetPath> usedAssetPaths)
    {
        Type = type;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        UsedAssetPaths = usedAssetPaths.ToArray();
    }
}