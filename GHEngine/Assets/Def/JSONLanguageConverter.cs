using GHEngine.IO.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets.Def;

public class JSONLanguageConverter : JSONAssetDefinitionConverter
{
    // Private static fields.
    private const string KEY_PATH = "path";
    private const string KEY_NAME_LOCAL = "name_local";
    private const string KEY_NAME_ENGLISH = "name_english";


    // Private fields.
    private readonly JSONPathConverter _pathConverter = new();


    // Inherited methods.
    public override AssetDefinition ReadDefinition(string assetName, JSONCompound compound)
    {
        AssetPath Path = _pathConverter.GetPath(compound.GetVerified<object>(KEY_PATH));
        string NameLocal = compound.GetVerified<string>(KEY_NAME_LOCAL);
        string NameEnglish = compound.GetVerified<string>(KEY_NAME_ENGLISH);
        return new GHLanguageDefinition(assetName, Path, NameEnglish, NameLocal);
    }

    public override void WriteDefinition(AssetDefinition definition, JSONCompound compound)
    {
        GHLanguageDefinition CastDefinition = (GHLanguageDefinition)definition;

        compound.Add(KEY_PATH, _pathConverter.WritePath(CastDefinition.TargetPath));
        compound.Add(KEY_NAME_LOCAL, CastDefinition.LanguageNameLocal);
        compound.Add(KEY_NAME_ENGLISH, CastDefinition.LanguageNameEnglish);
    }
}