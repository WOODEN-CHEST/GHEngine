using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine;

public class NameSpacedKey
{
    // Static fields.
    public const char SEPARATOR = ':';
    public const string NAMESPACE = "namespace";
    public const string KEY = "key";


    // Fields.
    public string Namespace { get; private init; }
    public string Key { get; private init; }


    // Private static fields.
    private static readonly HashSet<char> s_allowedCharacters = new()
    {
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
        'n', 'o', 'q', 'p', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
        'N', 'O', 'Q', 'P', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
        '_', '-'
    };


    // Constructors.
    public NameSpacedKey(string nameSpace, string key)
    {
        Namespace = nameSpace ?? throw new ArgumentNullException(nameof(nameSpace));
        Key = key ?? throw new ArgumentNullException(nameof(key));

        VerifyString(Namespace, NAMESPACE);
        VerifyString(Key, KEY);
    }

    public NameSpacedKey(string nameSpacedKey)
    {
        ArgumentNullException.ThrowIfNull(nameSpacedKey, nameof(nameSpacedKey));

        int Separator = nameSpacedKey.IndexOf(SEPARATOR);
        if ((Separator == -1) || (Separator == nameSpacedKey.Length - 1))
        {
            throw new FormatException("No key");
        }
        if (Separator == 0)
        {
            throw new FormatException("No namespace");
        }

        Namespace = nameSpacedKey[..Separator];
        Key = nameSpacedKey[(Separator + 1)..];

        VerifyString(Namespace, NAMESPACE);
        VerifyString(Key, KEY);
    }


    // Static methods.
    public static bool IsStringValid(string targetString)
    {
        foreach (char Character in targetString)
        {
            if (!s_allowedCharacters.Contains(Character))
            {
                return false;
            }
        }
        return true;
    }

    public static bool IsCharacterValid(char character)
    {
        return s_allowedCharacters.Contains(character);
    }


    // Private methods.
    private void VerifyString(string stringToVerify, string partName)
    {
        foreach (char Character in stringToVerify)
        {
            if (!s_allowedCharacters.Contains(Character))
            {
                throw new FormatException($"Invalid character in {partName}: '{Character}'");
            }
        }
    }


    // Inherited methods.
    public override bool Equals(object? obj)
    {
        if (obj is NameSpacedKey Target)
        {
            return (Key == Target.Key) && (Namespace == Target.Namespace);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Namespace.GetHashCode() * Key.GetHashCode();
    }

    public override string ToString()
    {
        return $"{Namespace}:{Key}";
    }
}