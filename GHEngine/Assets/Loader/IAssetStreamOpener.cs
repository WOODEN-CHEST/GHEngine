using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHEngine.Assets.Def;

namespace GHEngine.Assets.Loader;

public interface IAssetStreamOpener
{
    // Methods.
    Stream GetStream(AssetPath path);
    void SetMemoryStream(string path, Stream stream);
    void RemoveMemoryStream(string path);
}