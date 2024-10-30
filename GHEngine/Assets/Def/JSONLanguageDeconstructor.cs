using GHEngine.IO.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets.Def;

public class JSONLanguageDeconstructor : JSONAssetDefinitionDeconstructor
{
    // Private static fields.
    private const string KEY_PATH = "path";
    private const string KEY_NAME_LOCAL = "name_local";
    private const string KEY_NAME_ENGLISH = "name_english";


    // Private fields.
    private readonly JSONPathDeconstructor _pathDeconstructor = new();


    // Inherited methods.
    public override AssetDefinition DeconstructDefinition(string assetName, JSONCompound compound)
    {
        AssetPath Path = _pathDeconstructor.GetPath(compound.GetVerified<object>(KEY_PATH));
        string NameLocal = compound.GetVerified<string>(KEY_NAME_LOCAL);
        string NameEnglish = compound.GetVerified<string>(KEY_NAME_ENGLISH);
        return new GHLanguageDefinition(assetName, Path, NameEnglish, NameLocal);
    }
}