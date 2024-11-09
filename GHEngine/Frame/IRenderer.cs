using GHEngine.GameFont;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GHEngine.Frame;

public interface IRenderer : IDisposable
{
    // Fields.
    public float AspectRatio { get; }


    // Methods.
    public void DrawSprite(Texture2D texture,
        Vector2 position,
        Rectangle? sourceArea,
        Color mask,
        float rotation,
        Vector2 origin,
        Vector2 size,
        SpriteEffects effects,
        SpriteEffect? shader,
        SamplerState? state);

    public void DrawString(FontRenderProperties properties,
        string text,
        Vector2 position,
        Color mask,
        float rotation,
        Vector2 origin,
        Vector2 size,
        SpriteEffects effects,
        SpriteEffect? shader,
        SamplerState? state,
        Vector2? precomputedRelativeDrawSize);

    public void DrawLine(Color color,
        Vector2 startPoint,
        Vector2 endPoint,
        float width,
        SpriteEffect? shader,
        SamplerState? state);

    public void DrawRectangle(Color color,
        Vector2 startingPoint,
        Vector2 EndPoint,
        Vector2 origin,
        float rotation,
        SpriteEffect? shader,
        SamplerState? state);
}