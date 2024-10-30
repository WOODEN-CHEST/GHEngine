using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets.Def;

public interface IAssetDefinitionCollection : IEnumerable<AssetDefinition>
{
    void Add(AssetDefinition definition);
    void Remove(AssetDefinition definition);
    AssetDefinition? Get(AssetType type, string name);
    AssetDefinition[] GetOfType(AssetType type);
    void Clear();
}