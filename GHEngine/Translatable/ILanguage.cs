using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Translatable;

public interface ILanguage : ILanguageTextProvider, IDisposable
{
    // Fields.
    public string NameLocal { get; }
    public string NameEnglish { get; }


    // Methods.
    public void AddText(string key, string value);
    public void RemoveText(string key);
    public void Clear();
}