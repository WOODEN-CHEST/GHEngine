using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets.Def;

public class GHLanguageDefinition : GHSinglePathAssetDefinition
{
    // Fields.
    public string LanguageNameEnglish { get; private init; }
    public string LanguageNameLocal { get; private init; }


    // Constructors.
    public GHLanguageDefinition(string name, AssetPath path, string languageNameEnglish, string languageNameLocal)
        : base(AssetType.Language, name, path)
    {
        LanguageNameEnglish = languageNameEnglish ?? throw new ArgumentNullException(nameof(languageNameEnglish));
        LanguageNameLocal = languageNameLocal ?? throw new ArgumentNullException(nameof(languageNameLocal));
    }
}