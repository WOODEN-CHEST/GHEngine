using Microsoft.Xna.Framework.Graphics;


namespace GHEngine.Frame;

public interface IShadered
{
    public SpriteEffect? Shader { get; set; }
}