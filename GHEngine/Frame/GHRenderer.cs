using GHEngine.GameFont;
using GHEngine.Frame.Item;
using GHEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace GHEngine.Frame;


public class GHRenderer : IFrameRenderer
{
    // Private static fields.
    private const float LAYER_DEPTH = 0.5f;
   

    // Fields.
    public float AspectRatio => _aspectRatio;


    // Private fields.
    private readonly GraphicsDevice _graphicsDevice;
    private readonly SpriteBatch _spriteBatch;
    private readonly IDisplay _display;

    private SpriteEffect? _currentShader = null;
    private SamplerState _currentState;
    private readonly Texture2D _lineTexture;

    private RenderTarget2D? _frameRenderTarget = null;
    private RenderTarget2D? _layerRenderTarget = null;
    
    private float _aspectRatio;

    private readonly SamplerState _defaultSamplerState = SamplerState.LinearWrap;



    // Constructors.
    private GHRenderer(GraphicsDevice device, IDisplay display)
    {
        _graphicsDevice = device ?? throw new ArgumentNullException(nameof(device));
        _spriteBatch = new SpriteBatch(device);

        _lineTexture = new Texture2D(device, 1, 1, false, SurfaceFormat.ColorSRgb);
        _lineTexture.SetData(new Color[] { Color.White });

        _display = display ?? throw new ArgumentNullException(nameof(display));
    }


    // Static methods.
    public static GHRenderer Create(GraphicsDevice device, IDisplay display)
    {
        GHRenderer Renderer = new(device, display);
        Renderer.Initialize();
        return Renderer;
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
        BeginSpriteBatch(effect, SamplerState.AnisotropicWrap);
        _spriteBatch.Draw(source, Vector2.Zero, null, ItemMask.CombinedMask, 0f, Vector2.Zero, 1f,
            SpriteEffects.None, LAYER_DEPTH);
        _spriteBatch.End();
    }

    private void OnDisplayChangeEvent(object? sender, ScreenSizeChangeEventArgs args)
    {
        UpdateRenderProperties();
    }

    private void UpdateRenderProperties()
    {
        _frameRenderTarget?.Dispose();
        _layerRenderTarget?.Dispose();

        _frameRenderTarget = new RenderTarget2D(_graphicsDevice, _display.CurrentWindowSize.X, _display.CurrentWindowSize.Y,
            false, SurfaceFormat.Color, DepthFormat.Depth16, 0, RenderTargetUsage.PreserveContents);
        _layerRenderTarget = new RenderTarget2D(_graphicsDevice, _display.CurrentWindowSize.X, _display.CurrentWindowSize.Y,
            false, SurfaceFormat.Color, DepthFormat.Depth16, 0, RenderTargetUsage.PreserveContents);

        _aspectRatio = (float)_display.CurrentWindowSize.X / (float)_display.CurrentWindowSize.Y;
    }

