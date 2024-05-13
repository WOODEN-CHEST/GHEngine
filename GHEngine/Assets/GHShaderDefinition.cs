using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets;

public class GHShaderDefinition : AssetDefinition
{
    // Constructors.
    public GHShaderDefinition(string name) : base(AssetType.Shader, name) { }
}