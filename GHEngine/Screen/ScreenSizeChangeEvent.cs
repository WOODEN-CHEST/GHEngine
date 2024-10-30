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
    public IntVector NewSize { get; private init; }


    // Constructors.
    public ScreenSizeChangeEventArgs(IntVector newSize)
    {
        NewSize = newSize;
    }
}