using GHEngine.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets;

public class GHAssetProvider : IAssetProvider
{
    // Private fields.
    private readonly IAssetLoader _assetLoader;
    private readonly ILogger _logger;
    private readonly IAssetDefinitionCollection _definitions;

    private readonly Dictionary<object, HashSet<object>> _assetUsers;
    private readonly Dictionary<object, HashSet<object>> _assetUsers;

    private readonly Dictionary<AssetType, Dictionary<string, GHGameAsset>> _assets = new();
    


    // Constructors.
    public GHAssetProvider(IAssetLoader loader, IAssetDefinitionCollection definitions, ILogger logger)
    {
        _assetLoader = loader ?? throw new ArgumentNullException(nameof(loader));
        _definitions = definitions ?? throw new ArgumentNullException(nameof(_definitions));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }


    // Private methods.
    private object? TryGetLoadedAsset(AssetType type, string name)
    {
        if (!_assets.ContainsKey(type))
        {
            return null;
        }

        _assets[type].TryGetValue(name, out var Asset);
        return Asset;
    }

    private object? TryLoadAsset(AssetType type, string name)
    {
        try
        {
            return _assetLoader.Load(type, name);
        }
        catch (AssetLoadException e)
        {
            _logger.Warning($"Failed to load asset \"{type}\" of type \"{name}\": {e}");
            return null;
        }
    }



    // Inherited methods.
    public T? GetAsset<T>(object user, AssetType type, string name) where T : class
    {
        object? RetreivedAsset = TryGetLoadedAsset(type, name);
        if (RetreivedAsset != null)
        {
            return RetreivedAsset as T;
        }

        return TryLoadAsset(type, name) as T;
    }

    public void ReleaseAllAssets()
    {
        for (_assets)
    }

    public void ReleaseAsset(object user, AssetType type, string name)
    {
        throw new NotImplementedException();
    }

    public void ReleaseAsset(object user, object asset)
    {
        throw new NotImplementedException();
    }

    public void ReleaseUserAssets(object user)
    {
        throw new NotImplementedException();
    }
}