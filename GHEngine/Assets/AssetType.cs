using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets;

public struct AssetType
{
    // Static fields.
    public static AssetType Animation { get; } = new("animation", "animations");
    public static AssetType Sound { get; } = new("sound", "sounds");
    public static AssetType Song { get; } = new("song", "songs");
    public static AssetType Shader { get; } = new("shader", "shaders");
    public static AssetType Font { get; } = new("font", "fonts");
    public static AssetType Language { get; } = new("language", "languages");


    
    // Fields.
    public string TypeName { get; private init; }
    public string RootPathName { get; private init; }


    // Constructors.
    public AssetType(string typeName, string rootPathName)
    {
        TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
        RootPathName = rootPathName ?? throw new ArgumentNullException(nameof(rootPathName));
    }


    // Inherited methods.
    public override bool Equals(object? obj)
    {
        if (obj is AssetType TargetAsset)
        {
            return TargetAsset.TypeName == TypeName;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return TypeName.GetHashCode();
    }

    public override string ToString()
    {
        return TypeName;
    }


    // Operators.
    public static explicit operator string(AssetType type) => type.TypeName;

    public static bool operator ==(AssetType a, AssetType b)
    {
        return a.TypeName == b.TypeName;
    }

    public static bool operator !=(AssetType a, AssetType b)
    {
        return a.TypeName != b.TypeName;
    }
}