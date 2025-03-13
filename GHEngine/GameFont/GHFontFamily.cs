using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Runtime.CompilerServices;


namespace GHEngine.GameFont;

public class GHFontFamily : IDisposable
{
    // Static fields.
    public const char REFERENCE_CHAR = 'A';
    public const float DEFAULT_SIZE = 1f;


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

    private bool ContainsNonTransparentPixelInColumn(Image<Rgba32> image, int column)
    {
        for (int Row = 0; Row < image.Height; Row++)
        {
            if (image[column, Row].A > 0)
            {
                return true;
            }
        }
        return false;
    }

    private bool ContainsNonTransparentPixelInRow(Image<Rgba32> image, int row)
    {
        for (int Column = 0; Column < image.Width; Column++)
        {
            if (image[Column, row].A > 0)
            {
                return true;
            }
        }
        return false;
    }


    private (float OffsetX, float OffsetY) GetImageOffsets(Image<Rgba32> image, IntVector origin)
    {
        int OffsetX = origin.X;
        int OffsetY = -origin.Y;

        for (int Column = 0; Column < image.Width; Column++)
        {
            if (ContainsNonTransparentPixelInColumn(image, Column))
            {
                break;
            }
            else
            {
                OffsetX--;
            }
        }

        for (int Row = 0; Row < image.Height; Row++)
        {
            if (ContainsNonTransparentPixelInRow(image, Row))
            {
                break;
            }
            else
            {
                OffsetY++;
            }
        }

        return (OffsetX, OffsetY);
    }

    private GHCharacterTexture LoadTexture(char character, GHFontProperties properties)
    {
        Font TargetFont = GetFont(properties);
        RichTextOptions TargetTextOptions = new(TargetFont);
        FontRectangle DrawBounds = TextMeasurer.MeasureSize(character.ToString(), TargetTextOptions);
        FontRectangle AdvanceBounds = TextMeasurer.MeasureAdvance(character.ToString(), TargetTextOptions);

        if ((DrawBounds.Width <= 0f) || (DrawBounds.Height <= 0f))
        {
            return new GHCharacterTexture(null, 0f, 0f, AdvanceBounds.Width / AdvanceBounds.Height);
        }

        /* Garbage-ass algorithm used to generate image textures, but it works for MOST cases. */
        using Image<Rgba32> FontImage = new(
            (int)Math.Ceiling(DrawBounds.Width * 4f),
            (int)Math.Ceiling(DrawBounds.Height * 4f),
            new Rgba32(0u));

        TargetTextOptions.Origin = new System.Numerics.Vector2(FontImage.Width / 4, FontImage.Height / 4);

        FontImage.Mutate(context =>
        {
            context.DrawText(TargetTextOptions, character.ToString(), SixLabors.ImageSharp.Color.White);
        });

        IntVector Origin = new((int)TargetTextOptions.Origin.X, (int)TargetTextOptions.Origin.Y);
        (float OffsetX, float OffsetY) = GetImageOffsets(FontImage, Origin);
        FontImage.Mutate(context => context.EntropyCrop());

        return new(TextureUtilities.ConvertTexture(FontImage, _graphicsDevice),
            OffsetY,
            OffsetX,
            AdvanceBounds.Width / AdvanceBounds.Height);
    }

    // Methods.
    public Vector2 MeasureAbsoluteSize(string text, GHFontProperties properties)
    {
        return MeasureRelativeSize(text, properties) * properties.Size;
    }

    public Vector2 MeasureRelativeSize(string text, GHFontProperties properties)
    {
        Vector2 RelativeSize = Vector2.Zero;
        Vector2 CurrentLineRelativeSize = Vector2.Zero;
        TextOptions MeasureOptions = new(GetFont(properties));

        for (int i = 0; i < text.Length; i++)
        {
            char Character = text[i];
            CurrentLineRelativeSize.Y = 1f;

            if (Character == '\n')
            {
                CurrentLineRelativeSize = new(0f, 1f);

                RelativeSize.Y += 1f + properties.LineSpacing;
                continue;
            }
            else
            {
                if (CurrentLineRelativeSize.X > 0f)
                {
                    CurrentLineRelativeSize.X += properties.CharSpacing;
                }
                CurrentLineRelativeSize.X += TextMeasurer.MeasureAdvance(Character.ToString(), MeasureOptions).Width;
                RelativeSize.X = Math.Max(RelativeSize.X, CurrentLineRelativeSize.X);
            }
        }

        RelativeSize.Y += CurrentLineRelativeSize.Y;
        return RelativeSize;
    }

    public void ClearOldFonts(int fontsToRemain)
    {
        GHFontProperties[] CurrentFonts = _fontTextures.SupportedProperties.ToArray();
        for (int i = 0; i < _fontTextures.FontCount - fontsToRemain; i++)
        {
            GHFontProperties Properties = CurrentFonts[i];
            foreach (GHCharacterTexture CharTexture in _fontTextures.GetTexturesOfFont(Properties))
            {
                CharTexture.Texture?.Dispose();
            }
            _fontTextures.ClearTextures(Properties);
            _fonts.Remove(Properties);
        }
    }

    public GHCharacterTexture GetCharTexture(char character, GHFontProperties properties)
    {
        if (!_fontTextures.GetTexture(character, properties, out GHCharacterTexture? Texture))
        {
            Texture = LoadTexture(character, properties);
            _fontTextures.SetTexture(Texture, character, properties);
        }

        return Texture!;
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