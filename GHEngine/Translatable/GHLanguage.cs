using System.Collections.Generic;
using System.Linq;
using System;
using System.Transactions;
using GHEngine.Translatable;

namespace GHEngine.Translatable;

public class GHLanguage : ILanguage
{
    // Fields.
    public string NameLocal { get; private init; }
    public string NameEnglish { get; private init; }


    // Private fields.
    private readonly Dictionary<string, string> _definitions = new();


    // Constructors.
    public GHLanguage(string nameLocal, string nameEnglish)
    {
        if (string.IsNullOrWhiteSpace(nameLocal))
        {
            throw new ArgumentException($"Invalid local language name \"{nameLocal}\"", nameof(nameLocal));
        }
        if (string.IsNullOrWhiteSpace(nameLocal))
        {
            throw new ArgumentException($"Invalid English language name \"{nameEnglish}\"", nameof(nameEnglish));
        }

        NameLocal = nameLocal;
        NameEnglish = nameEnglish;
    }


    // Methods.
    public void SetText(string key, string value)
    {
        _definitions[key] = value ?? throw new ArgumentNullException(nameof(value));
    }

    public void Clear()
    {
        _definitions.Clear();
    }


    // Inherited methods.
    public string GetText(string key)
    {
        if (_definitions.TryGetValue(key, out string? Value))
        {
            return Value!;
        }
        return key;
    }

    public void AddText(string key, string value)
    {
        throw new NotImplementedException();
    }

    public void RemoveText(string key)
    {
        throw new NotImplementedException();
    }

    public void Dispose() { }
}