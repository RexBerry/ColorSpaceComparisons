using System.Numerics;

namespace ColorSpaceComparisons.Utils;

public static class GeometryUtils
{
    public static bool RayLineSegmentIntersection(
        Vector2 rayOrigin,
        Vector2 rayDirection,
        Vector2 lineSegmentEndpoint1,
        Vector2 lineSegmentEndpoint2,
        out Vector2 intersectionPoint
    )
    {
        const float Tolerance = 1e-5f;

        var intersects =
            LineLineIntersection(
                rayOrigin,
                rayDirection,
                lineSegmentEndpoint1,
                lineSegmentEndpoint2 - lineSegmentEndpoint1,
                out var t1,
                out var t2
            )
            && t1 > -Tolerance
            && t2 > -Tolerance
            && t2 < 1 + Tolerance;

        intersectionPoint = rayOrigin + t1 * rayDirection;
        return intersects;
    }

    private static bool LineLineIntersection(
        Vector2 origin1,
        Vector2 direction1,
        Vector2 origin2,
        Vector2 direction2,
        out float t1,
        out float t2
    )
    {
        var perpVec2 = new Vector2(direction2.Y, -direction2.X);
        var denominator = Vector2.Dot(
            direction1,
            new Vector2(direction2.Y, -direction2.X)
        );
        if (denominator == 0f)
        {
            t1 = t2 = 0f;
            return false;
        }

        var originDiff = origin2 - origin1;

        var perpVec1 = new Vector2(direction1.Y, -direction1.X);
        t1 = Vector2.Dot(originDiff, perpVec2) / denominator;
        t2 = Vector2.Dot(originDiff, perpVec1) / denominator;
        return true;
    }
}
