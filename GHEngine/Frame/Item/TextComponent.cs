using GHEngine.GameFont;
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
            TextChange?.Invoke(this, new(this));
        }
    }

    public GHFontFamily FontFamily
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

    public Vector2 RelativeDrawSize
    {
        get
        {
            if (_cachedDrawSize == null)
            {
                _cachedDrawSize = CalculateRelativeDrawSize(Text);
            }
            return _cachedDrawSize.Value;
        }
    }

    public bool IsBold
    {
        get => _isBold;
        set
        {
            if (_isBold != value)
            {
                _isBold = value;
                FontChange?.Invoke(this, new(this));
            }
        }
    }


    public bool IsItalic
    {
        get => _isItalic;
        set
        {
            if (_isItalic != value)
            {
                _isItalic = value;
                FontChange?.Invoke(this, new(this));
            }
        }
    }

    public float LineSpacing
    {
        get => _lineSpacing;
        set
        {
            if (_lineSpacing != value)
            {
                _lineSpacing = value;
                FontChange?.Invoke(this, new(this));
            }
        }
    }

    public float CharSpacing
    {
        get => _charSpacing;
        set
        {
            if (_charSpacing != value)
            {
                _charSpacing = value;
                FontChange?.Invoke(this, new(this));
            }
        }
    }

    public event EventHandler<TextComponentArgs>? TextChange;
    public event EventHandler<TextComponentArgs>? FontChange;
    public event EventHandler<TextComponentArgs>? FontSizeChange;


    // Private fields.
    private GHFontFamily _font;
    private GenericColorMask _colorMask;
    private string _text = string.Empty;
    private Vector2? _cachedDrawSize = null;
    private float _fontSize = 1f;

    private bool _isBold = false;
    private bool _isItalic = false;
    private float _lineSpacing = 0f;
    private float _charSpacing = 0f;


    // Constructors.
    public TextComponent(GHFontFamily font)
    {
        FontFamily = font;
    }

    public TextComponent(GHFontFamily font, string text)
    {
        FontFamily = font;
        Text = text;
    }


    // Methods.
    public Vector2 CalculateRelativeDrawSize(string text)
    {
        GHFontProperties Properties = new(1f, IsBold, IsItalic, LineSpacing, CharSpacing);
        return FontFamily.MeasureRelativeSize(Text, Properties);
    }
}