using GHEngine.IO.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets.Def;

public class JSONSoundConverter : JSONAssetDefinitionConverter
{
    // Private static fields.
    private const string KEY_PATH = "path";


    // Private fields.
    private readonly JSONPathConverter _pathConverter = new();


    // Inherited methods.
    public override AssetDefinition ReadDefinition(string assetName, JSONCompound compound)
    {
        return new GHSoundDefinition(assetName, _pathConverter.GetPath(compound.GetVerified<object>(KEY_PATH)));
    }

    public override void WriteDefinition(AssetDefinition definition, JSONCompound compound)
    {
        GHSoundDefinition CastDefinition = (GHSoundDefinition)definition;
        compound.Add(KEY_PATH, _pathConverter.WritePath(CastDefinition.TargetPath));
    }
}