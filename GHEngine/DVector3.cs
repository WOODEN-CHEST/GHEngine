using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine;

public class DVector3
{
    // Fields.
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    // Constructors.
    public DVector3(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public DVector3(double value)
    {
        X = value;
        Y = value;
        Z = value;
    }

    public DVector3(DVector3 value)
    {
        X = value.X;
        Y = value.Y;
        Z = value.Z;
    }

    public DVector3(Vector3 value)
    {
        X = value.X;
        Y = value.Y;
        Z = value.Z;
    }


    // Static methods.
    public static double Dot(DVector3 a, DVector3 b)
    {
        return (a.X * b.X) + (a.Y * b.Y) + (a.Z * b.Z);
    }

    public static DVector3 Normalize(DVector3 vector)
    {
        return vector / vector.Length();
    }

    public static DVector3 NormalizeOrDefault(DVector3 vector2, DVector3 defaultVector)
    {
        return GHMath.NormalizeOrDefault(vector2, defaultVector);
    }

    public static DVector3 Interpolate(DVector3 vector1, DVector3 vector2, double amount)
    {
        return GHMath.Interpolate(vector1, vector2, amount);
    }

    public static DVector3 Transform(DVector3 vector, Matrix matrix)
    {
        DVector3 NewVector = vector;

        NewVector.X = (vector.X * matrix.M11) + (vector.Y * matrix.M12) + (vector.Z * matrix.M13);
        NewVector.Y = (vector.X * matrix.M21) + (vector.Y * matrix.M22) + (vector.Z * matrix.M23);
        NewVector.Z = (vector.X * matrix.M31) + (vector.Y * matrix.M32) + (vector.Z * matrix.M33);

        return NewVector;
    }


    // Methods.
    public void Normalize()
    {
        double VectorLength = Length();
        X /= VectorLength;
        Y /= VectorLength;
        Z /= VectorLength;
    }

    public void NormalizeOrDefault(DVector3 defaultVector)
    {
        if ((LengthSquared() == 0f) || (LengthSquared() == -0f))
        {
            X = defaultVector.X;
            Y = defaultVector.Y;
            Z = defaultVector.Z;
        }
        else
        {
            Normalize();
        }
    }

    public double Length()
    {
        return Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
    }

    public double LengthSquared()
    {
        return (X * X) + (Y * Y) + (Z * Z);
    }

    // Inherited methods.
    public override bool Equals(object? obj)
    {
        if (obj is DVector3 DVector)
        {
            return (DVector == this);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode();
    }

    public override string ToString()
    {
        return $"({X}; {Y}; {Z})";
    }


    // Operator.
    public static DVector3 operator +(DVector3 a, DVector3 b)
    {
        return new DVector3((a.X + b.X), (a.Y + b.Y), (a.Z + b.Z));
    }

    public static DVector3 operator +(DVector3 a, double value)
    {
        return new DVector3((a.X + value), (a.Y + value), (a.Z + value));
    }

    public static DVector3 operator -(DVector3 a, DVector3 b)
    {
        return new DVector3((a.X - b.X), (a.Y - b.Y), (a.Z - b.Z));
    }

    public static DVector3 operator -(DVector3 a, double value)
    {
        return new DVector3((a.X - value), (a.Y - value), (a.Z - value));
    }

    public static DVector3 operator *(DVector3 a, DVector3 b)
    {
        return new DVector3((a.X * b.X), (a.Y * b.Y), (a.Z * b.Z));
    }

    public static DVector3 operator *(DVector3 a, double value)
    {
        return new DVector3((a.X * value), (a.Y * value), (a.Z * value));
    }

    public static DVector3 operator /(DVector3 a, DVector3 b)
    {
        return new DVector3((a.X / b.X), (a.Y / b.Y), (a.Z / b.Z));
    }

    public static DVector3 operator /(DVector3 a, double value)
    {
        return new DVector3((a.X / value), (a.Y / value), (a.Z / value));
    }

    public static bool operator ==(DVector3 a, DVector3 b)
    {
        return (a.X == b.X) && (a.Y == b.Y) && (a.Z == b.Z);
    }

    public static bool operator !=(DVector3 a, DVector3 b)
    {
        return (a.X != b.X) || (a.Y != b.Y) || (a.Z != b.Z);
    }

    public static implicit operator Vector3(DVector3 vector)
    {
        return new Vector3((float)vector.X, (float)vector.Y, (float)vector.Z);
    }
}