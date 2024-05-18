using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Frame;

public class GameFrameLoadArgs : EventArgs
{
    // Fields.
    public IGameFrame Frame { get; }


    // Constructors.
    public GameFrameLoadArgs(IGameFrame frame)
    {
        Frame = frame ?? throw new ArgumentNullException(nameof(frame));
    }
}