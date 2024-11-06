using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System.Text;

namespace GHEngine.Frame.Item;


public class TextBox : IRenderableItem, IShadered, IColorMaskable, IEnumerable<TextComponent>
{
    // Fields.
    public SpriteEffect? Shader { get; set; }
    public SpriteEffects Effects { get; set; }
    public bool IsVisible { get; set; }
    public Vector2 Position { get; set; }
    public float Rotation { get; set; }
    public Vector2 Origin { get; set; }
    public Vector2 Bounds { get; set; }
    public TextAlignOption Alignment { get; set; }

    public int Length
    {
        get
        {
            if (_cachedText == null)
            {
                CacheText();
            }
            return _cachedText!.Length;
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

    public TextComponent[] Components => _components.ToArray();
    public int ComponentCount => _components.Count;

    public int MaxCharacters
    {
        get => _maxCharacters;
        set
        {
            _maxCharacters = value;
            EnsureCharacterCount();
        }
    }

    public bool AllowNewlines
    {
        get => _allowNewlines;
        set
        {
            _allowNewlines = value;
            if (!value)
            {
                EnsureNoNewlines();
            }
        }
    }

    public Vector2 DrawSize
    {
        get
        {
            if (_cachedDrawSize == null)
            {
                UpdateDrawSize();
            }
            return _cachedDrawSize!.Value;
        }
    }


    // Private fields.
    private GenericColorMask _colorMask;
    private readonly List<TextComponent> _components = new();

    private int _maxCharacters = int.MaxValue;
    private bool _allowNewlines = true;
    private Vector2? _cachedDrawSize = null;
    private List<DrawLine>? _drawLines = new();

    private string? _cachedText = null;


    // Constructors.
    public TextBox() { }


    // Methods.
    public TextBox Add(TextComponent component)
    {
        OnComponentAdd(component);
        _components.Add(component);
        return this;
    }

    public TextBox Insert(TextComponent component, int index)
    {
        _components.Insert(index, component);
        OnComponentAdd(component);
        return this;
    }

    public TextBox Remove(TextComponent component)
    {
        OnComponentRemove(component);
        _components.Remove(component);
        return this;
    }

    public TextBox RemoveAt(int index)
    {
        OnComponentRemove(_components[index]);
        _components.RemoveAt(index);
        return this;
    }

    public TextBox Clear()
    {
        foreach (TextComponent Component in _components)
        {
            OnComponentRemove(Component);
        }
        _components.Clear();
        return this;
    }


    // Private methods.
    private void UpdateDrawSize()
    {
        Vector2 TotalDrawSize = Vector2.Zero;
        UpdateDrawLines();
        foreach (DrawLine Line in _drawLines!)
        {
            TotalDrawSize.X = Math.Max(TotalDrawSize.X, Line.DrawSize.X);
            TotalDrawSize.Y += Line.DrawSize.Y;
        }
    }

    private void EnsureCharacterCount()
    {
        if (_maxCharacters == int.MaxValue)
        {
            return;
        }

        int Characters = 0;
        int ComponentIndex;
        for (ComponentIndex = 0; (ComponentIndex < _components.Count) && (Characters < _maxCharacters); ComponentIndex++)
        {
            TextComponent Component = _components[ComponentIndex];
            if (Characters + Component.Text.Length > _maxCharacters)
            {
                Component.Text = Component.Text.Substring(0, _maxCharacters - Characters);
                break;
            }
            Characters += Component.Text.Length;
        }
        if (ComponentIndex < _components.Count)
        {
            _components.RemoveRange(ComponentIndex, _components.Count - ComponentIndex);
        }
    }

    private void EnsureNoNewlines()
    {
        foreach (TextComponent Component in _components)
        {
            Component.Text = Component.Text.Replace("\n", "");
        }
    }

    private void CacheText()
    {
        StringBuilder CombinedString = new();

        foreach (TextComponent Component in _components)
        {
            CombinedString.Append(Component.Text);
        }

        _cachedText = CombinedString.ToString();
    }

    private string[] SplitComponentIntoLines(TextComponent component, float maxXSize)
    {
        List<string> Lines = new();
        StringBuilder CurrentLine = new();

        for (int i = 0; i < component.Text.Length; i++)
        {
            char Character = component.Text[i];

            if (Character == '\n')
            {
                Lines.Add(CurrentLine.ToString());
                CurrentLine.Clear();
                continue;
            }
            if (!char.IsWhiteSpace(Character))
            {
                CurrentLine.Append(Character);
                continue;
            }
            if (component.CalculateRelativeDrawSize(CurrentLine.ToString()).X > maxXSize)
            {
                Lines.Add(CurrentLine.ToString());
                CurrentLine.Clear();
            }
        }

        Lines.Add(CurrentLine.ToString());

        return Lines.ToArray();
    }

    private void UpdateDrawLines()
    {
        if (_drawLines != null)
        {
            return;
        }

        _drawLines = new() { new DrawLine() };

        foreach (TextComponent Component in _components)
        {
            string[] Lines = SplitComponentIntoLines(Component, Bounds.X);

            for (int i = 0; i < Lines.Length; i++)
            {
                if (i > 0)
                {
                    _drawLines.Add(new DrawLine());
                }

                _drawLines[^1].Components.Add(new TextComponent(Component.FontFamily, Lines[i])
                {
                    Mask = Component.Mask,
                    Brightness = Component.Brightness,
                    Opacity = Component.Opacity,
                    FontSize = Component.FontSize
                });
                if (Component.RelativeDrawSize.X > Bounds.X)
                {
                    Component.FontSize *= Bounds.X / Component.RelativeDrawSize.X;
                }
            }
        }

        foreach (DrawLine Line in _drawLines)
        {
            Line.UpdateDrawSize();
        }
    }

    private void OnComponentTextChangeEvent(object? sender, TextComponentArgs args)
    {
        _cachedText = null;
        _drawLines = null;
        if (!AllowNewlines)
        {
            EnsureNoNewlines();
        }
        EnsureCharacterCount();
    }

    private void OnComponentFontChangeEvent(object? sender, TextComponentArgs args)
    {
        _cachedDrawSize = null;
    }

    private void OnComponentFontSizeChangeEvent(object? sender, TextComponentArgs args)
    {
        _cachedDrawSize = null;
    }

    private void OnComponentAdd(TextComponent component)
    {
        component.TextChange += OnComponentTextChangeEvent;
        component.FontChange += OnComponentFontChangeEvent;
        component.FontSizeChange += OnComponentFontSizeChangeEvent;

        _cachedText = null;
        _cachedDrawSize = null;
        _drawLines = null;
    }

    private void OnComponentRemove(TextComponent component)
    {
        component.TextChange -= OnComponentTextChangeEvent;
        component.FontChange -= OnComponentFontChangeEvent;
        component.FontSizeChange += OnComponentFontSizeChangeEvent;

        _cachedText = null;
        _cachedDrawSize = null;
        _drawLines = null;
    }

    // Inherited methods.
    public void Render(IRenderer renderer, IProgramTime time)
    {
        Vector2 OriginCenter = DrawSize * Origin;
        UpdateDrawLines();

        float UsedYSpace = 0f;
        foreach (DrawLine Line in _drawLines!)
        {
            float UsedXSpace = 0f;
            foreach (TextComponent Component in Line.Components)
            {
                Vector2 DrawPosition = Vector2.Zero;
                DrawPosition.X = Alignment switch
                {
                    TextAlignOption.Left => (Position.X - OriginCenter.X) + UsedXSpace,
                    TextAlignOption.Center => (Position.X - OriginCenter.X) + ((DrawSize.X - Line.DrawSize.X) * 0.5f) + UsedXSpace,
                    TextAlignOption.Right => (Position.X - OriginCenter.X) + DrawSize.X - Line.DrawSize.X + UsedXSpace,
                    _ => throw new EnumValueException(nameof(Alignment), Alignment)
                };
                DrawPosition.Y = Position.Y - OriginCenter.Y + (Line.DrawSize.Y - Component.RelativeDrawSize.Y) + UsedYSpace;

                Vector2 RelativeRotationOrigin = (Position - OriginCenter) - DrawPosition;
                GenericColorMask ColorMask = new()
                {
                    Mask = Component.Mask,
                    Brightness = Component.Brightness,
                    Opacity = Component.Opacity
                };

                renderer.DrawString(
                    new(Component.FontFamily, Component.FontSize, Component.IsBold, 
                        Component.IsItalic, Component.LineSpacing, Component.CharSpacing), 
                    Component.Text,
                    DrawPosition,
                    ColorMask.CombinedMask,
                    Rotation,
                    RelativeRotationOrigin,
                    Vector2.One, 
                    Effects,
                    Shader,
                    null,
                    Component.RelativeDrawSize);

                UsedXSpace += Component.RelativeDrawSize.X;
            }
            UsedYSpace += Line.DrawSize.Y;
        }
    }

    public IEnumerator<TextComponent> GetEnumerator()
    {
        foreach (TextComponent Component in _components)
        {
            yield return Component;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override string ToString()
    {
        if (_cachedText == null)
        {
            CacheText();
        }
        return _cachedText!;
    }


    // Operators.
    public TextComponent this[int index]
    {
        get => _components[index];
        set
        {
            OnComponentRemove(_components[index]);
            OnComponentAdd(value);
            _components[index] = value;
        }
    }


    // Types.
    private class DrawLine
    {
        // Fields.
        public List<TextComponent> Components { get; private set; } = new();
        public Vector2 DrawSize { get; private set; }


        // Methods.
        public void UpdateDrawSize()
        {
            Vector2 LineSize = Vector2.Zero;

            foreach (TextComponent Component in Components)
            {
                Vector2 ComponentSize = Component.RelativeDrawSize;
                LineSize.X += ComponentSize.X;
                LineSize.Y = Math.Max(LineSize.Y, ComponentSize.Y);
            }

            DrawSize = LineSize;
        }
    }
}