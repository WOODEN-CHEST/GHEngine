using GHEngine.Assets;
using GHEngine.Frame.Item;

namespace GHEngine.Frame;

public interface IGameFrame : IColorMaskable, IShadered, ITimeUpdatable
{
    // Properties.
    public string Name { get; }
    public ILayer? TopLayer { get; }
    public ILayer? BottomLayer { get; }
    public int LayerCount { get; }
    public ILayer[] Layers { get; }
    public ITimeUpdatable[] Items { get; }

    public event EventHandler<GameFrameLoadArgs>? FrameLoaded;


    // Methods.
    public void AddLayer(ILayer layer);

    public void RemoveLayer(ILayer layer);

    public void RemoveLayer(string name);

    public void RemoveLayer(int index);

    public ILayer? GetLayer(int index);

    public ILayer? GetLayer(string name);


    public void AddItem(ITimeUpdatable item);

    public void RemoveItem(ITimeUpdatable item);

    public void ClearItems();


    public void Load(IAssetProvider assetProvider);

    public void Start();

    public void End();

    public void Unload(IAssetProvider assetProvider);
}