// Ignore Spelling: Interp

using Microsoft.Xna.Framework;
using System;
using System.Runtime.CompilerServices;

namespace GHEngine;


public static class GHMath
{
    // Static methods.
    public static Vector2 Interpolate(Vector2 start, Vector2 end, float amount)
    {
        return start + ((end - start) * amount);
    }

    public static DVector2 Interpolate(DVector2 start, DVector2 end, double amount)
    {
        return start + ((end - start) * amount);
    }

    public static DVector3 Interpolate(DVector3 start, DVector3 end, double amount)
    {
        return start + ((end - start) * amount);
    }

    public static float Interpolate(float start, float end, float amount)
    {
        return start + ((end - start) * amount);
    }

    public static double Interpolate(double start, double end, double amount)
    {
        return start + ((end - start) * amount);
    }

    public static Vector2 PerpendicularVectorCounterClockwise(Vector2 vector)
    {
        return new Vector2(vector.Y, -vector.X);
    }

    public static DVector2 PerpendicularVectorCounterClockwise(DVector2 vector)
    {
        return new DVector2(vector.Y, -vector.X);
    }

    public static Vector2 PerpendicularVectorClockwise(Vector2 vector)
    {
        return new Vector2(-vector.Y, vector.X);
    }

    public static DVector2 PerpendicularVectorClockwise(DVector2 vector)
    {
        return new DVector2(-vector.Y, vector.X);
    }

    public static float CosineBetweenVectors(Vector2 vector1, Vector2 vector2)
    {
        return Vector2.Dot(vector1, vector2) / (vector1.Length() * vector2.Length());
    }

    public static double CosineBetweenVectors(DVector2 vector1, DVector2 vector2)
    {
        return DVector2.Dot(vector1, vector2) / (vector1.Length() * vector2.Length());
    }

    public static Vector2 NormalizeOrDefault(Vector2 vector, Vector2 defaultVector)
    {
        if ((vector.LengthSquared() == 0f) || (vector.LengthSquared() == -0f))
        {
            return defaultVector;
        }
        return Vector2.Normalize(vector);
    }

    public static DVector2 NormalizeOrDefault(DVector2 vector, DVector2 defaultVector)
    {
        if ((vector.LengthSquared() == 0f) || (vector.LengthSquared() == -0f))
        {
            return defaultVector;
        }
        return DVector2.Normalize(vector);
    }

    public static DVector3 NormalizeOrDefault(DVector3 vector, DVector3 defaultVector)
    {
        if ((vector.LengthSquared() == 0f) || (vector.LengthSquared() == -0f))
        {
            return defaultVector;
        }
        return DVector3.Normalize(vector);
    }

}