using System.Text;
using GHEngine.Assets.Def;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace GHEngine.Assets.Loader;

public class GHAssetStreamOpener : IAssetStreamOpener
{
    // Private fields.
    private readonly Dictionary<string, Stream> _memoryAssetStreams = new();
    private readonly string _pathRoot;


    // Constructors.
    public GHAssetStreamOpener(string pathRoot)
    {
        _pathRoot = pathRoot ?? throw new ArgumentNullException(nameof(pathRoot));
    }


    // Private methods.
    private Stream OpenFileStream(string path)
    {
        try
        {
            string FinalPath = Path.Combine(_pathRoot, path);
            return File.OpenRead(FinalPath);
        }
        catch (IOException e)
        {
            throw new AssetLoadException($"Failed to open path to file stream asset: {e}");
        }
    }

    private Stream OpenMemoryStream(string path)
    {
        if (_memoryAssetStreams.TryGetValue(path, out Stream? TargetStream))
        {
            return TargetStream;
        }
        throw new AssetLoadException($"No memory stream for asset \"{path}\" found.");
    }

    private string EnsurePathSeparators(string path)
    {
        StringBuilder NewPath = new(path);

        for (int i = 0; i < NewPath.Length; i++)
        {
            char Character = NewPath[i];
            if ((Character == '\\') || (Character == '/'))
            {
                NewPath[i] = Path.DirectorySeparatorChar;
            }
        }

        return NewPath.ToString();
    }


    // Inherited methods.
    public Stream GetStream(AssetPath path)
    {
        string ModifiedPath = EnsurePathSeparators(path.Path);

        return path.Type switch
        {
            AssetPathType.FileSystem => OpenFileStream(ModifiedPath),
            AssetPathType.Memory => OpenMemoryStream(ModifiedPath),
            _ => throw new EnumValueException(nameof(path), path.Type),
        };
    }

    public void RemoveMemoryStream(string path)
    {
        ArgumentNullException.ThrowIfNull(path, nameof(path));
        string ModifiedPath = EnsurePathSeparators(path);
        _memoryAssetStreams.Remove(ModifiedPath);
    }

    public void SetMemoryStream(string path, Stream stream)
    {
        ArgumentNullException.ThrowIfNull(path, nameof(path));
        ArgumentNullException.ThrowIfNull(path, nameof(stream));
        string ModifiedPath = EnsurePathSeparators(path);
        _memoryAssetStreams[ModifiedPath] = stream;
    }

    public bool DoesFileExist(string path)
    {
        string FullPath = Path.Combine(_pathRoot, path);
        try
        {
            return File.Exists(FullPath);
        }
        catch (IOException e)
        {
            throw new AssetLoadException("Exception while checking if file exists.", e);
        }
    }
}