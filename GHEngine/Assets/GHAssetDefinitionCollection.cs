using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets;

public class GHAssetDefinitionCollection : IAssetDefinitionCollection
{
    // Private fields.
    private readonly Dictionary<AssetType, Dictionary<string, AssetDefinition>> _definitions = new();

    // Constructors.
    public GHAssetDefinitionCollection() { }


    // Private methods.
    private void EnsureTypeExistence(AssetType type)
    {
        if (!_definitions.ContainsKey(type))
        {
            _definitions.Add(type, new());
        }
    }

    // Inherited methods.
    public void Add(AssetDefinition definition)
    {
        if (definition == null)
        {
            throw new ArgumentNullException(nameof(definition));
        }

        EnsureTypeExistence(definition.Type);
        _definitions[definition.Type].Add(definition.Name, definition);
    }

    public void Clear()
    {
        _definitions.Clear();
    }

    public AssetDefinition? Get(AssetType type, string name)
    {
        if (!_definitions.ContainsKey(type))
        {
            return null;
        }
        _definitions[type].TryGetValue(name, out AssetDefinition? definition);
        return definition;
    }

    public AssetDefinition[] GetAll()
    {
        return _definitions.SelectMany(Pair => Pair.Value.Values).ToArray();
    }

    public AssetDefinition[] GetAll(AssetType type)
    {
        if (!_definitions.ContainsKey(type))
        {
            return Array.Empty<AssetDefinition>();
        }
        return _definitions[type].Values.ToArray();
    }
}