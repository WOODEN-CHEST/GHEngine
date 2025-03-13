using Microsoft.Xna.Framework.Graphics;
using SixLabors.Fonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.GameFont;

public class FontTextureCollection
{
    // Fields.
    public int FontCount => _fontsByAge.Count;
    public IEnumerable<GHFontProperties> SupportedProperties => _fontsByAge;


    // Private fields.
    private readonly Dictionary<GHFontProperties, Dictionary<char, GHCharacterTexture>> _textures = new();
    private readonly Queue<GHFontProperties> _fontsByAge = new();


    // Methods.
    public void SetTexture(GHCharacterTexture texture, char character, GHFontProperties properties)
    {
        Dictionary<char, GHCharacterTexture> TextureDict = GetTextureDictionary(properties);

        TextureDict[character] = texture;
    }

    public bool GetTexture(char character, GHFontProperties properties, out GHCharacterTexture? texture)
    {
        texture = null;
        if (!_textures.TryGetValue(properties, out var TextureDict))
        {
            return false;
        }
        if (TextureDict.TryGetValue(character, out texture))
        {
            return true;
        }
        return false;
    }

    public void ClearTexture(char character, GHFontProperties properties)
    {
        if (_textures.TryGetValue(properties, out var TextureDict))
        {
            TextureDict.Remove(character);
        }
    }

    public void ClearTextures(GHFontProperties properties)
    {
        if (!_textures.TryGetValue(properties, out var TextureDict))
        {
            return;
        }

        _textures.Remove(properties);
    }

    public IEnumerable<GHCharacterTexture> GetTexturesOfFont(GHFontProperties properties)
    {
        if (_textures.TryGetValue(properties, out var TextureDict))
        {
            return TextureDict.Values;
        }
        return Enumerable.Empty<GHCharacterTexture>();
    }


    // Private methods.
    private Dictionary<char, GHCharacterTexture> GetTextureDictionary(GHFontProperties properties)
    {
        _textures.TryGetValue(properties, out var TextureDict);
        if (TextureDict != null)
        {
            return TextureDict;
        }

        TextureDict = new();
        _textures[properties] = TextureDict;
        _fontsByAge.Enqueue(properties);
        return TextureDict;
    }
}