using GHEngine.Assets.Def;
using GHEngine.Assets.Loader;
using GHEngine.Audio;
using GHEngine.Frame.Animation;
using GHEngine.Logging;
using GHEngine.Translatable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NAudio.Wave;

namespace GHEngine.Assets;

public class GHAssetProvider : IAssetProvider
{
    // Private fields.
    private readonly IAssetLoader _assetLoader;
    private readonly ILogger _logger;

    private readonly IAssetDefinitionCollection _definitions;
    private readonly Dictionary<AssetType, object> _defaultAssets = new();
    private readonly Dictionary<AssetType, Dictionary<string, GHGameAsset>> _assets = new();
    


    // Constructors.
    public GHAssetProvider(IAssetLoader loader,
        IAssetDefinitionCollection definitions,
        ILogger logger)
    {
        _assetLoader = loader ?? throw new ArgumentNullException(nameof(loader));
        _definitions = definitions ?? throw new ArgumentNullException(nameof(_definitions));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }


    // Private methods.
    private GHGameAsset? TryGetAsset(AssetType type, string name)
    {
        if (!_assets.TryGetValue(type, out Dictionary<string, GHGameAsset>? AssetDictionary))
        {
            return null;
        }

        AssetDictionary.TryGetValue(name, out GHGameAsset? Asset);
        return Asset;
    }

    private object? TryLoadAsset(AssetType type, string name)
    {
        AssetDefinition? Definition = _definitions.Get(type, name);
        _defaultAssets.TryGetValue(type, out object? DefaultAsset);

        if (Definition == null)
        {
            _logger.Warning($"Attempted to load undefined asset \"{name}\" of type \"{type}\"");
            return DefaultAsset;
        }

        try
        {
            return _assetLoader.Load(Definition);
        }
        catch (AssetLoadException e)
        {
            _logger.Warning(e.ToString());
            return DefaultAsset;
        }
    }

    private GHGameAsset? CreateAsset(AssetType type, string name)
    {
        object? Asset = TryLoadAsset(type, name);
        if (Asset == null)
        {
            return null;
        }

        if (!_assets.TryGetValue(type, out Dictionary<string, GHGameAsset>? AssetDictionary))
        {
            AssetDictionary = new();
            _assets.Add(type, AssetDictionary);
        }

        if (AssetDictionary.ContainsKey(name))
        {
            _logger.Error($"Attempted to add asset which already exists! (Type: \"{type.TypeName}\", Name:\"{name}\")" +
                $"Unloading old asset and replacing with new one.");
        }
        GHGameAsset GameAsset = new(Asset, type, name);
        AssetDictionary[name] = GameAsset;
        return GameAsset;
    }


    // Methods.
    public void SetDefaultAsset(AssetType type, object asset)
    {
        _defaultAssets[type] = asset ?? throw new ArgumentNullException(nameof(asset));
    }

    public void RemoveDefaultAsset(AssetType type)
    {
        _defaultAssets.Remove(type);
    }

    public void ClearDefaultAssets(AssetType type)
    {
        _defaultAssets.Clear();
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
        _assets.Clear();
    }

    public void ReleaseAsset(object user, AssetType type, string name)
    {
        if (!_assets.TryGetValue(type, out Dictionary<string, GHGameAsset>? AssetDictionary))
        {
            return;
        }

        if (!AssetDictionary.TryGetValue(name, out GHGameAsset? GameAsset))
        {
            return;
        }

        GameAsset.RemoveUser(user);
        if (GameAsset.UserCount == 0)
        {
            AssetDictionary.Remove(name);
        }
    }

    public void ReleaseAsset(object user, object asset)
    {
        foreach (GHGameAsset CurrentGameAsset in _assets.Values.SelectMany(assets => assets.Values))
        {
            if (CurrentGameAsset.Value == asset)
            {
                ReleaseAsset(user, CurrentGameAsset.Type, CurrentGameAsset.Name);
                break;
            }
        }
    }

    public void ReleaseUserAssets(object user)
    {
        foreach (GHGameAsset GameAsset in _assets.Values.SelectMany(assets => assets.Values))
        {
            if (GameAsset.ContainsUser(user))
            {
                ReleaseAsset(user, GameAsset.Type, GameAsset.Name);
            }
        }
    }


    // Types.
    private class GHGameAsset
    {
        // Fields.
        internal object Value { get; }
        internal AssetType Type { get; }
        internal string Name { get; }
        internal int UserCount => _users.Count;


        // Private fields.
        private readonly HashSet<object> _users = new(2);


        // Constructors.
        internal GHGameAsset(object value, AssetType type, string name)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Type = type;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }


        // Methods
        public void AddUser(object user)
        {
            _users.Add(user);
        }

        public void RemoveUser(object user)
        {
            _users.Remove(user);
        }

        public bool ContainsUser(object user)
        {
            return _users.Contains(user);
        }
    }
}