using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NAudio.Mixer;
using System.Collections;
using System.ComponentModel;
using System.Dynamic;
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
    public bool IsSplittingAllowed
    {
        get => _isSplittingAllowed;
        set
        {
            if (value ==_isSplittingAllowed)
            {
                return;
            }
            _isSplittingAllowed = value;
            _drawLines = null;
        }
    }
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

    public Vector2 DrawSize
    {
        get
        {
            if (_cachedDrawSize == null)
            {
                if (_drawLines == null)
                {
                    UpdateDrawLines();
                }
                _cachedDrawSize = GetDrawSize(_drawLines!);
            }
            return _cachedDrawSize!.Value;
        }
    }

    public Vector2 MaxSize
    {
        get => _maxSize;
        set
        {
            _maxSize = value;
            _drawLines = null;
        }
    }

    public TextFitMethod FitMethod
    {
        get => _fitMethod;
        set
        {
            _fitMethod = value;
            _drawLines = null;
        }
    }

    public string Text
    {
        get
        {
            if (_cachedText == null)
            {
                CacheText();
            }
            return _cachedText!;
        }
    }


    // Protected fields.
    protected DrawLine[] DrawLines
    {
        get
        {
            if (_drawLines == null)
            {
                UpdateDrawLines();
            }
            return _drawLines!;
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
    private bool _isSplittingAllowed = true;

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


    // Protected methods.
    protected virtual void OnDrawLinesUpdate(List<DrawLine> drawLines) { }

    protected virtual void EnsureDrawLineSize(List<DrawLine> lines)
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

    protected virtual string[] SplitComponentIntoLines(TextComponent component, float firstLineOffset, float maxXSize)
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

            Vector2 CurrentDrawSize = component.CalculateDrawSize(CurrentLine.ToString());
            float AllowedXSize = Lines.Count == 0 ? maxXSize - firstLineOffset : maxXSize;
            if (IsSplittingAllowed && (CurrentDrawSize.X > AllowedXSize))
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

    protected virtual List<DrawLine> GetRawDrawLineList()
    {
        List<DrawLine> DrawLines = new();
        DrawLine CurrentLine = new();

        foreach (TextComponent Component in _components)
        {
            CurrentLine.UpdateDrawSize();
            string[] TextLines = SplitComponentIntoLines(Component, CurrentLine.DrawSize.X, MaxSize.X);

            for (int LineStringIndex = 0; LineStringIndex < TextLines.Length; LineStringIndex++)
            {
                if (LineStringIndex > 0)
                {
                    DrawLines.Add(CurrentLine);
                    CurrentLine.UpdateDrawSize();
                    CurrentLine = new();
                }

                if (TextLines[LineStringIndex].Length == 0)
                {
                    continue;
                }

                CurrentLine.Components.Add(new(Component) { Text = TextLines[LineStringIndex] });
            }
        }

        DrawLines.Add(CurrentLine);
        CurrentLine.UpdateDrawSize();
        return DrawLines;
    }

    protected Vector2 GetDrawSize(IEnumerable<DrawLine> lines)
    {
        Vector2 TotalDrawSize = Vector2.Zero;
        foreach (DrawLine Line in lines)
        {
            TotalDrawSize.X = Math.Max(TotalDrawSize.X, Line.DrawSize.X);
            TotalDrawSize.Y += Line.DrawSize.Y;
        }
        return TotalDrawSize;
    }

    protected Vector2 GetDrawPosition(IEnumerable<DrawLine> drawLines, DrawLine line, TextComponent component)
    {
        Vector2 DrawPosition = Position;
        foreach (DrawLine TargetLine in drawLines)
        {
            if (line != TargetLine)
            {
                DrawPosition.Y += TargetLine.DrawSize.Y;
                continue;
            }

            float UsedXSpace = 0f;
            foreach (TextComponent LineComponent in TargetLine.Components)
            {
                if (LineComponent != component)
                {
                    UsedXSpace += LineComponent.DrawSize.X;
                    continue;
                }

                DrawPosition.X = Position.X;
                DrawPosition.X += Alignment switch
                {
                    TextAlignOption.Left => 0f,
                    TextAlignOption.Center => ((DrawSize.X - LineComponent.DrawSize.X) * 0.5f) + UsedXSpace,
                    TextAlignOption.Right => DrawSize.X - LineComponent.DrawSize.X + UsedXSpace,
                    _ => throw new NotSupportedException($"Text alignment method \"{Alignment}\" not supported by {GetType().FullName}")
                };
                DrawPosition.Y += TargetLine.DrawSize.Y - LineComponent.DrawSize.Y;
                return DrawPosition;
            }
            return DrawPosition;
        }
        return Vector2.Zero;
    }


    // Private methods.
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

    private void ResizeDrawLines(List<DrawLine> lines)
    {
        Vector2 Size = GetDrawSize(lines);
        if (Size.Y > MaxSize.Y)
        {
            float DownsizeFactor = MaxSize.Y / Size.Y;
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
            if (line.DrawSize.X <= MaxSize.X)
            {
                continue;
            }

            float DownsizeFactor = MaxSize.X / line.DrawSize.X;
            foreach (TextComponent Component in line.Components)
            {
                Component.FontSize *= DownsizeFactor;
            }
            line.UpdateDrawSize();
        }
    }

    private void CutDrawLines(List<DrawLine> lines)
    {
        Vector2 Size = GetDrawSize(lines);
        if (Size.X <= MaxSize.X && Size.Y <= MaxSize.Y)
        {
            _boxCutArea = null;
            return;
        }

        _boxCutArea = new(0f, 0f, Math.Min(MaxSize.X / Size.X, 1f), Math.Min(MaxSize.Y / Size.Y, 1f));
    }

    private void UpdateDrawLines()
    {
        if (_components.Count == 0)
        {
            _drawLines = Array.Empty<DrawLine>();
            return;
        }

        List<DrawLine> NewDrawLines = GetRawDrawLineList();
        EnsureDrawLineSize(NewDrawLines);
        OnDrawLinesUpdate(NewDrawLines);
        _drawLines = NewDrawLines.ToArray();
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
        RectangleF? SelectedBounds = DrawBounds ?? _boxCutArea;
        if (SelectedBounds == null)
        {
            return null;
        }

        Vector2 RelativeBoundsMin = Position + (DrawSize
            * new Vector2(SelectedBounds.Value.X, SelectedBounds.Value.Y));
        Vector2 RelativeBoundsMax = RelativeBoundsMin + (DrawSize 
            * new Vector2(SelectedBounds.Value.Width, SelectedBounds.Value.Height));

        float X = Math.Clamp((RelativeBoundsMin.X - drawPosition.X) / component.DrawSize.X, 0f, 1f);
        float Y = Math.Clamp((RelativeBoundsMin.Y - drawPosition.Y) / component.DrawSize.Y, 0f, 1f);
        float Width = Math.Clamp((RelativeBoundsMax.X - drawPosition.X) / component.DrawSize.X - X, 0f, 1f - X);
        float Height = Math.Clamp((RelativeBoundsMax.Y - drawPosition.Y) / component.DrawSize.Y - Y, 0f, 1f - X);
        return new(X, Y, Width, Height);
    }

    // Inherited methods.
    public virtual void Render(IRenderer renderer, IProgramTime time)
    {
        if (_drawLines == null)
        {
            UpdateDrawLines();
        }
        Vector2 RelativeOriginCenter = DrawSize * Origin;

        float UsedYSpace = 0f;
        foreach (DrawLine Line in _drawLines!)
        {
            float UsedXSpace = 0f;
            foreach (TextComponent Component in Line.Components)
            {
                Vector2 ExpectedPosition = GetDrawPosition(_drawLines, Line, Component);

                Vector2 RelativeComponentOrigin = (RelativeOriginCenter + Position - ExpectedPosition) / Component.DrawSize;
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
                Position,
                GetComponentDrawBounds(Component, ExpectedPosition),
                ColorMask.CombinedMask,
                Rotation,
                RelativeComponentOrigin,
                GHMath.GetWindowAdjustedVector(Component.DrawSize, renderer.AspectRatio),
                SpriteEffects.None,
                Shader,
                Component.CustomSamplerState,
                Component.DrawSize / new Vector2(Component.FontSize));

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
        return Text;
    }


    // Operators.
    public TextComponent this[int index]
    {
        get => _components[index];
        set
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));
            OnComponentRemove(_components[index]);
            OnComponentAdd(value);
            _components[index] = value;
        }
    }


    // Types.
    protected class DrawLine
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