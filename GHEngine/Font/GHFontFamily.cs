using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;


namespace GHEngine.GameFont;

public class GHFontFamily : IDisposable
{
    // Fields.
    public string FamilyName { get; private init; }
    public string Name { get; private init; }
    public char[] SupportedCharacters { get; }
    public int LoadedFontCount => _fonts.Count;


    // Private fields.
    private readonly GraphicsDevice _graphicsDevice;
    private readonly FontFamily _family;

    private readonly FontTextureCollection _fontTextures = new();
    private readonly Dictionary<GHFontProperties, Font> _fonts = new();


    // Constructors.
    public GHFontFamily(GraphicsDevice graphicsDevice, FontFamily family)
    {
        _graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
        _family = family;
    }


    // Private methods.
    private Font GetFont(GHFontProperties properties)
    {
        if (_fonts.TryGetValue(properties, out var TargetFont))
        {
            return TargetFont;
        }
        FontStyle BoldStyle = properties.IsBold ? FontStyle.Bold : FontStyle.Regular;
        FontStyle ItalicStyle = properties.IsItalic ? FontStyle.Italic : FontStyle.Regular;

        Font CreatedFont = _family.CreateFont(properties.Size, BoldStyle | ItalicStyle);
        _fonts.Add(properties, CreatedFont);
        return CreatedFont;
    }

    private Texture2D LoadTexture(char character, GHFontProperties properties)
    {
        Font TargetFont = GetFont(properties);
        RichTextOptions TargetTextOptions = new(TargetFont);
        TargetTextOptions.Origin = new(0f, 0f);
        FontRectangle DrawSize = TextMeasurer.MeasureAdvance(character.ToString(), TargetTextOptions);
        using Image<Rgba32> FontImage = new((int)Math.Ceiling(DrawSize.Width), (int)Math.Ceiling(DrawSize.Height));
        FontImage.Mutate(context => context.DrawText(TargetTextOptions, character.ToString(), SixLabors.ImageSharp.Color.White));

        Texture2D MonoGameTexture = new(_graphicsDevice, FontImage.Width, FontImage.Height, false, SurfaceFormat.ColorSRgb);
        MonoGameTexture.SetData(FontImage.GetPixelMemoryGroup().SelectMany(memory => memory.ToArray())
            .Select(color => new Microsoft.Xna.Framework.Color(color.Rgba)).ToArray());
        return MonoGameTexture;
    }


    // Methods.
    public Vector2 MeasurePixelSize(string text, GHFontProperties properties)
    {
        ArgumentNullException.ThrowIfNull(properties, nameof(properties));
        ArgumentNullException.ThrowIfNull(text, nameof(text));

        Vector2 Size = Vector2.Zero;
        Vector2 CurrentLineSize = Vector2.Zero;
        for (int i = 0; i < text.Length; i++)
        {
            char Character = text[i];
            if (CurrentLineSize.X > 0f)
            {
                CurrentLineSize.X += properties.CharSpacing;
            }

            Texture2D CharTexture = GetCharTexture(Character, properties);
            CurrentLineSize.Y = Math.Max(CurrentLineSize.Y, CharTexture.Height);

            if (Character == '\n')
            {
                CurrentLineSize = Vector2.Zero;
                
                Size.Y += GetCharTexture(Character, properties).Height + properties.LineSpacing;
                continue;
            }
            else
            {
                CurrentLineSize.X += CharTexture.Width;
                Size.X = Math.Max(Size.X, CurrentLineSize.X);
            }
        }
        
        return Size;
    }

    public void ClearOldFonts(int fontsToRemain)
    {
        GHFontProperties[] CurrentFonts = _fontTextures.SupportedProperties.ToArray();
        for (int i = 0; i < _fontTextures.FontCount - fontsToRemain; i++)
        {
            GHFontProperties Properties = CurrentFonts[i];
            foreach (Texture2D Texture in _fontTextures.GetTexturesOfFont(Properties))
            {
                Texture.Dispose();
            }
            _fontTextures.ClearTextures(Properties);
            _fonts.Remove(Properties);
        }
    }

    public Texture2D GetCharTexture(char character, GHFontProperties properties)
    {
        Texture2D? Texture = _fontTextures.GetTexture(character, properties);
        if (Texture == null)
        {
            Texture = LoadTexture(character, properties);
            _fontTextures.SetTexture(Texture, character, properties);
        }

        return Texture;
    }

    public void LoadFontCharacters(GHFontProperties properties, IEnumerable<char> characters)
    {
        ArgumentNullException.ThrowIfNull(properties, nameof(properties));

        foreach (char Character in characters)
        {
            GetCharTexture(Character, properties);
        }
    }

    public void LoadFullFont(GHFontProperties properties)
    {
        for (char Character = (char)0; Character < char.MaxValue; Character++)
        {
            GetCharTexture(Character, properties);
        }
    }

    public void Dispose()
    {
        ClearOldFonts(0);
    }
}