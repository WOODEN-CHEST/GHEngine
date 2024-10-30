using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHEngine.Assets.Def;

namespace GHEngine.Assets;

public interface IAssetProvider
{
    public T? GetAsset<T>(object user, AssetType type, string name) where T : class;

    public void ReleaseAsset(object user, AssetType type, string name);

    public void ReleaseAsset(object user, object asset);

    public void ReleaseUserAssets(object user);

    public void ReleaseAllAssets();
}