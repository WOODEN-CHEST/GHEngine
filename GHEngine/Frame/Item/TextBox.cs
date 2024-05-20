using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;


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
    public int Count => _components.Count;

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


    // Constructors.
    public TextBox()
    {

    }


    // Methods.
    public TextBox Append(TextComponent component)
    {
        SubscribeToComponent(component);
        _components.Add(component);
        return this;
    }

    public TextBox Remove(TextComponent component)
    {
        UnsubscribeFromComponent(component);
        _components.Remove(component);
        return this;
    }

    public TextBox Remove(int index)
    {
        UnsubscribeFromComponent(_components[index]);
        _components.RemoveAt(index);
        return this;
    }

    public TextBox Clear()
    {
        foreach (TextComponent Component in _components)
        {
            UnsubscribeFromComponent(Component);
        }
        _components.Clear();
        return this;
    }


    // Private methods.
    private Vector2 GetTotalLineSize(Vector2 totalLineSize, Vector2 lineSize)
    {
        return new Vector2(Math.Max(totalLineSize.X, lineSize.X), totalLineSize.Y + lineSize.Y);
    }

    private void UpdateDrawSize()
    {
        Vector2 TotalDrawSize = Vector2.Zero;
        Vector2 LineDrawSize = Vector2.Zero;

        foreach (TextComponent Component in _components)
        {
            string[] TextLines = Component.Text.Split('\n');

            for (int i = 0; i < TextLines.Length; i++)
            {
                if (i > 0)
                {
                    TotalDrawSize = GetTotalLineSize(TotalDrawSize, LineDrawSize);
                    LineDrawSize = Vector2.Zero;
                }

                Vector2 SplitLineSize = Component.CalculateDrawSize(TextLines[i]);
                LineDrawSize.X += SplitLineSize.X;
                LineDrawSize.Y += Math.Max(LineDrawSize.Y, SplitLineSize.Y);
            }
        }

        _cachedDrawSize = GetTotalLineSize(TotalDrawSize, LineDrawSize);
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

    private void SubscribeToComponent(TextComponent component)
    {
        component.TextChange += OnComponentTextChangeEvent;
        component.FontChange += OnComponentFontChangeEvent;
        component.FontSizeChange += OnComponentFontSizeChangeEvent;
    }

    private void UnsubscribeFromComponent(TextComponent component)
    {
        component.TextChange -= OnComponentTextChangeEvent;
        component.FontChange -= OnComponentFontChangeEvent;
        component.FontSizeChange += OnComponentFontSizeChangeEvent;
    }

    private void OnComponentTextChangeEvent(object? sender, TextComponentArgs args)
    {
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


    // Inherited methods.
    public void Render(IRenderer renderer, IProgramTime time)
    {
        Vector2 OriginCenter = DrawSize * Origin;


        foreach (TextComponent Component in _components)
        {

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
        throw new NotImplementedException();
    }


    // Operators.
    public TextComponent this[int index]
    {
        get => _components[index];
        set
        {
            UnsubscribeFromComponent(_components[index]);
            SubscribeToComponent(value);
            _components[index] = value;
        }
    }
}