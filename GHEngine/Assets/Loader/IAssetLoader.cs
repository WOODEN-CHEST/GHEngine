using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHEngine.Assets.Def;

namespace GHEngine.Assets.Loader;

public interface IAssetLoader
{
    // Methods.
    IDisposable Load(AssetDefinition definition);
}