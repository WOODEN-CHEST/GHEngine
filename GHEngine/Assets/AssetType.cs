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
    public static AssetType Animation { get; } = new("animation");
    public static AssetType Sound { get; } = new("sound");
    public static AssetType Song { get; } = new("song");
    public static AssetType Shader { get; } = new("shader");
    public static AssetType Font { get; } = new("font");
    public static AssetType Language { get; } = new("language");


    
    // Fields.
    public string TypeName { get; private init; }


    // Constructors.
    public AssetType(string typeName)
    {
        TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
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
    public static implicit operator string(AssetType type) => type.TypeName;

    public static bool operator ==(AssetType a, AssetType b)
    {
        return a.TypeName == b.TypeName;
    }

    public static bool operator !=(AssetType a, AssetType b)
    {
        return a.TypeName != b.TypeName;
    }
}