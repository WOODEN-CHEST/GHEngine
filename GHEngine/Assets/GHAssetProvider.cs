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
    private readonly GraphicsDevice _graphics;
    private readonly WaveFormat _audioFormat;

    private readonly IAssetDefinitionCollection _definitions;
    private readonly Dictionary<AssetType, Dictionary<string, GHGameAsset>> _assets = new();
    


    // Constructors.
    public GHAssetProvider(IAssetLoader loader,
        IAssetDefinitionCollection definitions,
        ILogger logger, 
        GraphicsDevice graphics,
        WaveFormat audioFormat)
    {
        _assetLoader = loader ?? throw new ArgumentNullException(nameof(loader));
        _definitions = definitions ?? throw new ArgumentNullException(nameof(_definitions));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _graphics = graphics ?? throw new ArgumentNullException(nameof(graphics));
        _audioFormat = audioFormat ?? throw new ArgumentNullException(nameof(audioFormat));
    }


    // Private methods.
    private GHGameAsset? TryGetAsset(AssetType type, string name)
    {
        if (!_assets.ContainsKey(type))
        {
            return null;
        }

        _assets[type].TryGetValue(name, out GHGameAsset? Asset);
        return Asset;
    }

    private object? TryLoadAsset(AssetType type, string name)
    {
        AssetDefinition? Definition = _definitions.Get(type, name);
        if (Definition == null)
        {
            _logger.Warning($"Attempted to load undefined asset \"{name}\" of type \"{type}\"");
            return GetDefaultAsset(type);
        }

        try
        {
            return _assetLoader.Load(Definition);
        }
        catch (AssetLoadException e)
        {
            _logger.Warning(e.ToString());
            return GetDefaultAsset(type);
        }
    }

    private object? GetDefaultAsset(AssetType type)
    {
        if (type == AssetType.Animation)
        {
            Texture2D Texture = new Texture2D(_graphics, 2, 2, false, SurfaceFormat.ColorSRgb);
            Texture.SetData(new Color[] { Color.HotPink, Color.Black, Color.Black, Color.HotPink });
            return new GHSpriteAnimation(0d, 0, true, null, false, Texture);
        }
        else if ((type == AssetType.Sound) || (type == AssetType.Song))
        {
            return new GHSound(Enumerable.Repeat(0f, _audioFormat.Channels).ToArray(), _audioFormat);
        }
        else if (type == AssetType.Language)
        {
            return new MissingLanguage();
        }
        else if (type == AssetType.Shader)
        {
            return new SpriteEffect(_graphics);
        }
        else if (type == AssetType.Font)
        {
            Texture2D Texture = new Texture2D(_graphics, 1, 1, false, SurfaceFormat.ColorSRgb);
            Texture.SetData(new Color[] { Color.White });
            return new SpriteFont(Texture, new(), new(), new(), 0, 0, new(), 'h');
        }
        else
        {
            _logger.Error($"Couldn't provide default asset for asset of type \"{type.TypeName}\"");
            return null;
        }
    }

    private GHGameAsset? CreateAsset(AssetType type, string name)
    {
        object? Asset = TryLoadAsset(type, name);
        if (Asset == null)
        {
            return null;
        }

        if (!_assets.ContainsKey(type))
        {
            _assets.Add(type, new());
        }
        if (_assets[type].ContainsKey(name))
        {
            _logger.Error($"Attempted to add asset which already exists! (Type: \"{type.TypeName}\", Name:\"{name}\")" +
                $"Unloading old asset and replacing with new one.");
            try
            {
                _assetLoader.Unload(_assets[type][name]);
            }
            catch (AssetUnloadException e)
            {
                _logger.Error(e.ToString());
            }
        }
        GHGameAsset GameAsset = new(Asset, type, name);
        _assets[type][name] = GameAsset;
        return GameAsset;
    }


    // Inherited methods.
    public T? GetAsset<T>(object user, AssetType type, string name) where T : class
    {
        GHGameAsset? RetreivedGameAsset = TryGetAsset(type, name) ?? CreateAsset(type, name);
        if (RetreivedGameAsset == null)
        {
            return null;
        }

        T? Asset = RetreivedGameAsset.Asset as T;
        RetreivedGameAsset.AddUser(user);
        return Asset;
    }

    public void ReleaseAllAssets()
    {
        foreach (GHGameAsset GameAsset in _assets.Values.SelectMany(assets => assets.Values))
        {
            try
            {
                _assetLoader.Unload(GameAsset.Asset);
            }
            catch (AssetUnloadException e)
            {
                _logger.Error(e.ToString());
            }
        }
        _assets.Clear();
    }

    public void ReleaseAsset(object user, AssetType type, string name)
    {
        if (!_assets.ContainsKey(type))
        {
            return;
        }

        _assets[type].TryGetValue(name, out GHGameAsset? GameAsset);
        if (GameAsset == null)
        {
            return;
        }

        GameAsset.RemoveUser(user);
        if (GameAsset.UserCount == 0)
        {
            _assetLoader.Unload(GameAsset.Asset);
            _assets[type].Remove(name);
        }
    }

    public void ReleaseAsset(object user, object asset)
    {
        foreach (GHGameAsset CurrentGameAsset in _assets.Values.SelectMany(assets => assets.Values))
        {
            if (CurrentGameAsset.Asset == asset)
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
        internal object Asset { get; }
        internal AssetType Type { get; }
        internal string Name { get; }
        internal int UserCount => _users.Count;


        // Private fields.
        private readonly HashSet<object> _users = new(2);


        // Constructors.
        internal GHGameAsset(object asset, AssetType type, string name)
        {
            Asset = asset ?? throw new ArgumentNullException(nameof(asset));
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