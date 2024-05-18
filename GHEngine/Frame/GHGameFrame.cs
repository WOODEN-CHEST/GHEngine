using GHEngine.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GHEngine.Frame;

public abstract class GHGameFrame : IGameFrame
{
    // Fields.
    public string Name { get; private init; }

    public ILayer? TopLayer => _layers.Count == 0 ? null : _layers[^1];

    public ILayer? BottomLayer => _layers.Count == 0 ? null : _layers[0];

    public int LayerCount => _layers.Count;

    public ILayer[] Layers => _layers.ToArray();

    public ITimeUpdatable[] Items => _items.ToArray();

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


    public event EventHandler<GameFrameLoadArgs>? FrameLoaded;


    // Private fields.
    private readonly List<ILayer> _layers = new();
    private GenericColorMask _colorMask;
    private readonly HashSet<ITimeUpdatable> _items = new();


    // Constructors.
    public GHGameFrame(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }



    // Protected methods.
    protected abstract void HandleLoad(IAssetProvider assetProvider);
    protected abstract void HandleStart();
    protected abstract void HandleEnd();
    protected abstract void HandleUnload(IAssetProvider assetProvider);
    protected abstract void HandleUpdate();

    protected void SwitchToFrame(IGameFrame frame, IAssetProvider assetProvider)
    {
        End();
        Task.Run(() =>
        {
            Unload(assetProvider);
            GC.Collect();
        });

        frame.Start();
    }


    // Inherited methods.
    public void AddItem(ITimeUpdatable item)
    {
        _items.Add(item);
    }

    public void RemoveItem(ITimeUpdatable item)
    {
        _items.Remove(item);
    }

    public void ClearItems()
    {
        _items.Clear();
    }


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



    public void Load(IAssetProvider assetProvider)
    {
        HandleLoad(assetProvider);
        FrameLoaded?.Invoke(this, new GameFrameLoadArgs(this));
    }

    public void Start()
    {
        HandleStart();
    }


    public void End()
    {
        HandleEnd();
        _items.Clear();
    }

    public void Unload(IAssetProvider assetProvider)
    {
        assetProvider.ReleaseUserAssets(this);
        HandleUnload(assetProvider);
    }

    public void Update(IProgramTime time)
    {
        foreach (ITimeUpdatable Item in _items)
        {
            Item.Update(time);
        }

        HandleUpdate();
    }
}
