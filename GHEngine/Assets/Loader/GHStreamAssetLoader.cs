using GHEngine.Assets.Def;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets.Loader;

public abstract class GHStreamAssetLoader : IAssetLoader
{
    // Fields.
    public IAssetStreamOpener StreamOpener { get; private init; }


    // Constructors.
    public GHStreamAssetLoader(IAssetStreamOpener streamOpener)
    {
        StreamOpener = streamOpener ?? throw new ArgumentNullException(nameof(streamOpener));
    }

    public abstract IDisposable Load(AssetDefinition definition);
}