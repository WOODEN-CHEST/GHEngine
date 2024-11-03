using GHEngine.Frame.Item;
using Microsoft.Xna.Framework.Graphics;

namespace GHEngine.Frame;

public interface ILayer : IColorMaskable, IShadered, IRenderableItem
{
    // Properties.
    string Name { get; }
    public int DrawableItemCount { get; }
    public IRenderableItem[] Items { get; }


    // Methods.
    public void AddItem(IRenderableItem item);
    public void AddItem(IRenderableItem item, float zIndex);
    public void RemoveItem(IRenderableItem item);
    public void ClearItems();
}