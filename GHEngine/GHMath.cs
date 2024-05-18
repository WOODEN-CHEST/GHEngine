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
        return DVector2.Dot(vector1, vector2) / (vector1.Length * vector2.Length);
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
        if ((vector.LengthSquared == 0f) || (vector.LengthSquared == -0f))
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

    public static bool IsPointInArea(Vector2 point, Vector2 areaCorner1, Vector2 areaCorner2, float areaRotation)
    {
        float MinX = Math.Min(areaCorner1.X, areaCorner2.X);
        float MinY = Math.Min(areaCorner1.Y, areaCorner2.Y);
        float MaxX = Math.Max(areaCorner1.X, areaCorner2.X);
        float MaxY = Math.Max(areaCorner1.Y, areaCorner2.Y);

        if (areaRotation == 0f)
        {
            return (MinX <= point.X) && (point.X <= MaxX) && (MinY <= point.Y) && (point.Y <= MaxY);
        }

        Vector2 MiddleAreaPoint = new(MinX + ((MaxX - MinX) * 0.5f), MinY + ((MaxY - MinY) * 0.5f));
        Vector2 RotatedPoint = MiddleAreaPoint + Vector2.Transform(point - MiddleAreaPoint, Matrix.CreateRotationZ(-areaRotation));
        return (MinX <= RotatedPoint.X) && (RotatedPoint.X <= MaxX) && (MinY <= RotatedPoint.Y) && (RotatedPoint.Y <= MaxY);
    }

    public static bool IsPointInArea(DVector2 point, DVector2 areaCorner1, DVector2 areaCorner2, float areaRotation)
    {
        double MinX = Math.Min(areaCorner1.X, areaCorner2.X);
        double MinY = Math.Min(areaCorner1.Y, areaCorner2.Y);
        double MaxX = Math.Max(areaCorner1.X, areaCorner2.X);
        double MaxY = Math.Max(areaCorner1.Y, areaCorner2.Y);

        if (areaRotation == 0d)
        {
            return (MinX <= point.X) && (point.X <= MaxX) && (MinY <= point.Y) && (point.Y <= MaxY);
        }

        DVector2 MiddleAreaPoint = new(MinX + ((MaxX - MinX) * 0.5d), MinY + ((MaxY - MinY) * 0.5d));
        DVector2 RotatedPoint = MiddleAreaPoint + DVector2.Rotate(point - MiddleAreaPoint, -areaRotation);
        return (MinX <= RotatedPoint.X) && (RotatedPoint.X <= MaxX) && (MinY <= RotatedPoint.Y) && (RotatedPoint.Y <= MaxY);
    }
}