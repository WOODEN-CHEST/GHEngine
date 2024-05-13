using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GHEngine.Frame.Animation;

public sealed class GHSpriteAnimation : ISpriteAnimation
{
    // Fields.
    public Texture2D[] Frames => _frames.ToArray();
    public int FrameCount => _frames.Length;
    public int MaxFrameIndex => FrameCount - 1;
    public double DefaultFPS { get; private init; }
    public int DefaultFrameStep { get; private init; }
    public bool DefaultIsLooped { get; private init; }
    public Rectangle? DefaultDrawRegion { get; private init; }
    public bool DefaultIsAnimating { get; private init; }


    // Private fields.
    private readonly Texture2D[] _frames;


    // Constructors.
    public GHSpriteAnimation(double fps, 
        int step,
        bool isLooped,
        Rectangle? drawRegion,
        bool isAnimating,
        params Texture2D[] frames)
    {
        DefaultFPS = fps;

        if (frames == null)
        {
            throw new ArgumentNullException(nameof(frames));
        }
        if (frames.Length == 0)
        {
            throw new ArgumentException("Animation must have at least 1 frame", nameof(frames));
        }

        _frames = frames;
        DefaultFrameStep = step;
        DefaultIsLooped = isLooped;
        DefaultDrawRegion = drawRegion;
        DefaultIsAnimating = isAnimating;
    }


    // Methods.
    public IAnimationInstance CreateInstance()
    {
        return new GHSpriteAnimation(this);
    }


    // Operators.
    public Texture2D this[int index]
    {
        get => Frames[index];
    }
}