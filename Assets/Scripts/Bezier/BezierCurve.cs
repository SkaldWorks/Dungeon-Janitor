using System.Collections.Generic;
using UnityEngine;

public static class BezierCurve
{
    public static Vector2 Evaluate(float t, List<Vector2> controlPoints)
    {
        if (controlPoints == null || controlPoints.Count == 0)
            return Vector2.zero;

        if (controlPoints.Count == 1)
            return controlPoints[0];

        // Keep t between 0 and 1.
        // 0 = start of curve, 1 = end of curve.
        t = Mathf.Clamp01(t);

        List<Vector2> points = new List<Vector2>(controlPoints);

        // De Casteljau's algorithm: repeatedly interpolates between neighboring points until only one point remains.
        while (points.Count > 1)
        {
            List<Vector2> next = new List<Vector2>();

            for (int i = 0; i < points.Count - 1; i++)
            {
                next.Add(
                    Vector2.Lerp(points[i], points[i + 1], t)
                );
            }

            points = next;
        }

        // This point is the position on the Bezier curve.
        return points[0];
    }

    public static List<Vector2> Sample(
        List<Vector2> controlPoints,
        int sampleCount = 100)
    {
        List<Vector2> result = new List<Vector2>();

        if (controlPoints == null || controlPoints.Count == 0)
            return result;

        if (controlPoints.Count == 1)
        {
            result.Add(controlPoints[0]);
            return result;
        }

        // Make sure we always have enough samples to draw a line.
        sampleCount = Mathf.Max(2, sampleCount);

        for (int i = 0; i < sampleCount; i++)
        {
            // Convert the sample number into a value from 0 to 1.
            float t = i / (float)(sampleCount - 1);

            // Calculate the curve position at that point.
            result.Add(Evaluate(t, controlPoints));
        }

        return result;
    }
}