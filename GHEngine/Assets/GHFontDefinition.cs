using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets;

public class GHFontDefinition : GHSinglePathAssetDefinition
{
    // Constructors.
    public GHFontDefinition(string name, string path) : base(AssetType.Font, name, path) { }
}