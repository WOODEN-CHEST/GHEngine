using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NAudio.Mixer;
using System.Collections;
using System.Text;

namespace GHEngine.Frame.Item;


public class TextBox : IRenderableItem, IShadered, IColorMaskable, IEnumerable<TextComponent>
{
    // Fields.
    public SpriteEffect? Shader { get; set; } = null;
    public bool IsVisible { get; set; } = true;
    public Vector2 Position { get; set; } = Vector2.Zero;
    public float Rotation { get; set; } = 0f;
    public Vector2 Origin { get; set; } = Vector2.Zero;
    public TextAlignOption Alignment { get; set; } = TextAlignOption.Left;
    public RectangleF? DrawBounds { get; set; } = null;
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

    public IEnumerable<TextComponent> Components => _components;
    public int ComponentCount => _components.Count;

    public int MaxCharacters
    {
        get => _maxCharacters;
        set
        {
            _maxCharacters = Math.Max(0, value);
            EnsureCharacterCount();
        }
    }

    public bool IsNewlineAllowed
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

    public Vector2 RelativeDrawSize
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

    public Vector2 MaxBoxSize
    {
        get => _maxSize;
        set
        {
            _maxSize = value;
            UpdateDrawLines();
        }
    }

    public TextFitMethod FitMethod
    {
        get => _fitMethod;
        set
        {
            _fitMethod = value;
            UpdateDrawLines();
        }
    }


    // Private fields.
    private GenericColorMask _colorMask = new();
    private readonly List<TextComponent> _components = new();

    private int _maxCharacters = int.MaxValue;
    private bool _allowNewlines = true;
    private Vector2? _cachedDrawSize = null;
    private DrawLine[]? _drawLines = null;
    private Vector2 _maxSize = new Vector2(float.PositiveInfinity);
    private TextFitMethod _fitMethod = TextFitMethod.Resize;
    private RectangleF? _boxCutArea = null;

    private string? _cachedText = null;


    // Constructors.
    public TextBox() { }

