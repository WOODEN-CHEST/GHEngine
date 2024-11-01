using GHEngine.Assets.Def;

namespace GHEngine.Assets.Loader;

public class GHGenericAssetLoader : IAssetLoader
{
    // Private fields.
    private readonly Dictionary<AssetType, IAssetLoader> _typeLoaders = new();


    // Constructors.
    public GHGenericAssetLoader() { }


    // Methods.
    public void SetTypeLoader(AssetType type, IAssetLoader loader)
    {
        _typeLoaders[type] = loader ?? throw new ArgumentNullException(nameof(type));
    }

    public void RemoveTypeLoader(AssetType type)
    {
        _typeLoaders.Remove(type);
    }

    public void ClearTypeLoaders()
    {
        _typeLoaders.Clear();
    }



    // Inherited methods.
    public object Load(AssetDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition, nameof(definition));

        _typeLoaders.TryGetValue(definition.Type, out IAssetLoader? Loader);
        if (Loader == null)
        {
            throw new AssetLoadException($"The asset type {definition.Type} is not supported by this generic loader.");
        }
        return Loader.Load(definition);
    }
}