namespace GHEngine.Frame.Animation;

public class AnimFinishEventArgs : EventArgs
{
    // Fields.
    public readonly AnimationFinishLocation FinishedLocation;


    // Constructors.
    public AnimFinishEventArgs(AnimationFinishLocation finishLocation) => FinishedLocation = finishLocation;
}