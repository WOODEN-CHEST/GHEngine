using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets;

public abstract class AssetDefinition
{
    // Fields.
    public string Name { get; private init; }
    public AssetType Type { get; private init; }


    // Constructors.
    public AssetDefinition(AssetType type, string name)
    {
        Type = type;
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}