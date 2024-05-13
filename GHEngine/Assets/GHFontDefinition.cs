using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets;

public class GHFontDefinition : AssetDefinition
{
    // Constructors.
    public GHFontDefinition(string name) : base(AssetType.Font, name) { }
}