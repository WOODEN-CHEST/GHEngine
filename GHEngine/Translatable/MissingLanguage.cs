using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Translatable;

internal class MissingLanguage : ILanguage
{
    // Fields.
    public string NameLocal => "Missing Language";

    public string NameEnglish => "Missing Language";


    // Methods.
    public void AddText(string key, string value) { }

    public void Clear() { }

    public string GetText(string key) => "Missing Translation.";

    public void RemoveText(string key) { }
}