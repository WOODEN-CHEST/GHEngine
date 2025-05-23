using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHEngine.Assets.Def;

namespace GHEngine.Assets;

public interface IAssetProvider
{
    T? GetAsset<T>(object user, AssetType type, string name) where T : class;

    void ReleaseAsset(object user, AssetType type, string name);

    void ReleaseAsset(object user, object asset);

    void ReleaseUserAssets(object user);

    void ReleaseAllAssets();
}