using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets;

public class GHShaderDefinition : GHSinglePathAssetDefinition
{
    // Constructors.
    public GHShaderDefinition(string name, string path) : base(AssetType.Shader, name, path) { }
}