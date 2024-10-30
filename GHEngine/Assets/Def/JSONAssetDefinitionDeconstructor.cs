using GHEngine.IO.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets.Def;

public abstract class JSONAssetDefinitionDeconstructor
{
    // Methods.
    public abstract AssetDefinition DeconstructDefinition(string assetName, JSONCompound compound);
}