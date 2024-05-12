using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GHEngine.Frame;

public interface IRenderer
{
    public void DrawSprite(Texture2D texture,
        Vector2 position,
        Rectangle? sourceArea,
        Color mask,
        float rotation,
        Vector2 origin,
        Vector2 scale,
        SpriteEffects effects,
        SpriteEffect shader);

    public void DrawString(SpriteFont font,
        string text,
        Vector2 position,
        Color mask,
        float rotation,
        Vector2 origin,
        Vector2 scale,
        SpriteEffect shader);

    public void DrawLine(Color color,
        Vector2 startPoint,
        Vector2 endPoint,
        float width);

    public void DrawLine(Color color,
        Vector2 startPoint,
        float rotation,
        float width);
}