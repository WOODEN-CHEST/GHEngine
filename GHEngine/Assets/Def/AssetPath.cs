using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets.Def;

public readonly struct AssetPath
{
    // Fields.
    public string Path { get; private init; }
    public AssetPathType Type { get; private init; }


    // Constructors.
    public AssetPath(string path, AssetPathType type)
    {
        Path = path ?? throw new ArgumentNullException(nameof(path));
        Type = type;
    }


    // Static methods.
    public static AssetPath File(string path)
    {
        return new AssetPath(path, AssetPathType.FileSystem);
    }

    public static AssetPath Memory(string path)
    {
        return new AssetPath(path, AssetPathType.Memory);
    }
}