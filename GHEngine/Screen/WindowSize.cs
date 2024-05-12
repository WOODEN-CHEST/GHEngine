using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Screen;

public struct WindowSize
{
    public int X { get; set; }
    public int Y { get; set; }


    // Constructors.
    public WindowSize(int x, int y)
    {
        X = x;
        Y = y;
    }


    // Operators.
    public static bool operator ==(WindowSize a, WindowSize b)
    {
        return (a.X == b.X) && (a.Y == b.Y);
    }

    public static bool operator !=(WindowSize a, WindowSize b)
    {
        return (a.X != b.X) || (a.Y != b.Y);
    }

    public static implicit operator Vector2(WindowSize windowSize)
    {
        return new Vector2(windowSize.X, windowSize.Y);
    }
}