using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Frame;

public interface IFrameRenderer : IRenderer
{
    void RenderFrame(IGameFrame frameToDraw, IProgramTime time);
    void Initialize();
}