    private void BeginSpriteBatch(SpriteEffect? shader, SamplerState sampler)
    {
        _currentShader = shader;
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, sampler, DepthStencilState.None,
            RasterizerState.CullCounterClockwise, shader, null);
    }

    private void EnsurePropertiesForDrawCall(SpriteEffect? shader, SamplerState sampler)
    {
        if ((_currentShader != shader) || (sampler != _currentState))
        {
            _spriteBatch.End();
            _currentShader = shader;
            BeginSpriteBatch(_currentShader, sampler);
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
        return (Vector2)_display.CurrentWindowSize / textureSize * scale;
    }

    private void Initialize()
    {
        _display.ScreenSizeChange += OnDisplayChangeEvent;
        UpdateRenderProperties();
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
        SpriteEffect? shader,
        SamplerState? state)
    {
        EnsurePropertiesForDrawCall(shader, state ?? _defaultSamplerState);

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

    public void DrawLine(Color color, 
        Vector2 startPoint, 
        Vector2 endPoint, 
        float width, 
        SpriteEffect? shader,
        SamplerState state)
    {
        EnsurePropertiesForDrawCall(shader, state);
        Vector2 Change = endPoint - startPoint;

        _spriteBatch.Draw(_lineTexture,
            ToWindowPosition(startPoint),
            null,
            color,
            MathF.Atan2(Change.Y, Change.X * AspectRatio),
            new Vector2(0f, 0.5f),
            new Vector2((ToWindowPosition(endPoint) - ToWindowPosition(startPoint)).Length(),
            width * Math.Min(_display.WindowedSize.X, _display.WindowedSize.Y)),
            SpriteEffects.None,
            LAYER_DEPTH);
    }

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
        Vector2? precomputedRelativeSize)
    {
        EnsurePropertiesForDrawCall(shader, state ?? _defaultSamplerState);
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        Vector2 IntendedWindowTextSize = (Vector2)_display.CurrentWindowSize * size;
        Vector2 RelativeSize = precomputedRelativeSize ?? properties.FontFamily.MeasureRelativeSize(text,
            new(GHFontFamily.DEFAULT_SIZE, properties.IsBold, properties.IsItalic, properties.LineSpacing, properties.CharSpacing));

        Vector2 ScalingPerUnit = size / RelativeSize;
        float AbsoluteFontSize = _display.CurrentWindowSize.Y / RelativeSize.Y * size.Y;
        Vector2 TextureScaling = new(IntendedWindowTextSize.X / AbsoluteFontSize / RelativeSize.X, 1f);
        GHFontProperties FontProperties = new(AbsoluteFontSize,  properties.IsBold,
            properties.IsItalic, properties.LineSpacing, properties.CharSpacing);


        Vector2 RelativeTextPositionInScreen = position;
        for (int i = 0; i < text.Length; i++)
        {
            char Character = text[i];
            if (Character == '\n')
            {
                RelativeTextPositionInScreen = new(position.X,
                    RelativeTextPositionInScreen.Y + (size.Y / RelativeSize.Y));
                continue;
            }

            Texture2D CharTexture = properties.FontFamily.GetCharTexture(Character, FontProperties);
            Vector2 ToOriginVectorRelative = (position + origin * size) - RelativeTextPositionInScreen;
            Vector2 ToOriginVectorWindowAbsolute = ToOriginVectorRelative * (Vector2)_display.WindowedSize;
            Vector2 ToOriginVectorSpriteAbsolute = ToOriginVectorWindowAbsolute 
                * new Vector2(1f / TextureScaling.X, 1f / TextureScaling.Y);

            _spriteBatch.Draw(CharTexture,
                ToWindowPosition(RelativeTextPositionInScreen) + ToOriginVectorWindowAbsolute,
                null,
                mask,
                rotation,
                ToOriginVectorSpriteAbsolute,
                TextureScaling,
                SpriteEffects.None,
                LAYER_DEPTH);

            RelativeTextPositionInScreen.X += (float)CharTexture.Width / (float)CharTexture.Height * ScalingPerUnit.X;
        }
    }


    public void DrawRectangle(Color color, 
        Vector2 startingPoint,
        Vector2 EndPoint, 
        Vector2 origin, 
        float rotation,
        SpriteEffect? shader,
        SamplerState? state)
    {
        EnsurePropertiesForDrawCall(shader, state ?? _defaultSamplerState);

        Vector2 AbsoluteSize = (EndPoint - startingPoint) * (Vector2)_display.CurrentWindowSize;
        Vector2 AbsoluteOrigin = origin * AbsoluteSize;
        _spriteBatch.Draw(_lineTexture,
            ToWindowPosition(startingPoint) + AbsoluteOrigin,
            null,
            color,
            rotation,
            origin,
            AbsoluteSize,
            SpriteEffects.None,
            LAYER_DEPTH);
    }

    public void RenderFrame(IGameFrame frameToDraw, IProgramTime time)
    {
        _graphicsDevice.Clear(Color.Black);

        if (frameToDraw.LayerCount == 0)
        {
            return;
        }

        foreach (ILayer Layer in frameToDraw.Layers)
        {
            if ((Layer.DrawableItemCount == 0) || !Layer.IsVisible)
            {
                continue;
            }

            _graphicsDevice.SetRenderTarget(_layerRenderTarget);
            _graphicsDevice.Clear(Color.Transparent);
            BeginSpriteBatch(null, _defaultSamplerState);
            Layer.Render(this, time);
            _spriteBatch.End();

            RenderColorMaskableOntoTarget(Layer, Layer.Shader, _layerRenderTarget!, _frameRenderTarget);
        }

        RenderColorMaskableOntoTarget(frameToDraw, frameToDraw.Shader, _frameRenderTarget!, null);
    }

    public void Dispose()
    {
        _display.ScreenSizeChange -= OnDisplayChangeEvent;
        _layerRenderTarget?.Dispose();
        _frameRenderTarget?.Dispose();
        _lineTexture.Dispose();
        _spriteBatch.Dispose();
    }
}