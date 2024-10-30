using GHEngine.IO.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets.Def;

public class JSONSongDeconstructor : JSONAssetDefinitionDeconstructor
{
    // Private static fields.
    private const string KEY_PATH = "path";


    // Private fields.
    private readonly JSONPathDeconstructor _pathDeconstructor = new();


    // Inherited methods.
    public override AssetDefinition DeconstructDefinition(string assetName, JSONCompound compound)
    {
        return new GHSongDefinition(assetName, _pathDeconstructor.GetPath(compound.GetVerified<object>(KEY_PATH)));
    }
}