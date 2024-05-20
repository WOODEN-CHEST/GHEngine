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
    // Static fields.
    public const float ONE_UNIT_SIZE = 0.003f;


    // Fields.
    public bool IsVisible { get; set; }

    public string Text
    {
        get => _text;
        set
        {
            _text = value ?? throw new ArgumentNullException(nameof(value));
            _cachedDrawSize = null;
            TextChange?.Invoke(this, new(this));
        }
    }

    public SpriteFont Font
    {
        get => _font;
        set
        {
            _font = value ?? throw new ArgumentNullException(nameof(value));
            _cachedDrawSize = null;
            FontChange?.Invoke(this, new(this));
        }
    }

    internal float FontSize
    {
        get => _fontSize;
        set
        {
            _fontSize = value;
            _cachedDrawSize = null;
            FontSizeChange?.Invoke(this, new(this));
        }
    }

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
                _cachedDrawSize = CalculateDrawSize(Text);
            }
            return _cachedDrawSize.Value;
        }
    }

    public event EventHandler<TextComponentArgs>? TextChange;
    public event EventHandler<TextComponentArgs>? FontChange;
    public event EventHandler<TextComponentArgs>? FontSizeChange;


    // Private fields.
    private SpriteFont _font;
    private GenericColorMask _colorMask;
    private string _text = string.Empty;
    private Vector2? _cachedDrawSize = null;
    private float _fontSize = 1f;


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


    // Methods.
    public Vector2 CalculateDrawSize(string text)
    {
        return new Vector2(_font.MeasureString(text).X / Font.LineSpacing * ONE_UNIT_SIZE * FontSize,
                    FontSize * ONE_UNIT_SIZE);
    }
}