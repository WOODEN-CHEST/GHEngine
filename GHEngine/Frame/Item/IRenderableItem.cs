using Microsoft.Xna.Framework.Graphics;

namespace GHEngine.Frame.Item;

public interface IRenderableItem
{
    // Fields.
    public bool IsVisible { get; set; }


    // Methods.
    public void Render(IRenderer renderer, IProgramTime time);
}