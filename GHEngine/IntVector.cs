using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine;

public struct IntVector
{
    // Static fields.
    public static IntVector Zero { get; } = new(0, 0);
    public static IntVector One { get; } = new(1, 1);
    public static IntVector UnitX { get; } = new(1, 0);
    public static IntVector UnitY { get; } = new(0, 1);


    // Fields.
    public int X { get; set; }
    public int Y { get; set; }


    // Constructors.
    public IntVector(int x, int y)
    {
        X = x;
        Y = y;
    }


    // Inherited methods.
    public override bool Equals(object? obj)
    {
        if (obj is IntVector Vector)
        {
            return (Vector.X == X) && (Vector.Y == Y);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return X.GetHashCode() + Y.GetHashCode();
    }

    public override string ToString()
    {
        return $"({X};{Y})";
    }


    // Operators.
    public static bool operator ==(IntVector a, IntVector b)
    {
        return (a.X == b.X) && (a.Y == b.Y);
    }

    public static bool operator !=(IntVector a, IntVector b)
    {
        return (a.X != b.X) || (a.Y != b.Y);
    }

    public static IntVector operator +(IntVector a, IntVector b)
    {
        return new(a.X + b.X, a.Y + b.Y);
    }

    public static IntVector operator -(IntVector a, IntVector b)
    {
        return new(a.X - b.X, a.Y - b.Y);
    }

    public static IntVector operator *(IntVector a, IntVector b)
    {
        return new(a.X * b.X, a.Y * b.Y);
    }
    public static IntVector operator *(IntVector vector, int value)
    {
        return new(vector.X * value, vector.Y * value);
    }

    public static IntVector operator /(IntVector a, IntVector b)
    {
        return new(a.X / b.X, a.Y / b.Y);
    }

    public static IntVector operator /(IntVector vector, int value)
    {
        return new(vector.X / value, vector.Y / value);
    }

    public static explicit operator Vector2(IntVector vector) => new(vector.X, vector.Y);

    public static explicit operator DVector2(IntVector vector) => new(vector.X, vector.Y);
}