using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets;

public class ShaderDefinition : AssetDefinition
{
    // Constructors.
    public ShaderDefinition(string name) : base(AssetType.Shader, name) { }
}