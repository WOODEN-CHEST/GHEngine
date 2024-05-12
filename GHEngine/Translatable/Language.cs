﻿using System.Collections.Generic;
using System.Linq;
using System;
using System.Transactions;
using GHEngine.Translatable;

namespace GHEngine.Translatable;

public class Language : ILanguageTextProvider
{
    // Fields.
    public string Name { get; private init; }


    // Private fields.
    private readonly Dictionary<string, string> _definitions = new();


    // Constructors.
    public Language(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException($"Invalid language name \"{name}\"", nameof(name));
        }
        Name = name;
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
}