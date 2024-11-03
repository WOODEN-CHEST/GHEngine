using GHEngine.Assets.Def;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine;

public class AssetLoadException : Exception
{
    public AssetLoadException(AssetDefinition definition, string message, Exception? innerException = null) 
        : base($"Failed to load asset of type \"{definition.Type.TypeName}\" and name \"{definition.Name}\". {message}", innerException)
    { }

    public AssetLoadException(string message, Exception? innerException = null) : base(message, innerException) { }
}