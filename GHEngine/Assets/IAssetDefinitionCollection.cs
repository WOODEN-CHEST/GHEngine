using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets;

public interface IAssetDefinitionCollection
{
    void Add(AssetDefinition definition);

    AssetDefinition? Get(AssetType type, string name);

    AssetDefinition[] GetAll();

    AssetDefinition[] GetAll(AssetType type);

    void Clear();
}