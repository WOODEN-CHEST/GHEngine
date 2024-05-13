using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Frame.Animation;

public interface IAnimationInstance : ITimeUpdatable, ICloneable
{
    // Fields.
    ISpriteAnimation Animation { get; }
    Rectangle? DrawRegion { get; set; }
    public bool IsAnimating { get; set; }
    double FPS { get; set; }
    int FrameStep { get; set; }
    bool IsLooped { get; set; }
    int FrameIndex { get; set; }

    public event EventHandler<AnimFinishEventArgs>? AnimationFinished;


    // Methods.
    public Texture2D GetCurrentFrame();

    public void Reset();
}
