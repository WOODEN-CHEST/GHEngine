using GHEngine.Assets.Def;
using GHEngine.Translatable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets.Loader;

public class LanguageLoader : GHStreamAssetLoader
{
    // Static fields.
    public const string FILE_EXTENSION = ".json";


    // Constructors.
    public LanguageLoader(IAssetStreamOpener streamOpener) : base(streamOpener) { }


    // Inherited methods.
    public override IDisposable Load(AssetDefinition definition)
    {
        if (definition is not GHLanguageDefinition LanguageDefinition)
        {
            throw new AssetLoadException("Asset definition is not a font definition.");
        }

        string ModifiedPath = Path.Combine(LanguageDefinition.Type.RootPathName, LanguageDefinition.TargetPath.Path);
        if (LanguageDefinition.TargetPath.Type == AssetPathType.FileSystem)
        {
            ModifiedPath = Path.ChangeExtension(ModifiedPath, FILE_EXTENSION);
        }
        AssetPath FullPath = new AssetPath(ModifiedPath, LanguageDefinition.TargetPath.Type);

        ILanguage Language = new GHLanguage(LanguageDefinition.LanguageNameLocal, LanguageDefinition.LanguageNameEnglish);
        new JSONLanguageReader().Read(Language, StreamOpener.GetStream(FullPath));
        return Language;
    }
}