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
    // Constructors,
    public LanguageLoader(IAssetStreamOpener streamOpener) : base(streamOpener) { }


    // Inherited methods.
    public override object Load(AssetDefinition definition)
    {
        if (definition is not GHLanguageDefinition LanguageDefinition)
        {
            throw new AssetLoadException("Asset definition is not a font definition.");
        }

        ILanguage Language = new GHLanguage(LanguageDefinition.LanguageNameLocal, LanguageDefinition.LanguageNameEnglish);
        AssetPath FullPath = new AssetPath( Path.Combine(LanguageDefinition.Type.RootPathName,
            LanguageDefinition.TargetPath.Path), LanguageDefinition.TargetPath.Type);

        new JSONLanguageReader().Read(Language, StreamOpener.GetStream(FullPath));
        return Language;
    }
}