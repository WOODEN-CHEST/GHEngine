using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Screen;

public class ScreenSizeChangeEventArgs : EventArgs
{
    // Fields.
    public WindowSize NewSize { get; private init; }


    // Constructors.
    public ScreenSizeChangeEventArgs(WindowSize newSize)
    {
        NewSize = newSize;
    }
}