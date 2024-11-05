using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine;

public readonly struct LimitedDRay
{
    // Private static fields.
    private const double MAX_MULTIPLIER = 1_000_000d;


    // Fields.
    public double Addition { get; }
    public double Multiplier { get; }


    // Constructors.
    public LimitedDRay(double addition, double multiplier)
    {
        Addition = addition;
        Multiplier = multiplier;
    }

    public LimitedDRay(DEdge edge) : this(edge.Vertex1, edge.Vertex2) { }

    public LimitedDRay(DVector2 vertex1, DVector2 vertex2)
    {
        double DeltaY = vertex2.Y - vertex1.Y;
        double DeltaX = vertex2.X - vertex1.X;

        double Mult = DeltaY / DeltaX;
        if (double.IsNaN(Mult) || double.IsInfinity(DeltaY / DeltaX) || (Math.Abs(Mult) > MAX_MULTIPLIER))
        {
            Multiplier = MAX_MULTIPLIER;
        }
        else
        {
            Multiplier = Mult;
        }

        Addition = vertex1.Y - (vertex1.X * Multiplier);
    }


    // Methods.
    public double ValueAt(double x)
    {
        return (x * Multiplier) + Addition;
    }

    public double ArgumentAt(double y)
    {
        return (y - Addition) / Multiplier;
    }


    // Inherited methods.
    public override bool Equals(object? obj)
    {
        if (obj is LimitedDRay Ray)
        {
            return (Addition == Ray.Addition) && (Multiplier == Ray.Multiplier);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Addition.GetHashCode() * Multiplier.GetHashCode() - Multiplier.GetHashCode();
    }

    public override string ToString()
    {
        return $"{Multiplier}X {(Addition > 0d ? '+' : '-')} {Math.Abs(Addition)}";
    }


    // Static methods.
    public static DVector2? GetIntersection(LimitedDRay a, LimitedDRay b)
    {
        if (a.Multiplier == b.Multiplier)
        {
            return a.Addition == b.Addition ? new DVector2(0d, a.ValueAt(0d)) : null;
        }

        double X = (b.Addition - a.Addition) / (a.Multiplier - b.Multiplier);
        double Y = a.ValueAt(X);
        return new DVector2(X, Y);
    }

    public static LimitedDRay GetPerpendicular(LimitedDRay ray)
    {
        double Multiplier = -(1d / ray.Multiplier);
        if (double.IsNaN(Multiplier) || double.IsInfinity(Multiplier) || (Math.Abs(Multiplier) > MAX_MULTIPLIER))
        {
            Multiplier = MAX_MULTIPLIER;
        }
        return new LimitedDRay(ray.Addition, Multiplier);
    }
}