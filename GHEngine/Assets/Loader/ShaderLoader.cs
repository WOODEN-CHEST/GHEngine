using GHEngine.Assets.Def;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Assets.Loader;

public class ShaderLoader : IAssetLoader
{
    // Private fields.
    private readonly ContentManager _content;


    // Constructors.
    public ShaderLoader(ContentManager content)
    {
        _content = content ?? throw new ArgumentNullException(nameof(content));
    }


    // Inherited methods.
    public IDisposable Load(AssetDefinition definition)
    {
        if (definition is not GHShaderDefinition ShaderDefinition)
        {
            throw new AssetLoadException("Asset definition is not a font definition.");
        }
        if (ShaderDefinition.TargetPath.Type != AssetPathType.FileSystem)
        {
            throw new AssetLoadException($"Shader loader only supports file system content loading.");
        }

        try
        {
            return _content.Load<SpriteEffect>(ShaderDefinition.TargetPath.Path);
        }
        catch (Exception e)
        {
            throw new AssetLoadException($"Failed to load shader: {e}");
        }
    }
}