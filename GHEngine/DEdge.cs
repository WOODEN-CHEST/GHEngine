using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine;

public class DEdge
{
    // Fields.
    public DVector2 Vertex1 { get; }
    public DVector2 Vertex2 { get; }
    public DVector2 Vector => Vertex2 - Vertex1;
    public DVector2 Normal => DVector2.NormalizeOrDefault(GHMath.PerpendicularVectorCounterClockwise(Vector), DVector2.UnitY);
    public double Length => Vector.Length;


    // Constructors.
    public DEdge(DVector2 vertex1, DVector2 vertex2)
    {
        Vertex1 = vertex1;
        Vertex2 = vertex2;
    }

    public DEdge(DEdge edge)
    {
        Vertex1 = edge.Vertex1;
        Vertex2 = edge.Vertex2;
    }


    // Methods.
    public bool IsPointInEdgesArea(DVector2 point)
    {
        return GHMath.IsPointInArea(point, Vertex1, Vertex2, 0d);
    }


    // Inherited methods.
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is DEdge EdgeObj)
        {
            return (EdgeObj.Vertex1 == Vertex1) && (EdgeObj.Vertex2 == Vertex2);
        }
        return false;
    }

    public override string ToString()
    {
        return $"[({Vertex1.X};{Vertex1.Y});({Vertex2.X};{Vertex2.Y})]";
    }

    public override int GetHashCode()
    {
        return Vertex1.GetHashCode() * Vertex2.GetHashCode() + 1142908;
    }


    // Operators.
    public static explicit operator LimitedDRay(DEdge edge) => new LimitedDRay(edge);
}