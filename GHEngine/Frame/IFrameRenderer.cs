using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Frame;

public interface IFrameRenderer : IRenderer
{
    // Fields.
    Color? ScreenColor { get; set; }
    int DrawCallsTotal { get; }
    int DrawCallsSprite { get; }
    int DrawCallsString { get; }
    int DrawCallsCharacter { get; }
    int DrawCallsLine { get; }
    int DrawCallsRectangle { get; }
    int RenderTargetSwitchCount { get; }
    int SpriteBatchBeginCount { get; }


    // Methods.
    void RenderFrame(IGameFrame frameToDraw, IProgramTime time);
}