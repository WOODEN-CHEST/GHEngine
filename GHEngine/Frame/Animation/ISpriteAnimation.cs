using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Frame.Animation;

public interface ISpriteAnimation : IDisposable
{
    // Fields.
    Texture2D[] Frames { get; }
    int FrameCount { get; }
    int MaxFrameIndex { get; }
    double DefaultFPS { get; }
    int DefaultFrameStep { get; }
    bool DefaultIsLooped { get; }
    RectangleF? DefaultDrawRegion { get; }
    bool DefaultIsAnimating { get; }


    // Methods.
    IAnimationInstance CreateInstance();


    // Operators.
    Texture2D this[int index] { get; }
}