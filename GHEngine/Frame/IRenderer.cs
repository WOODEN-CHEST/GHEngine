﻿using GHEngine.Font;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GHEngine.Frame;

public interface IRenderer : IDisposable
{
    public void DrawSprite(Texture2D texture,
        Vector2 position,
        Rectangle? sourceArea,
        Color mask,
        float rotation,
        Vector2 origin,
        Vector2 size,
        SpriteEffects effects,
        SpriteEffect? shader);

    public void DrawString(GHFont font,
        string text,
        Vector2 position,
        Color mask,
        float rotation,
        Vector2 origin,
        Vector2 size,
        SpriteEffects effects,
        SpriteEffect? shader);

    public void DrawLine(Color color,
        Vector2 startPoint,
        Vector2 endPoint,
        float width,
        SpriteEffect? shader);

    public void DrawLine(Color color,
        Vector2 startPoint,
        float rotation,
        float width,
        float length,
        SpriteEffect? shader);

    public void DrawRectangle(Color color,
        Vector2 startingPoint,
        Vector2 EndPoint,
        SpriteEffect? shader);
}