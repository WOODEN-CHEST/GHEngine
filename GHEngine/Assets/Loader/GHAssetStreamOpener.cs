using System.Text;
using GHEngine.Assets.Def;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace GHEngine.Assets.Loader;

public class GHAssetStreamOpener : IAssetStreamOpener
{
    // Private fields.
    private readonly Dictionary<string, Stream> _memoryAssetStreams = new();
    private string[] _rootAssetPaths = Array.Empty<string>();


    // Constructors.
    public GHAssetStreamOpener() { }


    // Private methods.
    private Stream OpenFileStream(string path)
    {
        string? SelectedPath = SelectAssetPath(path);
        if (SelectedPath == null)
        {
            throw new AssetLoadException($"No asset found for path {path}");
        }

        try
        {
            return File.OpenRead(SelectedPath);
        }
        catch (IOException e)
        {
            throw new AssetLoadException($"Failed to open path to file stream asset: {e}");
        }
    }

    private string? SelectAssetPath(string path)
    {
        foreach (string RootPath in _rootAssetPaths)
        {
            string FullPath = Path.Combine(RootPath, path);
            if (File.Exists(FullPath))
            {
                return FullPath;
            }
        }
        return null;
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

    public void RemoveMemoryStream(string path, bool disposeStream)
    {
        ArgumentNullException.ThrowIfNull(path, nameof(path));
        string ModifiedPath = EnsurePathSeparators(path);
        
        if (_memoryAssetStreams.TryGetValue(ModifiedPath, out var TargetStream))
        {
            _memoryAssetStreams.Remove(ModifiedPath);
            if (disposeStream)
            {
                TargetStream.Dispose();
            }
        }
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
        return SelectAssetPath(path) != null;
    }

    public void SetAssetPaths(string[]? rootAssetPaths)
    {
        lock (_memoryAssetStreams)
        {
            _rootAssetPaths = rootAssetPaths?.ToArray() ?? Array.Empty<string>();
        }
    }
}