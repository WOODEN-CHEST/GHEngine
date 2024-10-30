using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets;

public class AssetUnloadException : Exception
{
    public AssetUnloadException(object asset, string message) 
        : base($"Failed to unload asset of type \"{asset.GetType().FullName}\". {message}") { }
}
