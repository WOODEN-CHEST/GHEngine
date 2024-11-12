using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Frame;

public interface IFrameRenderer : IRenderer
{
    Color? ScreenColor { get; set; }
    void RenderFrame(IGameFrame frameToDraw, IProgramTime time);
}