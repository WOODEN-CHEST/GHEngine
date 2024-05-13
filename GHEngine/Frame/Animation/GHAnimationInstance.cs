using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GHEngine.Frame.Animation;

public sealed class GHAnimationInstance : IAnimationInstance
{
    // Fields.
    public ISpriteAnimation Animation { get; private init; }
    public Rectangle? DrawRegion { get; set; } = null;
    public bool IsLooped { get; set; }

    public bool IsAnimating
    {
        get => _isAnimating;
        set
        {
            _isAnimating = value;
            UpdateShouldAnimate();
        }
    }

    public double FPS
    {
        get => _fps;
        set
        {
            if (value < 0d)
            {
                throw new ArgumentException("FPS cannot be negative", nameof(value));
            }

            _fps = value;
            _secondsPerFrame = 1d / _fps;
            UpdateShouldAnimate();
        }
    }

    public int FrameStep
    {
        get => _frameStep;
        set
        {
            _frameStep = value;
            UpdateShouldAnimate();
        }
    }

    public int FrameIndex
    {
        get => _frameIndex;
        set => _frameIndex = Math.Clamp(value, 0, Animation.MaxFrameIndex);
    }



    public event EventHandler<AnimFinishEventArgs>? AnimationFinished;


    // Private fields.
    private int _frameIndex;
    private int _frameStep = 1;

    private bool _isAnimating = true;
    private bool _shouldAnimate = true;
    private double _fps;
    private double _secondsSinceFrameSwitch;
    private double _secondsPerFrame;


    // Constructors.
    internal GHAnimationInstance(ISpriteAnimation animation)
    {
        Animation = animation ?? throw new ArgumentNullException(nameof(animation));
        Reset();
    }


    // Private methods.
    private void UpdateShouldAnimate()
    {
        _shouldAnimate = (_fps != 0d) && (_frameStep != 0) && (Animation.MaxFrameIndex != 0) && IsAnimating;
    }


    private void AnimationWrap(int newFrameIndex, AnimationFinishLocation finishLocation)
    {
        AnimationFinished?.Invoke(this, new(finishLocation));

        if (IsLooped)
        {
            FrameIndex = newFrameIndex;
        }
        else
        {
            _frameIndex = Math.Clamp(_frameIndex, 0, Animation.MaxFrameIndex);
            FrameStep = 0;
            UpdateShouldAnimate();
        }
    }

    private void IncrementFrame()
    {
        _frameIndex += FrameStep;

        if (_frameIndex > Animation.MaxFrameIndex)
        {
            AnimationWrap(0, AnimationFinishLocation.End);
        }
        else if (_frameIndex < 0)
        {
            AnimationWrap(Animation.MaxFrameIndex, AnimationFinishLocation.Start);
        }
    }


    // Inherited methods.
    public Texture2D GetCurrentFrame() => Animation.Frames[_frameIndex];

    public void Reset()
    {
        _secondsSinceFrameSwitch = 0;
        FPS = Animation.DefaultFPS;
        IsLooped = Animation.DefaultIsLooped;
        FrameStep = Animation.DefaultFrameStep;
        DrawRegion = Animation.DefaultDrawRegion;
        IsAnimating = Animation.DefaultIsAnimating;
    }

    public void Update(IProgramTime time)
    {
        if (!_shouldAnimate)
        {
            return;
        }

        _secondsSinceFrameSwitch += time.PassedTime.TotalSeconds;
        if (_secondsSinceFrameSwitch > _secondsPerFrame)
        {
            _secondsSinceFrameSwitch  = 0d;
            IncrementFrame();
        }
    }

    public object Clone()
    {
        return new GHAnimationInstance(Animation)
        {
            IsLooped = IsLooped,
            DrawRegion = DrawRegion,
            FPS = FPS,
            FrameIndex = FrameIndex,
            FrameStep = FrameStep,
            AnimationFinished = AnimationFinished,
            IsAnimating = IsAnimating,
        };
    }
}