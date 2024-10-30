using GHEngine.Assets.Def;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine;

public class AssetLoadException : Exception
{
    public AssetLoadException(AssetDefinition definition, string message) 
        : base($"Failed to load asset of type \"{definition.Type.TypeName}\" and name \"{definition.Name}\". {message}")
    { }

    public AssetLoadException(string message) : base(message) { }
}