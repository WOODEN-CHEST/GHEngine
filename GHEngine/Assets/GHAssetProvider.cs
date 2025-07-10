using GHEngine.Assets.Def;
using GHEngine.Assets.Loader;
using GHEngine.Logging;

namespace GHEngine.Assets;

public class GHAssetProvider : IAssetProvider
{
    // Private fields.
    private readonly object _lockObject = new();
    private readonly IAssetLoader _assetLoader;
    private readonly ILogger? _logger;

    private readonly IAssetDefinitionCollection _definitions;
    private readonly Dictionary<AssetType, IDisposable> _defaultAssets = new();
    private readonly Dictionary<AssetType, Dictionary<string, GHGameAsset>> _assets = new();
    


    // Constructors.
    public GHAssetProvider(IAssetLoader loader,
        IAssetDefinitionCollection definitions,
        ILogger? logger)
    {
        _assetLoader = loader ?? throw new ArgumentNullException(nameof(loader));
        _definitions = definitions ?? throw new ArgumentNullException(nameof(_definitions));
        _logger = logger;
    }


    // Methods.
    public void SetDefaultAsset(AssetType type, IDisposable asset)
    {
        lock (_defaultAssets)
        {
            _defaultAssets[type] = asset ?? throw new ArgumentNullException(nameof(asset));
        }
    }

    public void RemoveDefaultAsset(AssetType type)
    {
        lock (_defaultAssets)
        {
            _defaultAssets.Remove(type);
        }
    }

    public void ClearDefaultAssets(AssetType type)
    {
        lock (_defaultAssets)
        {
            _defaultAssets.Clear();
        }
    }


    // Private methods.
    private GHGameAsset? TryGetAsset(AssetType type, string name)
    {
        lock (_lockObject)
        {
            if (!_assets.TryGetValue(type, out Dictionary<string, GHGameAsset>? AssetDictionary))
            {
                return null;
            }

            AssetDictionary.TryGetValue(name, out GHGameAsset? Asset);
            return Asset;
        }
    }

    private IDisposable? TryLoadAsset(AssetType type, string name)
    {
        AssetDefinition? Definition = _definitions.Get(type, name);
        _defaultAssets.TryGetValue(type, out IDisposable? DefaultAsset);

        if (Definition == null)
        {
            _logger?.Warning($"Attempted to load undefined asset \"{name}\" of type \"{type}\"");
            return DefaultAsset;
        }

        try
        {
            return _assetLoader.Load(Definition);
        }
        catch (AssetLoadException e)
        {
            _logger?.Warning($"Exception while loading asset: {e}");
            return DefaultAsset;
        }
    }

    private GHGameAsset? CreateAsset(AssetType type, string name)
    {
        IDisposable? Asset = TryLoadAsset(type, name);
        if (Asset == null)
        {
            return null;
        }

        lock (_lockObject)
        {
            if (!_assets.TryGetValue(type, out var AssetDictionary))
            {
                AssetDictionary = new();
                _assets.Add(type, AssetDictionary);
            }

            if (AssetDictionary.TryGetValue(name, out GHGameAsset? ExistingAsset))
            {
                _logger?.Error($"Attempted to add asset which already exists! (Type: \"{type.TypeName}\", Name:\"{name}\")" +
                    $"Unloading old asset and replacing with new one.");
                ExistingAsset.Value.Dispose();
            }

            GHGameAsset GameAsset = new(Asset, type, name);
            AssetDictionary[name] = GameAsset;
            return GameAsset;
        }
    }

    private void ReleaseSingleAsset(object user, GHGameAsset asset, Dictionary<string, GHGameAsset> assetDict)
    {
        asset.RemoveUser(user);
        if (asset.UserCount == 0)
        {
            if (!_defaultAssets.ContainsValue(asset.Value))
            {
                asset.Value.Dispose();
            }

            assetDict.Remove(asset.Name);
        }
    }


    // Inherited methods.
    public T? GetAsset<T>(object user, AssetType type, string name) where T : class
    {
        GHGameAsset? RetrievedGameAsset = TryGetAsset(type, name) ?? CreateAsset(type, name);
        if (RetrievedGameAsset == null)
        {
            return null;
        }

        T? Asset = RetrievedGameAsset.Value as T;
        RetrievedGameAsset.AddUser(user);
        return Asset;
    }

    public void ReleaseAllAssets()
    {
        lock (_lockObject)
        {
            foreach (GHGameAsset Asset in _assets.Values.SelectMany(dict => dict.Values))
            {
                if (!_defaultAssets.ContainsValue(Asset.Value))
                {
                    Asset.Value.Dispose();
                }
            }
            _assets.Clear();
        }
    }

    public void ReleaseAsset(object user, AssetType type, string name)
    {
        lock (_lockObject)
        {
            if (!_assets.TryGetValue(type, out Dictionary<string, GHGameAsset>? AssetDictionary))
            {
                return;
            }

            if (!AssetDictionary.TryGetValue(name, out GHGameAsset? GameAsset))
            {
                return;
            }

            ReleaseSingleAsset(user, GameAsset, AssetDictionary);
        }
    }

    public void ReleaseAsset(object user, object asset)
    {
        lock (_lockObject)
        {
            foreach (var AssetDict in _assets.Values)
            {
                foreach (GHGameAsset GameAsset in AssetDict.Values)
                {
                    if (GameAsset.Value == asset)
                    {
                        ReleaseSingleAsset(user, GameAsset, AssetDict);
                    }
                }
            }
        }
    }

    public void ReleaseUserAssets(object user)
    {
        lock (_lockObject)
        {
            foreach (var AssetDict in _assets.Values)
            {
                foreach (GHGameAsset GameAsset in AssetDict.Values)
                {
                    if (GameAsset.ContainsUser(user))
                    {
                        ReleaseSingleAsset(user, GameAsset, AssetDict);
                    }
                }
            }
        }
    }


    // Types.
    private class GHGameAsset
    {
        // Fields.
        internal IDisposable Value { get; private init; }
        internal AssetType Type { get; private init; }
        internal string Name { get; private init; }
        internal int UserCount => _users.Count;


        // Private fields.
        private readonly HashSet<object> _users = new(2);


        // Constructors.
        internal GHGameAsset(IDisposable value, AssetType type, string name)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Type = type;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }


        // Methods
        public void AddUser(object user)
        {
            lock (_users)
            {
                _users.Add(user);
            }
        }

        public void RemoveUser(object user)
        {
            lock (_users)
            {
                _users.Remove(user);
            }
        }

        public bool ContainsUser(object user)
        {
            lock (_users)
            {
                return _users.Contains(user);
            }
        }
    }
}