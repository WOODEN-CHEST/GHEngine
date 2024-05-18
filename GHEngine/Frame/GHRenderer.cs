using GHEngine.Frame.Item;
using GHEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace GHEngine.Frame;


public class GHRenderer : IRenderer
{
    // Private static fields.
    private const float LAYER_DEPTH = 0f;
    private const float ONE_LINE_UNIT = 0.05f;


    // Private fields.
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;
    private readonly IDisplay _display;

    private SpriteEffect? _currentShader = null;
    private readonly Texture2D _lineTexture;

    private RenderTarget2D _frameRenderTarget = null!;
    private RenderTarget2D _layerRenderTarget = null!;


    // Constructors.
    public GHRenderer(GraphicsDevice device, IDisplay display)
    {
        _graphicsDevice = device ?? throw new ArgumentNullException(nameof(device));
        _spriteBatch = new SpriteBatch(device);

        _lineTexture = new Texture2D(device, 1, 1, false, SurfaceFormat.ColorSRgb);
        _lineTexture.SetData(new Color[] { Color.White });

        _display = display ?? throw new ArgumentNullException(nameof(display));
        _display.ScreenSizeChange += OnDisplayChangeEvent;

        CreateRenderTargets();
    }


    // Methods.
    public void RenderFrame(IGameFrame frameToDraw, IProgramTime time)
    {
        foreach (ILayer Layer in frameToDraw.Layers)
        {
            if ((Layer.DrawableItemCount == 0) || !Layer.IsVisible)
            {
                continue;
            }
            
            _graphicsDevice.SetRenderTarget(_layerRenderTarget);
            BeginSpriteBatch(null);
            Layer.Render(this, time);
            _spriteBatch.End();

            RenderColorMaskableOntoTarget(Layer, Layer.Shader, _layerRenderTarget, _frameRenderTarget);
        }

        RenderColorMaskableOntoTarget(frameToDraw, frameToDraw.Shader, _frameRenderTarget, null);
    }


    // Private methods.
    private void RenderColorMaskableOntoTarget(IColorMaskable maskable, SpriteEffect? effect, Texture2D source, RenderTarget2D? target)
    {
        GenericColorMask ItemMask = new();
        ItemMask.Mask = maskable.Mask;
        ItemMask.Brightness = maskable.Brightness;
        ItemMask.Opacity = maskable.Opacity;

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
            false, SurfaceFormat.ColorSRgb, DepthFormat.Depth16, 0, RenderTargetUsage.PreserveContents);
        _layerRenderTarget = new RenderTarget2D(_graphicsDevice, _display.CurrentWindowSize.X, _display.CurrentWindowSize.Y,
            false, SurfaceFormat.ColorSRgb, DepthFormat.Depth16, 0, RenderTargetUsage.PreserveContents);
    }

    private void BeginSpriteBatch(SpriteEffect? shader)
    {
        _currentShader = shader;
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.Default,
            RasterizerState.CullClockwise, shader, null);
    }

    private void EnsureShaderForDrawCall(SpriteEffect? shader)
    {
        if (_currentShader != shader)
        {
            _spriteBatch.End();
            BeginSpriteBatch(_currentShader);
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
        _spriteBatch.Draw(texture, _display.ToWindowPosition(position), sourceArea, mask, rotation,
            origin * new Vector2(texture.Width, texture.Height),
            _display.ToWindowSize(size) / new Vector2(texture.Width, texture.Height), effects, LAYER_DEPTH);
    }

    public void DrawString(SpriteFont font, 
        string text, 
        Vector2 position,
        Color mask, 
        float rotation, 
        Vector2 origin,
        Vector2 size,
        SpriteEffects effects,
        SpriteEffect? shader)
    {
        EnsureShaderForDrawCall(shader);
        Vector2 TextSize = font.MeasureString(text);
        _spriteBatch.DrawString(font, text, _display.ToWindowPosition(position), mask, rotation,
            origin * TextSize, _display.ToWindowSize(size) / TextSize, effects, LAYER_DEPTH);
    }

    public void DrawLine(Color color, Vector2 startPoint, Vector2 endPoint, float width, SpriteEffect? shader)
    {
        Vector2 Change = endPoint - startPoint;
        DrawLine(color, startPoint, MathF.Atan2(-Change.Y, Change.X), width, Change.Length(), shader);
    }

    public void DrawLine(Color color, Vector2 startPoint, float rotation, float width, float length, SpriteEffect? shader)
    {
        EnsureShaderForDrawCall(shader);
        _spriteBatch.Draw(_lineTexture, _display.ToWindowPosition(startPoint), null, color, rotation,
            new Vector2(0.5f, 0f), _display.ToWindowSize(new Vector2(ONE_LINE_UNIT * width, ONE_LINE_UNIT * length)),
            SpriteEffects.None, LAYER_DEPTH);
    }

    public void Dispose()
    {
        _display.ScreenSizeChange -= OnDisplayChangeEvent;
        _layerRenderTarget.Dispose();
        _frameRenderTarget.Dispose();
        _lineTexture.Dispose();
        _spriteBatch.Dispose();
    }
}