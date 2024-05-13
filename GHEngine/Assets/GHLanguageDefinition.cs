using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets;

public class GHLanguageDefinition : AssetDefinition
{
    // Fields.
    public string LanguageNameEnglish { get; private init; }
    public string LanguageNameLocal { get; private init; }


    // Constructors.
    public GHLanguageDefinition(string name, string languageNameEnglish, string languageNameLocal)
        : base(AssetType.Language, name)
    {
        LanguageNameEnglish = languageNameEnglish ?? throw new ArgumentNullException(nameof(languageNameEnglish));
        LanguageNameLocal = languageNameLocal ?? throw new ArgumentNullException(nameof(languageNameLocal));
    }
}