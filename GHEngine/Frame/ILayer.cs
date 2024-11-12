using GHEngine.Frame.Item;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GHEngine.Frame;

public interface ILayer : IRenderableItem, IColorMaskable, IShadered
{
    // Properties.
    string Name { get; }
    int DrawableItemCount { get; }
    IRenderableItem[] Items { get; }
    Vector2 Position { get; set; }
    Vector2 Size { get; set; }
    Vector2 Origin { get; set; }
    float Rotation { get; set; }
    RectangleF? DrawBounds { get; set; }
    SamplerState? CustomSamplerState { get; set; }
    SpriteEffects Effects { get; set; }


    // Methods.
    void AddItem(IRenderableItem item);
    void AddItem(IRenderableItem item, float zIndex);
    void RemoveItem(IRenderableItem item);
    void ClearItems();
}