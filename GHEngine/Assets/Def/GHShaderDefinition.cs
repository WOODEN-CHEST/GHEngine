using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets.Def;

public class GHShaderDefinition : GHSinglePathAssetDefinition
{
    // Constructors.
    public GHShaderDefinition(string name, AssetPath path) : base(AssetType.Shader, name, path) { }
}