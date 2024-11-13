using GHEngine.GameFont;
using GHEngine.Frame.Item;
using GHEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection.Emit;


namespace GHEngine.Frame;


public class GHRenderer : IFrameRenderer
{
    // Private static fields.
    private const float LAYER_DEPTH = 0.5f;
   

    // Fields.
    public float AspectRatio => _aspectRatio;
    public Color? ScreenColor { get; set; } = Color.Black;


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
    private Rectangle? GetBoundsRectangle(RectangleF? normalizedRectangle, Vector2 textureSize)
    {
        if (normalizedRectangle.HasValue)
        {
            RectangleF Value = normalizedRectangle.Value;
            return new Rectangle((int)(Value.X * textureSize.X), (int)(Value.Y * textureSize.Y),
                (int)(Value.Width * textureSize.X), (int)(Value.Height * textureSize.Y));
        }
        return null;
    }

    private void RenderLayerOnFrame(ILayer layer, Color? frameClearColor)
    {
        _graphicsDevice.SetRenderTarget(_frameRenderTarget);

        if (frameClearColor != null)
        {
            _graphicsDevice.Clear(frameClearColor.Value);
        }

        Vector2 WindowSize = (Vector2)_display.CurrentWindowSize;
        BeginSpriteBatch(layer.Shader, layer.CustomSamplerState);
        _spriteBatch.Draw(_layerRenderTarget,
            layer.Position * WindowSize, 
            GetBoundsRectangle(layer.DrawBounds, (Vector2)_display.CurrentWindowSize),
            new GenericColorMask(layer.Mask, layer.Brightness, layer.Opacity).CombinedMask, 
            layer.Rotation,
            layer.Origin * WindowSize, 
            layer.Size,
            layer.Effects,
            LAYER_DEPTH);
        _spriteBatch.End();
    }

    private void RenderFrameOnScreen(IGameFrame frame)
    {
        _graphicsDevice.SetRenderTarget(null);

        if (ScreenColor != null)
        {
            _graphicsDevice.Clear(ScreenColor.Value);
        }

        Vector2 WindowSize = (Vector2)_display.CurrentWindowSize;
        BeginSpriteBatch(frame.Shader, frame.CustomSamplerState);
        _spriteBatch.Draw(_frameRenderTarget,
        frame.Position * WindowSize,
        GetBoundsRectangle(frame.DrawBounds, (Vector2)_display.CurrentWindowSize),
        new GenericColorMask(frame.Mask, frame.Brightness, frame.Opacity).CombinedMask,
        frame.Rotation,
        frame.Origin * WindowSize,
        frame.Size,
        frame.Effects,
            LAYER_DEPTH);
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

    private void BeginSpriteBatch(SpriteEffect? shader, SamplerState? sampler)
    {
        _currentShader = shader;
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, sampler ?? _defaultSamplerState,
            DepthStencilState.None, RasterizerState.CullCounterClockwise, shader, null);
    }

    private void EnsurePropertiesForDrawCall(SpriteEffect? shader, SamplerState? sampler)
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

    private (Rectangle?, Vector2) GetCharDrawBounds(RectangleF? fullDrawBounds,
        Vector2 position, 
        Vector2 relativeCharPosition,
        Vector2 charTextureSize,
        Vector2 size,
        Vector2 textureScaling)
    {
        if (fullDrawBounds == null)
        {
            return (null, Vector2.Zero);
        }

        float MinX = Math.Max(0f, fullDrawBounds.Value.X * size.X + position.X - relativeCharPosition.X)
            * _display.WindowedSize.X / textureScaling.X;
        float MinY = Math.Max(0f, fullDrawBounds.Value.Y * size.Y + position.Y - relativeCharPosition.Y)
            * _display.WindowedSize.Y / textureScaling.Y;
        float MaxX = Math.Max(0f, fullDrawBounds.Value.Height * size.X + position.X - relativeCharPosition.X)
            * _display.WindowedSize.X / textureScaling.X;
        float MaxY = Math.Max(0f, fullDrawBounds.Value.Width * size.Y + position.Y - relativeCharPosition.Y)
            * _display.WindowedSize.Y / textureScaling.Y;

        float Width = Math.Max(Math.Min(MaxX - MinX, charTextureSize.X - MinX), 0f);
        float Height = Math.Max(Math.Min(MaxY - MinY, charTextureSize.Y- MinY), 0f);

        return (new Rectangle((int)MinX, (int)MinY, (int)Width, (int)Height),  
            new Vector2(MinX, MinY));
    }



