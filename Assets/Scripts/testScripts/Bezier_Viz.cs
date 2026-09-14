using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Bezier_Viz : MonoBehaviour
{

    public RectTransform canvas;
    public GameObject pointPrefab;
    public List<Vector2> controlPoints = new List<Vector2>()
    {
        new Vector2(400.0f, 0.0f),
        new Vector2(200.0f, 200.0f),
        new Vector2(-400.0f, -200.0f)
    };

    LineRenderer[] mLineRenderers = null;

    List<GameObject> mPointGameObjects = new List<GameObject>();

    public float LineWidth;
    public float LineWidthBezier;
    public Color LineColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);
    public Color BezierCurveColor = new Color(0.5f, 0.6f, 0.8f, 0.8f);

    private LineRenderer CreateLine()
    {
        GameObject obj = new GameObject("LineRenderer");

        // Make the line part of the Canvas hierarchy
        obj.transform.SetParent(canvas, false);

        LineRenderer lr = obj.AddComponent<LineRenderer>();

        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = LineColor;
        lr.endColor = LineColor;
        lr.startWidth = LineWidth;
        lr.endWidth = LineWidth;

        // Make sure it renders above/below your UI as desired
        lr.sortingOrder = 0;

        return lr;
    }

    void Start()
    {
        //create lines
        mLineRenderers = new LineRenderer[2];
        mLineRenderers[0] = CreateLine();
        mLineRenderers[1] = CreateLine();

        //name lines
        mLineRenderers[0].name = "LineRenderer_obj_0";
        mLineRenderers[1].name = "LineRenderer_obj_1";

        //control point instances
        for (int i = 0; i < controlPoints.Count; i++)
        {
            GameObject obj = Instantiate(pointPrefab, canvas);
            obj.GetComponent<RectTransform>().anchoredPosition = controlPoints[i];
            obj.name = "ControlPoint_" + i.ToString();
            mPointGameObjects.Add(obj);
        }
    }
    void Update()
    {
        LineRenderer lineRenderer = mLineRenderers[0];
        LineRenderer curveRenderer = mLineRenderers[1];

        List<Vector2> pts = new List<Vector2>();
        for (int i = 0; i < mPointGameObjects.Count; i++)
        {
            pts.Add(mPointGameObjects[i].GetComponent<RectTransform>().anchoredPosition);        
        }
        //set line renderer for strainght lines between control points
        lineRenderer.positionCount = pts.Count;
        for (int i = 0; i < mPointGameObjects.Count; i++)
        {
            RectTransform rect = mPointGameObjects[i].GetComponent<RectTransform>();
            pts.Add(rect.anchoredPosition);
        }

        //draw bezier curve
        List<Vector2> curve = BezierCurve.PointList2(pts, 0.01f);
        curveRenderer.positionCount = curve.Count;
        curveRenderer.startColor = BezierCurveColor;
        curveRenderer.endColor = BezierCurveColor;
        curveRenderer.startWidth = LineWidthBezier;
        curveRenderer.endWidth = LineWidthBezier;

        for (int i = 0; i < curve.Count; i++)
        {
            curveRenderer.SetPosition(i, curve[i]);
        }
    }
    private void OnGUI()
    {
        Event e = Event.current;
        if (e.isMouse)
        {
            if (e.clickCount == 2 && e.button == 0)
            {
                Vector2 rayPos = new Vector2(
                    Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                    Camera.main.ScreenToWorldPoint(Input.mousePosition).y);

                InsertNewControlPoint(rayPos);
            }
        }
    }

    void InsertNewControlPoint(Vector2 p)
    {
        if (mPointGameObjects.Count >= 18)
        {
            Debug.Log  ("Cannot create more points, Max is 18");
            return;
        }
        GameObject obj = Instantiate(pointPrefab, canvas);
        obj.GetComponent<RectTransform>().anchoredPosition = p;
        obj.name = "ControlPoint_" + mPointGameObjects.Count.ToString();
        mPointGameObjects.Add(obj);
    }

    public List<Vector2> GetControlPoints()
    {
        List<Vector2> points = new List<Vector2>();

        for (int i = 0; i < mPointGameObjects.Count; i++)
        {
            RectTransform rect =
                mPointGameObjects[i].GetComponent<RectTransform>();

            points.Add(rect.anchoredPosition);
        }

        return points;
    }
}
