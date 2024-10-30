using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine;

public struct DVector2
{
    // Static fields.
    public static DVector2 UnitX { get; } = new DVector2(1d, 0d);
    public static DVector2 UnitY { get; } = new DVector2(0d, 1d);
    public static DVector2 One { get; } = new DVector2(1d, 1d);
    public static DVector2 Zero { get; } = new DVector2(0d, 0d);


    // Fields.
    public double X { get; set; }
    public double Y { get; set; }
    public readonly double Length => Math.Sqrt((X* X) + (Y* Y));
    public readonly double LengthSquared => (X* X) + (Y* Y);


    // Constructors.
    public DVector2(double x, double y)
    {
        X = x;
        Y = y;
    }

    public DVector2(double value)
    {
        X = value;
        Y = value;
    }

    public DVector2(DVector2 value)
    {
        X = value.X;
        Y = value.Y;
    }

    public DVector2(Vector2 value)
    {
        X = value.X;
        Y = value.Y;
    }


    // Static methods.
    public static double Dot(DVector2 a, DVector2 b)
    {
        return (a.X * b.X) + (a.Y * b.Y);
    }

    public static DVector2 Normalize(DVector2 vector)
    {
        return vector / vector.Length;
    }

    public static DVector2 NormalizeOrDefault(DVector2 vector2, DVector2 defaultVector)
    {
        return GHMath.NormalizeOrDefault(vector2, defaultVector);
    }

    public static DVector2 Interpolate(DVector2 vector1, DVector2 vector2, double amount)
    {
        return GHMath.Interpolate(vector1, vector2, amount);
    }

    public static DVector2 Rotate(DVector2 vector, double angleRad)
    {
        double SinAngle = Math.Sin(angleRad);
        double CosAngle = Math.Cos(angleRad);

        DVector2 NewVector = new(vector);
        NewVector.X = (vector.X * CosAngle) + (vector.Y * (-SinAngle));
        NewVector.Y = (vector.X * SinAngle) + (vector.Y * CosAngle);
        return NewVector;
    }

    public static DVector2 Transform(DVector2 vector, Matrix matrix)
    {
        DVector2 NewVector = vector;
        NewVector.X = (vector.X * matrix.M11) + (vector.Y * matrix.M12);
        NewVector.Y = (vector.X * matrix.M21) + (vector.Y * matrix.M22);
        return NewVector;
    }

    public static double Distance(DVector2 a, DVector2 b)
    {
        return (b - a).Length;
    }

    public static double DistanceSquared(DVector2 a, DVector2 b)
    {
        return (b - a).LengthSquared;
    }


    // Methods.
    public void Normalize()
    {
        double VectorLength = Length;
        X /= VectorLength;
        Y /= VectorLength;
    }

    public void NormalizeOrDefault(DVector2 defaultVector)
    {
        if ((LengthSquared == 0f) || (LengthSquared == -0f))
        {
            X = defaultVector.X;
            Y = defaultVector.Y;
        }
        else
        {
            Normalize();
        }
    }


    // Inherited methods.
    public readonly override bool Equals(object? obj)
    {
        if (obj is DVector2 DVector)
        {
            return (DVector == this);
        }
        return false;
    }

    public readonly override int GetHashCode()
    {
        return X.GetHashCode() + Y.GetHashCode();
    }

    public readonly override string ToString()
    {
        return $"({X}; {Y})";
    }


    // Operator.
    public static DVector2 operator +(DVector2 a, DVector2 b)
    {
        return new DVector2((a.X + b.X), (a.Y + b.Y));
    }

    public static DVector2 operator +(DVector2 a, double value)
    {
        return new DVector2((a.X + value), (a.Y + value));
    }

    public static DVector2 operator -(DVector2 a, DVector2 b)
    {
        return new DVector2((a.X - b.X), (a.Y - b.Y));
    }

    public static DVector2 operator -(DVector2 a, double value)
    {
        return new DVector2((a.X - value), (a.Y - value));
    }

    public static DVector2 operator *(DVector2 a, DVector2 b)
    {
        return new DVector2((a.X * b.X), (a.Y * b.Y));
    }

    public static DVector2 operator *(DVector2 a, double value)
    {
        return new DVector2((a.X * value), (a.Y * value));
    }

    public static DVector2 operator /(DVector2 a, DVector2 b)
    {
        return new DVector2((a.X / b.X), (a.Y / b.Y));
    }

    public static DVector2 operator /(DVector2 a, double value)
    {
        return new DVector2((a.X / value), (a.Y / value));
    }

    public static bool operator ==(DVector2 a, DVector2 b)
    {
        return (a.X == b.X) && (a.Y == b.Y);
    }

    public static bool operator !=(DVector2 a, DVector2 b)
    {
        return (a.X != b.X) || (a.Y != b.Y);
    }

    public static implicit operator Vector2(DVector2 vector)
    {
        return new Vector2((float)vector.X, (float)vector.Y);
    }
}