    // Inherited methods.
    public void DrawSprite(Texture2D texture, 
        Vector2 position,
        RectangleF? sourceArea,
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
            GetBoundsRectangle(sourceArea, new(texture.Width, texture.Height)), 
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
        SamplerState? state)
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
        RectangleF? bounds,
        Color mask,
        float rotation,
        Vector2 origin,
        Vector2 size,
        SpriteEffects effects,
        SpriteEffect? shader,
        SamplerState? state,
        Vector2? precomputedRelativeSize)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }
        EnsurePropertiesForDrawCall(shader, state ?? _defaultSamplerState);

        Vector2 IntendedWindowTextSize = (Vector2)_display.CurrentWindowSize * size;
        Vector2 RelativeSize = precomputedRelativeSize ?? properties.FontFamily.MeasureRelativeSize(text,
            new(GHFontFamily.DEFAULT_SIZE, properties.IsBold, properties.IsItalic, properties.LineSpacing, properties.CharSpacing));

        Vector2 ScalingPerUnit = size / RelativeSize;
        float AbsoluteFontSize = Math.Abs(_display.CurrentWindowSize.Y / RelativeSize.Y * size.Y);
        Vector2 TextureScaling = new(IntendedWindowTextSize.X / AbsoluteFontSize / RelativeSize.X, 1f);
        GHFontProperties FontProperties = new(AbsoluteFontSize,  properties.IsBold,
            properties.IsItalic, properties.LineSpacing, properties.CharSpacing);

        Vector2 RelativeTextPositionInScreen = position;
        for (int i = 0; i < text.Length; i++)
        {
            char Character = text[i];

            if (Character == '\n')
            {
                RelativeTextPositionInScreen = new(position.X, RelativeTextPositionInScreen.Y
                    + (size.Y * (properties.LineSpacing + 1f) / RelativeSize.Y));
                continue;
            }

            Texture2D? CharTexture = properties.FontFamily.GetCharTexture(Character, FontProperties);
            if (CharTexture == null)
            {
                continue;
            }

            (Rectangle? CharDrawBounds, Vector2 BoundsOffset) = GetCharDrawBounds(bounds, position,
                RelativeTextPositionInScreen, new Vector2(CharTexture.Width, CharTexture.Height), size, TextureScaling);
            if (CharDrawBounds.HasValue && (CharDrawBounds.Value.Width == 0 || CharDrawBounds.Value.Height == 0))
            {
                continue;
            }
            Vector2 ToOriginVectorRelative = (position + origin * size) - RelativeTextPositionInScreen;
            Vector2 ToOriginVectorWindowAbsolute = ToOriginVectorRelative * (Vector2)_display.WindowedSize - BoundsOffset;
            Vector2 ToOriginVectorSpriteAbsolute = ToOriginVectorWindowAbsolute / new Vector2(TextureScaling.X, TextureScaling.Y);

            _spriteBatch.Draw(CharTexture,
                ToWindowPosition(RelativeTextPositionInScreen) + ToOriginVectorWindowAbsolute + BoundsOffset,
                CharDrawBounds,
                mask,
                rotation,
                ToOriginVectorSpriteAbsolute,
                TextureScaling,
                effects,
                LAYER_DEPTH);

            RelativeTextPositionInScreen.X += ((float)CharTexture.Width / (float)CharTexture.Height * ScalingPerUnit.X)
                + (properties.CharSpacing * size.Y / RelativeSize.X);
        }
    }


    public void DrawRectangle(Color color, 
        Vector2 startingPoint,
        Vector2 endPoint, 
        Vector2 origin, 
        float rotation,
        SpriteEffect? shader,
        SamplerState? state)
    {
        EnsurePropertiesForDrawCall(shader, state ?? _defaultSamplerState);

        Vector2 MinPoint = new(Math.Min(startingPoint.X, endPoint.X), Math.Min(startingPoint.Y, endPoint.Y));
        Vector2 MaxPoint = new(Math.Max(startingPoint.X, endPoint.X), Math.Max(startingPoint.Y, endPoint.Y));

        Vector2 AbsoluteSize = (MaxPoint - MinPoint) * (Vector2)_display.CurrentWindowSize;
        Vector2 AbsoluteOrigin = origin * AbsoluteSize;
        _spriteBatch.Draw(_lineTexture,
            ToWindowPosition(MinPoint) + AbsoluteOrigin,
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
        if (frameToDraw.LayerCount == 0)
        {
            _graphicsDevice.SetRenderTarget(null);
            _graphicsDevice.Clear(Color.Black);
            return;
        }

        ILayer[] Layers = frameToDraw.Layers;
        for (int i = 0; i < Layers.Length; i++)
        {
            ILayer Layer = Layers[i];
            if ((Layer.DrawableItemCount == 0) || !Layer.IsVisible)
            {
                continue;
            }

            _graphicsDevice.SetRenderTarget(_layerRenderTarget);
            _graphicsDevice.Clear(Color.Transparent);
            BeginSpriteBatch(null, _defaultSamplerState);
            Layer.Render(this, time);
            _spriteBatch.End();

            RenderLayerOnFrame(Layer, i == 0 ? Color.Transparent : null);
        }

        RenderFrameOnScreen(frameToDraw);
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