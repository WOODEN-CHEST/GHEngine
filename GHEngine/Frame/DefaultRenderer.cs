using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Frame;

public class DefaultRenderer : IRenderer
{
    // Private fields.
    private readonly GraphicsDevice _graphicsDevice;
    private readonly RenderTarget2D? _renderTarget;
    private readonly SpriteBatch _spriteBatch;

    private bool _isDrawingStarted = false;
    private SpriteEffect? _currentShader = null;
    private Matrix _drawMatrix;


    // Constructors.
    public DefaultRenderer(RenderTarget2D? renderTarget, GraphicsDevice device)
    {
        _graphicsDevice = device ?? throw new ArgumentNullException(nameof(device));
        _renderTarget = renderTarget;
        _spriteBatch = new SpriteBatch(device);
    }


    // Methods.
    public void BeginDraw(Vector2 windowSize, float aspectRatio)
    {
        _graphicsDevice.SetRenderTarget(_renderTarget);

        _drawMatrix = Matrix.Identity;
        _drawMatrix.M11 *= aspectRatio;
        _drawMatrix.M22;
 
        BeginSpriteBatch(null);
    }

    public void EndDraw()
    {
        _spriteBatch.End();
    }


    // Inherited methods.
    public void DrawSprite(Texture2D texture, 
        Vector2 position,
        Rectangle? sourceArea,
        Color mask,
        float rotation, 
        Vector2 origin,
        Vector2 scale,
        SpriteEffects effects,
        SpriteEffect shader)
    {
        
        _spriteBatch.Draw(texture, position, sourceArea, mask, rotation, origin, scale, effects, 0f);
    }

    public void DrawString(SpriteFont font, 
        string text, 
        Vector2 position,
        Color mask, 
        float rotation, 
        Vector2 origin,
        Vector2 scale, 
        SpriteEffect shader)
    {
        throw new NotImplementedException();
    }

    public void DrawLine(Color color, Vector2 startPoint, Vector2 endPoint, float width)
    {
        throw new NotImplementedException();
    }

    public void DrawLine(Color color, Vector2 startPoint, float rotation, float width)
    {
        throw new NotImplementedException();
    }


    // Private methods.
    private void BeginSpriteBatch(SpriteEffect? shader)
    {
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.Default,
            RasterizerState.CullClockwise, shader, _drawMatrix);
    }

    private void EnsureSpriteBatch(SpriteEffect shader)
    {
        if (_currentShader != shader)
        {
            _spriteBatch.End();
            BeginSpriteBatch(_currentShader);
        }
    }
}