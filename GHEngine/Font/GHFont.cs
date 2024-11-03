using Microsoft.Xna.Framework.Graphics;
using SixLabors.Fonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Font;

public class GHFont
{
    // Fields.
    public string FamilyName { get; private init; }
    public string Name { get; private init; }
    public char[] SupportedCharacters { get; }


    // Private fields.
    private readonly GraphicsDevice _graphicsDevice;
    private readonly FontFamily _family;

    private readonly FontTextureCollection _fontTextures = new();

    // Constructors.
    public GHFont(GraphicsDevice graphicsDevice, FontFamily family)
    {
        _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
        _family = family;
    }


    // Methods.
    public void ClearOldFonts(int fontsToRemain)
    {

    }

    public Texture2D? GetCharTexture(char character, GHFontProperties properties)
    {
        return null;
    }
}