using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets;

public interface IAssetLoader
{
    // Fields.
    string AssetPath { get; set; }


    // Methods.
    object Load(AssetDefinition definition);

    void Unload(object asset);
}