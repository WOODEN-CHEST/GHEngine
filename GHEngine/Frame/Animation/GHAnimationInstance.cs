using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GHEngine.Frame.Animation;

public sealed class GHAnimationInstance : IAnimationInstance
{
    // Fields.
    public ISpriteAnimation Source { get; private init; }
    public RectangleF? DrawRegion { get; set; } = null;
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
        set => _frameIndex = Math.Clamp(value, 0, Source.MaxFrameIndex);
    }

    public Texture2D CurrentFrame => Source[_frameIndex];



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
    public GHAnimationInstance(ISpriteAnimation animation)
    {
        Source = animation ?? throw new ArgumentNullException(nameof(animation));
        Reset();
    }

    private GHAnimationInstance(GHAnimationInstance animationToClone)
    {
        Source = animationToClone.Source;
        IsLooped = animationToClone.IsLooped;
        DrawRegion = animationToClone.DrawRegion;
        FPS = animationToClone.FPS;
        FrameIndex = animationToClone.FrameIndex;
        FrameStep = animationToClone.FrameStep;
        AnimationFinished = animationToClone.AnimationFinished;
        IsAnimating = animationToClone.IsAnimating;
    }


    // Private methods.
    private void UpdateShouldAnimate()
    {
        _shouldAnimate = (_fps != 0d) && (_frameStep != 0) && (Source.MaxFrameIndex != 0) && IsAnimating;
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
            _frameIndex = Math.Clamp(_frameIndex, 0, Source.MaxFrameIndex);
            FrameStep = 0;
            UpdateShouldAnimate();
        }
    }

    private void IncrementFrame()
    {
        _frameIndex += FrameStep;

        if (_frameIndex > Source.MaxFrameIndex)
        {
            AnimationWrap(0, AnimationFinishLocation.End);
        }
        else if (_frameIndex < 0)
        {
            AnimationWrap(Source.MaxFrameIndex, AnimationFinishLocation.Start);
        }
    }


    // Inherited methods.
    public void Reset()
    {
        _secondsSinceFrameSwitch = 0;
        FPS = Source.DefaultFPS;
        IsLooped = Source.DefaultIsLooped;
        FrameStep = Source.DefaultFrameStep;
        DrawRegion = Source.DefaultDrawRegion;
        IsAnimating = Source.DefaultIsAnimating;
    }

    public void Update(IProgramTime time)
    {
        if (!_shouldAnimate)
        {
            return;
        }

        _secondsSinceFrameSwitch += time.PassedTime.TotalSeconds;
        while (_secondsSinceFrameSwitch > _secondsPerFrame)
        {
            _secondsSinceFrameSwitch -= _secondsPerFrame;
            IncrementFrame();
        }
    }

    public IAnimationInstance CreateClone()
    {
        return new GHAnimationInstance(this);
    }
}