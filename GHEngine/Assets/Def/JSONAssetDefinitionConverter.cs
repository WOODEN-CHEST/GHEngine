using GHEngine.IO.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets.Def;

public abstract class JSONAssetDefinitionConverter
{
    // Methods.
    public abstract AssetDefinition ReadDefinition(string assetName, JSONCompound compound);
    public abstract void WriteDefinition(AssetDefinition definition, JSONCompound compound);
}