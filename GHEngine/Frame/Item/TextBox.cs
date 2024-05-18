using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;


namespace GHEngine.Frame.Item;


public class TextBox : IRenderableItem, IShadered, IColorMaskable, IEnumerable<TextComponent>
{
    // Fields.
    public Vector2 Position { get; set; }
    public float Rotation { get; set; }
    public Vector2 Origin { get; set; }
    public Vector2 Bounds { get; set; }

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

    public SpriteEffect? Shader { get; set; }
    public SpriteEffects Effects { get; set; }
    public bool IsVisible { get; set; }
    public TextComponent[] Components => _components.ToArray();
    public int Count => _components.Count;
    public Vector2 DrawSize => GetDrawSize();


    // Private fields.
    private GenericColorMask _colorMask;
    private readonly List<TextComponent> _components = new();


    // Constructors.
    public TextBox()
    {

    }


    // Methods.
    public TextBox Append(TextComponent component)
    {
        _components.Add(component);
        return this;
    }

    public TextBox Remove(TextComponent component)
    {
        _components.Remove(component);
        return this;
    }

    public TextBox Remove(int index)
    {
        _components.RemoveAt(index);
        return this;
    }

    public TextBox Clear()
    {
        _components.Clear();
        return this;
    }


    // Private methods.
    private Vector2 GetDrawSize()
    {
        throw new NotImplementedException();
    }



    // Inherited methods.
    public void Render(IRenderer renderer, IProgramTime time)
    {
        Vector2 DrawSize = GetDrawSize();
        Vector2 OriginCenter = DrawSize * Origin;
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
        set => _components[index] = value;
    }
}