using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets;

public interface IAssetDefinitionCollection
{
    AssetDefinition? Get(AssetType type, string name);

    void Add(AssetDefinition definition);

    AssetDefinition[] GetAll();

    AssetDefinition[] GetAll(AssetType type);

    void Clear();
}