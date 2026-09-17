using System.Collections.Generic;
using UnityEngine;

public class Bezier_Viz : MonoBehaviour
{
    public RectTransform curveArea;
    public GameObject pointPrefab;
    public UIBezierLine controlLine;
    public UIBezierLine curveLine;
    public int sampleCount = 100;

    [SerializeField]
    List<Vector2> startingPoints = new List<Vector2>
    {
        new Vector2(400, 0),
        new Vector2(200, 200),
        new Vector2(-400, -200)
    };

    List<GameObject> points = new List<GameObject>();

    void Awake()
    {
        for (int i = 0; i < startingPoints.Count; i++)
        {
            GameObject point = Instantiate(pointPrefab, curveArea);
            point.GetComponent<RectTransform>().anchoredPosition = startingPoints[i];

            Point_Viz viz = point.GetComponent<Point_Viz>();
            if (viz != null)
                viz.SetMovable(i != 0 && i != startingPoints.Count - 1);

            points.Add(point);
        }
    }

    void Update()
    {
        Refresh();
    }

    public void ResetPoints(Vector2 first, Vector2 last)
    {
        for (int i = 0; i < points.Count; i++)
        {
            Vector2 pos = i == 0
                ? first
                : i == points.Count - 1
                    ? last
                    : startingPoints[i];

            points[i].GetComponent<RectTransform>().anchoredPosition = pos;
        }

        Refresh();
    }

    public List<Vector2> GetControlPoints()
    {
        List<Vector2> result = new List<Vector2>();

        foreach (GameObject point in points)
            result.Add(point.GetComponent<RectTransform>().anchoredPosition);

        return result;
    }

    void Refresh()
    {
        List<Vector2> controlPoints = GetControlPoints();

        if (controlLine != null)
            controlLine.SetPoints(controlPoints);

        if (curveLine != null)
            curveLine.SetPoints(
                BezierCurve.Sample(controlPoints, sampleCount)
            );
    }
}