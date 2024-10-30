using GHEngine.IO.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets.Def;

public class JSONPathDeconstructor
{
    // Private static fields.
    private const int LIST_LENGTH = 2;
    private const string TYPE_FILE = "file";
    private const string TYPE_MEMORY = "memory";
    private const string KEY_NAME = "name";
    private const string KEY_TYPE = "type";


    // Private methods.
    private AssetPath GetSimplePath(string path)
    {
        return AssetPath.File(path);
    }

    private AssetPathType StringToPathType(string type)
    {
        return type switch
        {
            TYPE_FILE => AssetPathType.FileSystem,
            TYPE_MEMORY => AssetPathType.Memory,
            _ => throw new JSONEntryException($"Invalid asset path type: \"{type}\"")
        };
    }

    private AssetPath GetListPath(JSONList list)
    {
        if (list.Count != LIST_LENGTH)
        {
            throw new JSONEntryException($"Invalid entry count in path list: {list.Count}, expected {LIST_LENGTH}");
        }

        string PathName = list.GetVerified<string>(0);
        AssetPathType PathType = StringToPathType(list.GetVerified<string>(1));
        return new AssetPath(PathName, PathType);
    }

    private AssetPath GetCompoundPath(JSONCompound compound)
    {
        string PathName = compound.GetVerified<string>(KEY_NAME);
        AssetPathType PathType = StringToPathType(compound.GetVerified<string>(KEY_TYPE));
        return new AssetPath(PathName, PathType);
    }


    // Methods.
    public AssetPath GetPath(object path)
    {
        if (path is string TargetString)
        {
            return GetSimplePath(TargetString);
        }
        if (path is JSONList TargetList)
        {
            return GetListPath(TargetList);
        }
        if (path is JSONCompound TargetCompound)
        {
            return GetCompoundPath(TargetCompound);
        }
        throw new JSONEntryException($"Can't resolve asset path with value: {new JSONSerializer().Serialize(path, false)}");
    }
}