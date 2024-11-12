using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Frame.Animation;

public interface IAnimationInstance : ITimeUpdatable
{
    // Fields.
    ISpriteAnimation Source { get; }
    RectangleF? DrawRegion { get; set; }
    public bool IsAnimating { get; set; }
    double FPS { get; set; }
    int FrameStep { get; set; }
    bool IsLooped { get; set; }
    int FrameIndex { get; set; }
    public Texture2D CurrentFrame { get; }

    public event EventHandler<AnimFinishEventArgs>? AnimationFinished;


    // Methods.
    public void Reset();

    public IAnimationInstance CreateClone();
}
