using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Frame;

public interface IFrameRenderer : IRenderer
{
    // Fields.
    BlendState RenderBlendState { get; set; }
    DepthStencilState RenderDepthStencilState { get; set; }
    RasterizerState RenderRasterizerState { get; set; }
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