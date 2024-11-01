using GHEngine.Assets.Def;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets.Loader;

public class FontLoader : GHStreamAssetLoader
{
    // Constructors,
    public FontLoader(IAssetStreamOpener streamOpener) : base(streamOpener) { }


    // Inherited methods.
    public override object Load(AssetDefinition definition)
    {
        if (definition is not GHFontDefinition FontDefinition)
        {
            throw new AssetLoadException("Asset definition is not a font definition.");
        }

        throw new NotImplementedException();
    }
}