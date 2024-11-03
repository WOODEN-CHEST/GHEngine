using GHEngine.Font;
using GHEngine.Frame.Item;
using GHEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace GHEngine.Frame;


public class GHRenderer : IFrameRenderer
{
    // Private static fields.
    private const float LAYER_DEPTH = 0.5f;
    private const float ONE_LINE_UNIT = 0.05f;


    // Private fields.
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;
    private readonly IDisplay _display;

    private SpriteEffect? _currentShader = null;
    private readonly Texture2D _lineTexture;

    private RenderTarget2D? _frameRenderTarget = null;
    private RenderTarget2D? _layerRenderTarget = null;


    // Constructors.
    public GHRenderer(GraphicsDevice device, IDisplay display)
    {
        _graphicsDevice = device ?? throw new ArgumentNullException(nameof(device));
        _spriteBatch = new SpriteBatch(device);

        _lineTexture = new Texture2D(device, 1, 1, false, SurfaceFormat.ColorSRgb);
        _lineTexture.SetData(new Color[] { Color.White });

        _display = display ?? throw new ArgumentNullException(nameof(display));
    }


    // Private methods.
    private void RenderColorMaskableOntoTarget(IColorMaskable maskable, SpriteEffect? effect, Texture2D source, RenderTarget2D? target)
    {
        GenericColorMask ItemMask = new()
        {
            Mask = maskable.Mask,
            Brightness = maskable.Brightness,
            Opacity = maskable.Opacity
        };

        _graphicsDevice.SetRenderTarget(target);
        BeginSpriteBatch(effect);
        _spriteBatch.Draw(source, Vector2.Zero, null, ItemMask.CombinedMask, 0f, Vector2.Zero, 1f,
            SpriteEffects.None, LAYER_DEPTH);
        _spriteBatch.End();
    }

    private void OnDisplayChangeEvent(object? sender, ScreenSizeChangeEventArgs args)
    {
        CreateRenderTargets();
    }

    private void CreateRenderTargets()
    {
        _frameRenderTarget?.Dispose();
        _layerRenderTarget?.Dispose();

        _frameRenderTarget = new RenderTarget2D(_graphicsDevice, _display.CurrentWindowSize.X, _display.CurrentWindowSize.Y,
            false, SurfaceFormat.Color, DepthFormat.Depth16, 0, RenderTargetUsage.PreserveContents);
        _layerRenderTarget = new RenderTarget2D(_graphicsDevice, _display.CurrentWindowSize.X, _display.CurrentWindowSize.Y,
            false, SurfaceFormat.Color, DepthFormat.Depth16, 0, RenderTargetUsage.PreserveContents);
    }

    private void BeginSpriteBatch(SpriteEffect? shader)
    {
        _currentShader = shader;
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None,
            RasterizerState.CullCounterClockwise, shader, null);
    }

    private void EnsureShaderForDrawCall(SpriteEffect? shader)
    {
        if (_currentShader != shader)
        {
            _spriteBatch.End();
            _currentShader = shader;
            BeginSpriteBatch(_currentShader);
        }
    }

    private Vector2 ToVirtualPosition(Vector2 position)
    {
        return position / (Vector2)_display.CurrentWindowSize;
    }

    private Vector2 ToWindowPosition(Vector2 position)
    {
        return position * (Vector2)_display.CurrentWindowSize;
    }

    private Vector2 ToWindowScale(Vector2 textureSize, Vector2 scale)
    {
        IntVector CurrentWindowSize = _display.CurrentWindowSize;
        if (CurrentWindowSize.X > CurrentWindowSize.Y)
        {
            return new Vector2(CurrentWindowSize.Y) / textureSize * scale;
        }
        else
        {
            return new Vector2(CurrentWindowSize.X) / textureSize * scale;
        }
    }


    // Inherited methods.
    public void DrawSprite(Texture2D texture, 
        Vector2 position,
        Rectangle? sourceArea,
        Color mask,
        float rotation, 
        Vector2 origin,
        Vector2 size,
        SpriteEffects effects,
        SpriteEffect? shader)
    {
        EnsureShaderForDrawCall(shader);
        _spriteBatch.Draw(texture,
            ToWindowPosition(position),
            sourceArea, 
            mask, 
            rotation,
            origin * new Vector2(texture.Width, texture.Height),
            ToWindowScale(new(texture.Width, texture.Height), size), 
            effects, 
            LAYER_DEPTH);
    }

    public void DrawLine(Color color, Vector2 startPoint, Vector2 endPoint, float width, SpriteEffect? shader)
    {
        Vector2 Change = endPoint - startPoint;
        DrawLine(color, startPoint, MathF.Atan2(-Change.Y, Change.X), width, Change.Length(), shader);
    }

    public void DrawLine(Color color, Vector2 startPoint, float rotation, float width, float length, SpriteEffect? shader)
    {
        throw new NotImplementedException();
        //EnsureShaderForDrawCall(shader);
        //_spriteBatch.Draw(_lineTexture, _display.ToWindowPosition(startPoint), null, color, rotation,
        //    new Vector2(0.5f, 0f), _display.ToWindowSize(new Vector2(ONE_LINE_UNIT * width, ONE_LINE_UNIT * length)),
        //    SpriteEffects.None, LAYER_DEPTH);
    }

    public void Dispose()
    {
        _display.ScreenSizeChange -= OnDisplayChangeEvent;
        _layerRenderTarget?.Dispose();
        _frameRenderTarget?.Dispose();
        _lineTexture.Dispose();
        _spriteBatch.Dispose();
        _display.ScreenSizeChange -= OnDisplayChangeEvent;
    }

    public void DrawString(GHFont font, 
        string text, Vector2 position,
        Color mask,
        float rotation,
        Vector2 origin,
        Vector2 size, 
        SpriteEffects effects, 
        SpriteEffect? shader)
    {
        throw new NotImplementedException();
    }

    public void DrawRectangle(Color color, Vector2 startingPoint, Vector2 EndPoint, SpriteEffect? shader)
    {
        throw new NotImplementedException();
    }

    public void RenderFrame(IGameFrame frameToDraw, IProgramTime time)
    {
        foreach (ILayer Layer in frameToDraw.Layers)
        {
            if ((Layer.DrawableItemCount == 0) || !Layer.IsVisible)
            {
                continue;
            }

            _graphicsDevice.SetRenderTarget(_layerRenderTarget);
            _graphicsDevice.Clear(Color.Transparent);
            BeginSpriteBatch(null);
            Layer.Render(this, time);
            _spriteBatch.End();

            RenderColorMaskableOntoTarget(Layer, Layer.Shader, _layerRenderTarget!, _frameRenderTarget);
        }

        RenderColorMaskableOntoTarget(frameToDraw, frameToDraw.Shader, _frameRenderTarget!, null);
    }

    public void Initialize()
    {
        _display.ScreenSizeChange += OnDisplayChangeEvent;
        CreateRenderTargets();
    }
}