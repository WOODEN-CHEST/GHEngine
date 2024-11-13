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
    public bool IsVisible { get; set; } = true;

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

    public float FontSize
    {
        get => _fontSize;
        set
        {
            _fontSize = float.IsNaN(value) ? 0f : Math.Max(0f, value);
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
                _lineSpacing = float.IsNaN(value) ? 0f : value;
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
                _charSpacing = float.IsNaN(value) ? 0f : value;
                FontChange?.Invoke(this, new(this));
            }
        }
    }

    public SamplerState? CustomSamplerState { get; set; } = null;

    public event EventHandler<TextComponentArgs>? TextChange;
    public event EventHandler<TextComponentArgs>? FontChange;
    public event EventHandler<TextComponentArgs>? FontSizeChange;


    // Private fields.
    private GHFontFamily _font;
    private GenericColorMask _colorMask = new();
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

    public TextComponent(TextComponent component)
    {
        ArgumentNullException.ThrowIfNull(component, nameof(component));

        FontFamily = component.FontFamily;
        FontSize = component.FontSize;
        Text = component.Text;
        Mask = component.Mask;
        Brightness = component.Brightness;
        Opacity = component.Opacity;
        LineSpacing = component.LineSpacing;
        CharSpacing = component.CharSpacing;
        IsBold = component.IsBold;
        IsItalic = component.IsItalic;
        IsVisible = component.IsVisible;
    }


    // Methods.
    public Vector2 CalculateDrawSize(string text)
    {
        GHFontProperties Properties = new(1f, IsBold, IsItalic, LineSpacing, CharSpacing);
        return FontFamily.MeasureRelativeSize(Text, Properties) * FontSize;
    }
}