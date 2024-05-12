using Microsoft.Xna.Framework.Graphics;


namespace GHEngine.Frame.Item;


public interface IDrawInfo
{
    // Internal fields.
    public SpriteBatch SpriteBatch { get; }
    public IProgramTime Time { get; }
}