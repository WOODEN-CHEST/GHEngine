using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Frame.Item;

public class TextComponent : IColorMaskable
{
    // Fields.
    public bool IsVisible { get; set; }

    public string Text
    {
        get => _text;
        set
        {
            _text = value ?? throw new ArgumentNullException(nameof(value));
            _cachedDrawSize = null;
        }
    }

    public SpriteFont Font
    {
        get => _font;
        set
        {
            _font = value ?? throw new ArgumentNullException(nameof(value));
            _cachedDrawSize = null;
        }
    }

    internal float FontSize { get; set; }

    public float Brightness
    {
        get => _colorMask.Brightness;
        set => _colorMask.Brightness = value;
    }

    public float Opacity
    {
        get => _colorMask.Opacity;
        set => _colorMask.Opacity = value;
    }

    public Color Mask
    {
        get => _colorMask.Mask;
        set => _colorMask.Mask = value;
    }

    public Vector2 DrawSize
    {
        get
        {
            if (_cachedDrawSize == null)
            {
                _cachedDrawSize = _font.MeasureString(Text);
            }
            return _cachedDrawSize.Value;
        }
    }


    // Private static fields.
    private const float ONE_UNIT_SIZE = 0.003f;


    // Private fields.
    private SpriteFont _font;
    private GenericColorMask _colorMask;
    private string _text = string.Empty;
    private Vector2? _cachedDrawSize = null;


    // Constructors.
    public TextComponent(SpriteFont font)
    {
        Font = font;
    }

    public TextComponent(SpriteFont font, string text)
    {
        Font = font;
        Text = text;
    }
}