using Microsoft.Xna.Framework.Graphics;

namespace GHEngine.Frame.Item;

public interface IDrawableItem
{
    // Fields.
    public const float DEFAULT_LAYER_DEPTH = 0f;


    // Properties.
    public bool IsVisible { get; set; }

    public Effect? Shader { get; set; }


    // Methods.
    public void Draw(IDrawInfo drawInfo);
}