    public TextBox(IEnumerable<TextComponent> components)
    {
        ArgumentNullException.ThrowIfNull(components, nameof(components));
        foreach (TextComponent Component in components)
        {
            Add(Component);
        }
    }


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
        if (_drawLines == null)
        {
            UpdateDrawLines();
        }    
        foreach (DrawLine Line in _drawLines!)
        {
            TotalDrawSize.X = Math.Max(TotalDrawSize.X, Line.DrawSize.X);
            TotalDrawSize.Y += Line.DrawSize.Y;
        }
        _cachedDrawSize = TotalDrawSize;
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
            if (component.CalculateDrawSize(CurrentLine.ToString()).X > maxXSize)
            {
                Lines.Add(CurrentLine.ToString());
                CurrentLine.Clear();
            }
            else
            {
                CurrentLine.Append(Character);
            }
        }

        Lines.Add(CurrentLine.ToString());

        return Lines.ToArray();
    }

    private DrawLine[] GetDrawLines()
    {
        List<DrawLine> DrawLines = new();
        Vector2 CurrentSize = Vector2.Zero;
        DrawLine CurrentLine = new();

        foreach (TextComponent Component in _components)
        {
            string[] TextLines = SplitComponentIntoLines(Component, MaxBoxSize.X);

            for (int LineStringIndex = 0; LineStringIndex < TextLines.Length; LineStringIndex++)
            {
                if (LineStringIndex > 0)
                {
                    DrawLines.Add(CurrentLine);
                    CurrentLine = new();
                }

                TextComponent LineComponent = new(Component) { Text = TextLines[LineStringIndex] };
                CurrentLine.UpdateDrawSize();
                Vector2 ExpectedSize = new(CurrentLine.DrawSize.X + LineComponent.DrawSize.X,
                    Math.Max(CurrentLine.DrawSize.Y, LineComponent.DrawSize.Y));

                if ((MaxBoxSize.X >= ExpectedSize.X) && (MaxBoxSize.Y >= ExpectedSize.Y))
                {
                    continue;
                }
                if (FitMethod == TextFitMethod.Cut)
                {
                    return DrawLines.ToArray();
                }
                else if (FitMethod == TextFitMethod.Resize)
                {
                    
                }
            }
        }

        DrawLines.Add(CurrentLine);
        return DrawLines.ToArray();
    }

    private List<DrawLine> GetRawDrawLineList()
    {
        List<DrawLine> DrawLines = new();
        Vector2 CurrentSize = Vector2.Zero;
        DrawLine CurrentLine = new();

        foreach (TextComponent Component in _components)
        {
            string[] TextLines = SplitComponentIntoLines(Component, MaxBoxSize.X);

            for (int LineStringIndex = 0; LineStringIndex < TextLines.Length; LineStringIndex++)
            {
                if (LineStringIndex > 0)
                {
                    DrawLines.Add(CurrentLine);
                    CurrentLine.UpdateDrawSize();
                    CurrentLine = new();
                }

               CurrentLine.Components.Add(new(Component) { Text = TextLines[LineStringIndex] });
            }
        }

        DrawLines.Add(CurrentLine);
        CurrentLine.UpdateDrawSize();
        return DrawLines;
    }

    private void ResizeDrawLines(List<DrawLine> lines)
    {
        float VerticalSize = lines.Select(line => line.DrawSize.Y).Sum();
        if (VerticalSize > MaxBoxSize.Y)
        {
            float DownsizeFactor = MaxBoxSize.Y / VerticalSize;
            foreach (DrawLine line in lines)
            {
                foreach (TextComponent Component in line.Components)
                {
                    Component.FontSize *= DownsizeFactor;
                }
                line.UpdateDrawSize();
            }
        }

        foreach (DrawLine line in lines)
        {
            if (line.DrawSize.X <= MaxBoxSize.X)
            {
                continue;
            }

            float DownsizeFactor = MaxBoxSize.X / line.DrawSize.X;
            foreach (TextComponent Component in line.Components)
            {
                Component.FontSize *= DownsizeFactor;
            }
            line.UpdateDrawSize();
        }
    }

    private void CutDrawLines(List<DrawLine> lines)
    {

    }

    private void EnsureDrawLineSize(List<DrawLine> lines)
    {
        _boxCutArea = null;

        switch (FitMethod)
        {
            case TextFitMethod.Resize:
                ResizeDrawLines(lines);
                break;

            case TextFitMethod.Cut:
                CutDrawLines(lines);
                break;
        }
    }

    private void UpdateDrawLines()
    {
        if (_components.Count == 0)
        {
            _drawLines = Array.Empty<DrawLine>();
            return;
        }

        List<DrawLine> DrawLines = GetRawDrawLineList();
        EnsureDrawLineSize(DrawLines);
        _drawLines = DrawLines.ToArray();
    }

    private void OnComponentTextChangeEvent(object? sender, TextComponentArgs args)
    {
        _cachedText = null;
        _drawLines = null;
        if (!IsNewlineAllowed)
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
    
    private RectangleF? GetComponentDrawBounds(TextComponent component, Vector2 drawPosition)
    {
        if (DrawBounds == null)
        {
            return _boxCutArea;
        }

        return null;
    }

    // Inherited methods.
    public void Render(IRenderer renderer, IProgramTime time)
    {
        if (_drawLines == null)
        {
            UpdateDrawLines();
        }
        Vector2 RelativeOriginCenter = RelativeDrawSize * Origin;

        float UsedYSpace = 0f;
        foreach (DrawLine Line in _drawLines!)
        {
            float UsedXSpace = 0f;
            foreach (TextComponent Component in Line.Components)
            {
                Vector2 DrawPosition = Vector2.Zero;
                DrawPosition.X = Position.X;
                DrawPosition.X += Alignment switch
                {
                    TextAlignOption.Left => UsedXSpace,
                    TextAlignOption.Center => ((RelativeDrawSize.X - Line.DrawSize.X) * 0.5f) + UsedXSpace,
                    TextAlignOption.Right => RelativeDrawSize.X - Line.DrawSize.X + UsedXSpace,
                    _ => throw new NotSupportedException($"Drawing method \"{Alignment}\" not supported by {GetType().FullName}")
                };
                DrawPosition.Y = Position.Y + (Line.DrawSize.Y - Component.DrawSize.Y) + UsedYSpace;

                Vector2 RelativeComponentOrigin = RelativeOriginCenter - DrawPosition;
                GenericColorMask ColorMask = new()
                {
                    Mask = Component.Mask,
                    Brightness = Component.Brightness,
                    Opacity = Component.Opacity
                };

                FontRenderProperties Properties = new(Component.FontFamily, Component.IsBold,
                    Component.IsItalic, Component.LineSpacing, Component.CharSpacing);
                    renderer.DrawString(Properties,
                    Component.Text,
                    DrawPosition,
                    GetComponentDrawBounds(Component, DrawPosition),
                    ColorMask.CombinedMask,
                    Rotation,
                    Origin,
                    Component.DrawSize,
                    SpriteEffects.None,
                    Shader,
                    Component.CustomSamplerState,
                    Component.DrawSize / GHMath.GetWindowAdjustedVector(new Vector2(Component.FontSize), renderer.AspectRatio));

                UsedXSpace += Component.DrawSize.X;
            }
            UsedYSpace += Line.DrawSize.Y;
        }
    }

    public IEnumerator<TextComponent> GetEnumerator()
    {
        return _components.GetEnumerator();
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
        public List<TextComponent> Components { get; private init; } = new();
        public Vector2 DrawSize { get; private set; }


        // Methods.
        public void UpdateDrawSize()
        {
            Vector2 LineSize = Vector2.Zero;

            foreach (TextComponent Component in Components)
            {
                Vector2 ComponentSize = Component.DrawSize;
                LineSize.X += ComponentSize.X;
                LineSize.Y = Math.Max(LineSize.Y, ComponentSize.Y);
            }

            DrawSize = LineSize;
        }
    }
}