using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets.Def;

public class AssetDefinitionReadException : Exception
{
    public AssetDefinitionReadException(string message) : base(message) { }
}
