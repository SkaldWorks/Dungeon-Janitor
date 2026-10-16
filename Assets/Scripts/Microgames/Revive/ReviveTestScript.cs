
using System.Collections.Generic;
using UnityEngine;

public class ReviveTestScript : MonoBehaviour
{
    [Header("Existing Canvas Setup")]
    public Bezier_Viz sourceVisualizer;

    public RectTransform curveArea;
    public GameObject pointPrefab;
    public UIBezierLine controlLine;
    public UIBezierLine curveLine;

    [Header("Curve Settings")]
    public int sampleCount = 100;

    [Header("Initial Points (used if no points exist)")]

    [Header("Double-Click Settings")]
    public float doubleClickWindow = 0.3f;
    public float doubleClickMaxDistance = 24f;

    private readonly List<GameObject> points =
        new List<GameObject>();

    private Canvas canvas;

    private float lastClickTime = -1f;
    private Vector2 lastClickPosition;


    void Start()
    {
        if (sourceVisualizer == null)
            sourceVisualizer = FindObjectOfType<Bezier_Viz>();

        if (sourceVisualizer != null)
        {
            if (curveArea == null)
                curveArea = sourceVisualizer.curveArea;

            if (pointPrefab == null)
                pointPrefab = sourceVisualizer.pointPrefab;

            if (controlLine == null)
                controlLine = sourceVisualizer.controlLine;

            if (curveLine == null)
                curveLine = sourceVisualizer.curveLine;

            // Remove the points created by the original visualizer.
            sourceVisualizer.enabled = false;

            for (int i = curveArea.childCount - 1; i >= 0; i--)
            {
                Transform child = curveArea.GetChild(i);

                if (child.GetComponent<Point_Viz>() != null)
                    Destroy(child.gameObject);
            }
        }

        if (curveArea == null || pointPrefab == null)
        {
            Debug.LogError(
                "ReviveTestScript needs a Curve Area and Point Prefab."
            );

            enabled = false;
            return;
        }

        canvas = curveArea.GetComponentInParent<Canvas>();

        points.Clear();

        // Start with no points and empty lines.
        if (controlLine != null)
            controlLine.SetPoints(new List<Vector2>());

        if (curveLine != null)
            curveLine.SetPoints(new List<Vector2>());
    }

    void Update()
    {
        HandleDoubleClick();
        RefreshLines();
    }

    void FindExistingPoints()
    {
        points.Clear();

        for (int i = 0; i < curveArea.childCount; i++)
        {
            GameObject child = curveArea.GetChild(i).gameObject;

            if (child.GetComponent<Point_Viz>() != null)
            {
                points.Add(child);
            }
        }
    }

    void SetAllPointsMovable()
    {
        foreach (GameObject point in points)
        {
            if (point == null)
                continue;

            Point_Viz pointViz = point.GetComponent<Point_Viz>();

            if (pointViz != null)
                pointViz.SetMovable(true);
        }
    }

    void HandleDoubleClick()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Vector2 screenPosition = Input.mousePosition;
        Camera eventCamera = GetEventCamera();

        // Only accept clicks inside the curve area.
        if (!RectTransformUtility.RectangleContainsScreenPoint(
            curveArea, screenPosition, eventCamera))
        {
            lastClickTime = -1f;
            return;
        }

        // Clicking on an existing point should not add another point.
        if (IsOverExistingPoint(screenPosition, eventCamera))
        {
            lastClickTime = -1f;
            return;
        }

        float currentTime = Time.unscaledTime;

        bool isDoubleClick =
            currentTime - lastClickTime <= doubleClickWindow &&
            Vector2.Distance(
                screenPosition,
                lastClickPosition
            ) <= doubleClickMaxDistance;

        if (isDoubleClick)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                curveArea,
                screenPosition,
                eventCamera,
                out Vector2 localPosition))
            {
                CreatePoint(localPosition);
            }

            // Prevent one triple-click from adding multiple points.
            lastClickTime = -1f;
        }
        else
        {
            lastClickTime = currentTime;
            lastClickPosition = screenPosition;
        }
    }

    bool IsOverExistingPoint(
        Vector2 screenPosition,
        Camera eventCamera)
    {
        foreach (GameObject point in points)
        {
            if (point == null)
                continue;

            RectTransform rect = point.GetComponent<RectTransform>();

            if (rect != null &&
                RectTransformUtility.RectangleContainsScreenPoint(
                    rect, screenPosition, eventCamera))
            {
                return true;
            }
        }

        return false;
    }

    void CreatePoint(Vector2 position)
    {
        GameObject newPoint = Instantiate(
            pointPrefab,
            curveArea,
            false
        );

        RectTransform rect = newPoint.GetComponent<RectTransform>();

        if (rect == null)
        {
            Debug.LogError(
                "The point prefab must have a RectTransform."
            );

            Destroy(newPoint);
            return;
        }

        rect.anchoredPosition = position;

        Point_Viz pointViz = newPoint.GetComponent<Point_Viz>();

        if (pointViz == null)
            pointViz = newPoint.AddComponent<Point_Viz>();

        // Newly created points are always movable.
        pointViz.SetMovable(true);

        newPoint.transform.SetAsLastSibling();

        points.Add(newPoint);

        RefreshLines();
    }

    void RefreshLines()
    {
        if (curveArea == null)
            return;

        List<Vector2> controlPoints = new List<Vector2>();

        foreach (GameObject point in points)
        {
            if (point == null)
                continue;

            RectTransform rect = point.GetComponent<RectTransform>();

            if (rect != null)
                controlPoints.Add(rect.anchoredPosition);
        }

        if (controlLine != null)
            controlLine.SetPoints(controlPoints);

        if (curveLine != null)
        {
            curveLine.SetPoints(
                BezierCurve.Sample(
                    controlPoints,
                    Mathf.Max(2, sampleCount)
                )
            );
        }
    }

    Camera GetEventCamera()
    {
        if (canvas == null ||
            canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            return null;
        }

        return canvas.worldCamera != null
            ? canvas.worldCamera
            : Camera.main;
    }
}
