using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets.Def;

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
        ArgumentNullException.ThrowIfNull(definition, nameof(definition));
        EnsureTypeExistence(definition.Type);
        _definitions[definition.Type].Add(definition.Name, definition);
    }

    public void Remove(AssetDefinition definition)
    {
        _definitions.TryGetValue(definition.Type, out Dictionary<string, AssetDefinition>? TypeDefinitions);
        if (TypeDefinitions == null)
        {
            return;
        }
        TypeDefinitions.Remove(definition.Name);
    }

    public void Clear()
    {
        _definitions.Clear();
    }

    public AssetDefinition? Get(AssetType type, string name)
    {
        _definitions.TryGetValue(type, out Dictionary<string, AssetDefinition>? TypeDefinitions);
        if (TypeDefinitions == null)
        {
            return null;
        }
        TypeDefinitions.TryGetValue(name, out AssetDefinition? Definition);
        return Definition;
    }

    public AssetDefinition[] GetOfType(AssetType type)
    {
        _definitions.TryGetValue(type, out Dictionary<string, AssetDefinition>? TypeDefinitions);
        return TypeDefinitions?.Values.ToArray() ?? Array.Empty<AssetDefinition>();
    }

    public IEnumerator<AssetDefinition> GetEnumerator()
    {
        foreach (AssetDefinition Definition in _definitions.Values.SelectMany(definitions => definitions.Values))
        {
            yield return Definition;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}