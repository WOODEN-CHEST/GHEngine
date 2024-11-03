using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Font;

public class FontTextureCollection
{
    // Private fields.
    private readonly Dictionary<GHFontProperties, Dictionary<char, Texture2D>> _textures = new();


    // Methods.
    public void SetTexture(Texture2D texture, char character, GHFontProperties properties)
    {
        Dictionary<char, Texture2D> TextureDict = GetTextureDictionary(properties);

        TextureDict[character] = texture;
    }


    // Private methods.
    private Dictionary<char, Texture2D> GetTextureDictionary(GHFontProperties properties)
    {
        _textures.TryGetValue(properties, out Dictionary<char, Texture2D>? TextureDict);
        if (TextureDict != null)
        {
            return TextureDict;
        }

        TextureDict = new();
        _textures[properties] = TextureDict;
        return TextureDict;
    }
}