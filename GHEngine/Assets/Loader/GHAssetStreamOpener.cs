using GHEngine.Assets.Def;

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
        _memoryAssetStreams.TryGetValue(path, out Stream? TargetStream);
        if (TargetStream == null)
        {
            throw new AssetLoadException($"No memory stream for asset \"{path}\" found.");
        }
        return TargetStream;
    }


    // Inherited methods.
    public Stream GetStream(AssetPath path)
    {
        return path.Type switch
        {
            AssetPathType.FileSystem => OpenFileStream(path.Path),
            AssetPathType.Memory => OpenFileStream(path.Path),
            _ => throw new EnumValueException(nameof(path), path.Type),
        };
    }

    public void RemoveMemoryStream(string path)
    {
        ArgumentNullException.ThrowIfNull(path, nameof(path));
        _memoryAssetStreams.Remove(path);
    }

    public void SetMemoryStream(string path, Stream stream)
    {
        ArgumentNullException.ThrowIfNull(path, nameof(path));
        ArgumentNullException.ThrowIfNull(path, nameof(stream));
        _memoryAssetStreams[path] = stream;
    }
}