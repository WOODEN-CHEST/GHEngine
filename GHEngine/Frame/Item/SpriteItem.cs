using GHEngine.Frame.Animation;
using GHEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics.CodeAnalysis;

namespace GHEngine.Frame.Item;

public class SpriteItem : ICloneable, IColorMaskable, IRenderableItem, ITimeUpdatable, IShadered
{
    // Fields.
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public float Rotation { get; set; }
    Vector2 Origin { get; set; }
    public SpriteEffects Effects { get; set; }

    public IAnimationInstance Animation
    {
        get => _animation;
        set => _animation = value ?? throw new ArgumentNullException(nameof(value));
    }

    public float Brightness
    {
        get => _colorMask.Brightness;
        set => _colorMask.Brightness = value;
    }

    public float Opacity
    {
        get => _colorMask.Opacity;
        set => _colorMask.Opacity = value;
    }

    public Color Mask
    {
        get => _colorMask.Mask;
        set => _colorMask.Mask = value;
    }

    public bool IsVisible { get; set; }
    public SpriteEffect? Shader { get; set; }


    // Private fields.
    private GenericColorMask _colorMask;
    private IAnimationInstance _animation;


    // Constuctors.
    public SpriteItem(IAnimationInstance instance, Vector2 size)
    {
        Animation = instance;
        Size = size;
    }



    // Inherited methods.
    public object Clone()
    {
        return new SpriteItem((IAnimationInstance)Animation.CreateClone(), Size)
        {
            _colorMask = _colorMask,
            IsVisible = IsVisible,
            Position = Position,
        };
    }

    public void Render(IRenderer renderer, IProgramTime time)
    {
        renderer.DrawSprite(_animation.CurrentFrame, Position, _animation.DrawRegion, _colorMask.CombinedMask,
            Rotation, Origin, Size, Effects, Shader);
    }

    public void Update(IProgramTime time)
    {
        _animation.Update(time);
    }
}