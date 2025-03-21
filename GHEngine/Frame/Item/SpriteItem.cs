﻿using GHEngine.Frame.Animation;
using GHEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics.CodeAnalysis;

namespace GHEngine.Frame.Item;

public class SpriteItem : IColorMaskable, IRenderableItem, ITimeUpdatable, IShadered
{
    // Fields.
    public Vector2 Position { get; set; } = Vector2.Zero;
    public Vector2 PositionOrigin { get; set; } = Vector2.Zero;
    public Vector2 Size { get; set; } = Vector2.One;
    public float Rotation { get; set; } = 0f;
    public Vector2 Origin { get; set; } = Vector2.Zero;
    public SpriteEffects Effects { get; set; } = SpriteEffects.None;
    public Vector2 FrameSize => new(Animation.CurrentFrame.Width, Animation.CurrentFrame.Height);
    public bool IsSizeAdjusted { get; set; } = true;
    public bool IsPositionAdjusted { get; set; } = false;

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

    public SamplerState? TargetSampleState { get; set; } = null;


    // Private fields.
    private GenericColorMask _colorMask = new();
    private IAnimationInstance _animation;


    // Constructors.
    public SpriteItem(IAnimationInstance instance)
    {
        Animation = instance;
    }

    public SpriteItem(SpriteItem itemToClone)
    {
        Animation = itemToClone.Animation.CreateClone();
        Position = itemToClone.Position;
        _colorMask = itemToClone._colorMask;
        Origin = itemToClone.Origin;
        Size = itemToClone.Size;
        Rotation = itemToClone.Rotation;
        Effects = itemToClone.Effects;
        Shader = itemToClone.Shader;
        IsVisible = itemToClone.IsVisible;
    }


    // Methods.
    public SpriteItem CreateClone()
    {
        return new(this);
    }

    public Vector2 GetAdjustedDrawSize(float aspectRatio)
    {
        return GHMath.GetWindowAdjustedVector(Size, aspectRatio);
    }

    public Vector2 GetAdjustedPosition(float aspectRatio)
    {
        return PositionOrigin + GHMath.GetWindowAdjustedVector(Position, aspectRatio);
    }


    // Inherited methods.
    public void Render(IRenderer renderer, IProgramTime time)
    {
        Vector2 DrawPosition = IsPositionAdjusted ? GetAdjustedPosition(renderer.AspectRatio) : Position;
        Vector2 DrawSize = IsSizeAdjusted ? GetAdjustedDrawSize(renderer.AspectRatio) : Size;

        renderer.DrawSprite(_animation.CurrentFrame, DrawPosition, _animation.DrawRegion, _colorMask.CombinedMask,
            Rotation, Origin, DrawSize, Effects, Shader, TargetSampleState);
    }

    public void Update(IProgramTime time)
    {
        _animation.Update(time);
    }
}