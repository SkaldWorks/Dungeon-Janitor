using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve
{
    //lookup table
    private static float[] Factorial = new float[]
    {
        1.0f,
        1.0f,
        2.0f,
        6.0f,
        24.0f,
        120.0f,
        720.0f,
        5040.0f,
        40320.0f,
        362880.0f,
        3628800.0f,
        39916800.0f,
        479001600.0f,
        6227020800.0f,
        87178291200.0f,
        1307674368000.0f,
        20922789888000.0f,
        355687428096000.0f,
        6402373705728000.0f,
    };

    //binomial function from binomial equation
    private static float Binomial(int n, int i)
    {
        float ni;
        float a1 = Factorial[n];
        float a2 = Factorial[i];
        float a3 = Factorial[n - i];
        ni = a1 / (a2 * a3);
        return ni;
    }

    //Bernstein basis points
    private static float Bernstein(int n, int i, float t)
    {
        float t_i = Mathf.Pow(t, i);
        float t_n_minus_i = Mathf.Pow(1 - t, n - i);
        float basis = Binomial(n, i) * t_i * t_n_minus_i;
        return basis;
    }

    //bezier point function
    //returns bezier point
    //t is between 0 and 1

    public static Vector3 Point3(float t, List<Vector3> controlPoints)
    {
        int N = controlPoints.Count - 1; //degree of bezier curve
        if (N > 18)
        {
            Debug.LogError("Bezier curve degree is too high. Max degree is 18.");
            controlPoints.RemoveRange(18, controlPoints.Count - 18);
        }

        if(t <= 0) return controlPoints[0];
        if(t >= 1) return controlPoints[controlPoints.Count - 1];

        Vector3 p = new Vector3();

        for (int i = 0; i < controlPoints.Count; ++i)
        {
            Vector3 bn = Bernstein(N, i, t) * controlPoints[i];
            p += bn;
        }
        return p;
    }

    //calculate points for curve
    public static List<Vector3> PointList3(List<Vector3> controlPoints, float interval = 0.01f)
    {
        int N = controlPoints.Count - 1; //degree of bezier curve
        if (N > 18)
        {
            Debug.LogError("Bezier curve degree is too high. Max degree is 18.");
            controlPoints.RemoveRange(18, controlPoints.Count - 18);
        }
    List<Vector3> curvePoints = new List<Vector3>();

        for (float t = 0.0f; t <= 1.0f + interval - 0.0001f; t += interval)
        {
            Vector3 p = new Vector3();
            for (int i = 0; i < controlPoints.Count; ++i)
            {
                Vector3 bn = Bernstein(N, i, t) * controlPoints[i];
                p += bn;
            }
            curvePoints.Add(p);
        }
        return curvePoints;
    }


// do the same for Vector2 (for 2d points)


    public static Vector2 Point2(float t, List<Vector2> controlPoints)
    {
        int N = controlPoints.Count - 1; //degree of bezier curve
        if (N > 18)
        {
            Debug.LogError("Bezier curve degree is too high. Max degree is 18.");
            controlPoints.RemoveRange(18, controlPoints.Count - 18);
        }

        if(t <= 0) return controlPoints[0];
        if(t >= 1) return controlPoints[controlPoints.Count - 1];

        Vector2 p = new Vector2();

        for (int i = 0; i < controlPoints.Count; ++i)
        {
            Vector2 bn = Bernstein(N, i, t) * controlPoints[i];
            p += bn;
        }
        return p;
    }

    //calculate points for curve
    public static List<Vector2> PointList2(List<Vector2> controlPoints, float interval = 0.01f)
    {
        int N = controlPoints.Count - 1; //degree of bezier curve
        if (N > 18)
        {
            Debug.LogError("Bezier curve degree is too high. Max degree is 18.");
            controlPoints.RemoveRange(18, controlPoints.Count - 18);
        }
    List<Vector2> curvePoints = new List<Vector2>();

        for (float t = 0.0f; t <= 1.0f + interval - 0.0001f; t += interval)
        {
            Vector2 p = new Vector2();
            for (int i = 0; i < controlPoints.Count; ++i)
            {
                Vector2 bn = Bernstein(N, i, t) * controlPoints[i];
                p += bn;
            }
            curvePoints.Add(p);
        }
        return curvePoints;
    }

}
