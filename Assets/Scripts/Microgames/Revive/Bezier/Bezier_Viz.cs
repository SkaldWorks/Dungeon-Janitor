using System.Collections.Generic;
using UnityEngine;

public class Bezier_Viz : MonoBehaviour
{
    [Header("UI")]
    public RectTransform curveArea;
    public GameObject pointPrefab;

    [Header("Player Curve")]
    [SerializeField]
    private List<Vector2> startingControlPoints = new List<Vector2>
    {
        new Vector2(400f, 0f),
        new Vector2(200f, 200f),
        new Vector2(-400f, -200f)
    };

    [Header("Lines")]
    public UIBezierLine controlLine;
    public UIBezierLine curveLine;

    [Header("Curve")]
    // How many points are used to draw the smooth Bezier curve.
    // Higher = smoother, but more calculations.
    public int sampleCount = 100;

    private readonly List<GameObject> pointObjects =
        new List<GameObject>();

    private void Start()
    {
        CreateControlPoints();
        Refresh();
    }

    private void Update()
    {
        // Rebuild the lines every frame so they follow the points.
        Refresh();
    }

    private void CreateControlPoints()
    {
        foreach (Vector2 position in startingControlPoints)
        {
            CreatePoint(position);
        }
    }

    private void CreatePoint(Vector2 position)
    {
        // Prevent creating more points than the Bezier code can handle.
        if (pointObjects.Count >= 18)
        {
            Debug.LogWarning(
                "Cannot have more than 18 control points.",
                this
            );

            return;
        }

        if (pointPrefab == null || curveArea == null)
            return;

        GameObject point =
            Instantiate(pointPrefab, curveArea);

        RectTransform rect =
            point.GetComponent<RectTransform>();

        rect.anchoredPosition = position;

        point.name =
            "ControlPoint_" + pointObjects.Count;

        pointObjects.Add(point);
    }


    private void Refresh()
    {
        // Read the current positions of the draggable control points.
        List<Vector2> points = GetControlPoints();

        if (points.Count < 2)
            return;

        // Draw straight lines connecting the control points.
        if (controlLine != null)
        {
            controlLine.SetPoints(points);
        }

        // Convert the control points into a bunch of points to make a Bezier curve.
        if (curveLine != null)
        {
            List<Vector2> curve =
                BezierCurve.Sample(
                    points,
                    sampleCount
                );

            curveLine.SetPoints(curve);
        }
    }

    public List<Vector2> GetControlPoints()
    {
        List<Vector2> points =
            new List<Vector2>();

        for (int i = 0; i < pointObjects.Count; i++)
        {
            RectTransform rect =
                pointObjects[i].GetComponent<RectTransform>();

            points.Add(rect.anchoredPosition);
        }

        return points;
    }
}