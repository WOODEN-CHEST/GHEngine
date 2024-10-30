using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets.Def;

public class GHFontDefinition : GHSinglePathAssetDefinition
{
    // Constructors.
    public GHFontDefinition(string name, AssetPath path) : base(AssetType.Font, name, path) { }
}