using GHEngine.Assets;
using GHEngine.Frame.Item;

namespace GHEngine.Frame;

public interface IGameFrame : IColorMaskable, IShadered
{
    // Properties.
    public ILayer? TopLayer { get; }
    public ILayer? BottomLayer { get; }
    public int LayerCount { get; }
    public ILayer[] Layers { get; }


    // Methods.
    public void AddLayer(ILayer layer);
    public void RemoveLayer(ILayer layer);
    public void RemoveLayer(string name);
    public void RemoveLayer(int index);
    public ILayer? GetLayer(int index);
    public ILayer? GetLayer(string name);
}