using GHEngine.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GHEngine.Frame;

public class GHGameFrame : IGameFrame
{
    // Fields.
    public ILayer? TopLayer => _layers.Count == 0 ? null : _layers[^1];
    public ILayer? BottomLayer => _layers.Count == 0 ? null : _layers[0];
    public int LayerCount => _layers.Count;
    public ILayer[] Layers => _layers.ToArray();

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


    // Private fields.
    private readonly List<ILayer> _layers = new();
    private GenericColorMask _colorMask = new();


    // Constructors.
    public GHGameFrame() { }


    // Inherited methods.
    public void AddLayer(ILayer layer)
    {
        _layers.Add(layer);
    }

    public ILayer? GetLayer(int index)
    {
        return _layers[index];
    }

    public ILayer? GetLayer(string name)
    {
        foreach (ILayer Layer in _layers)
        {
            if (Layer.Name == name)
            {
                return Layer;
            }
        }
        return null;
    }

    public void RemoveLayer(ILayer layer)
    {
        _layers.Remove(layer);
    }

    public void RemoveLayer(string name)
    {
        foreach (ILayer Layer in _layers)
        {
            if (Layer.Name == name)
            {
                _layers.Remove(Layer);
                return;
            }
        }
    }

    public void RemoveLayer(int index)
    {
        _layers.RemoveAt(index);
    }
}
