using GHEngine.Assets;
using GHEngine.Frame.Item;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GHEngine.Frame;

public interface IGameFrame : IColorMaskable, IShadered
{
    // Properties.
    bool IsVisible { get; set; }
    public ILayer? TopLayer { get; }
    public ILayer? BottomLayer { get; }
    public int LayerCount { get; }
    public ILayer[] Layers { get; }
    Vector2 Position { get; set; }
    Vector2 Size { get; set; }
    Vector2 Origin { get; set; }
    float Rotation { get; set; }
    RectangleF? DrawBounds { get; set; }
    SamplerState? CustomSamplerState { get; set; }
    SpriteEffects Effects { get; set; }


    // Methods.
    public void AddLayer(ILayer layer);
    public void RemoveLayer(ILayer layer);
    public void RemoveLayer(string name);
    public void RemoveLayer(int index);
    public ILayer? GetLayer(int index);
    public ILayer? GetLayer(string name);
}