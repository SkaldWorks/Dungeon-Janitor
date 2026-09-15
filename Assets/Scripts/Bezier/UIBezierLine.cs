using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UIBezierLine : MaskableGraphic
{
    public float lineWidth = 5f;

    public float LineWidth
    {
        get => lineWidth;

        set
        {
            lineWidth = value;
            SetVerticesDirty();
        }
    }

    private List<Vector2> curvePoints =
        new List<Vector2>();

    public void SetPoints(List<Vector2> points)
    {
        if (points == null)
        {
            SetVerticesDirty();
            return;
        }

        curvePoints = points;
        SetVerticesDirty();
    }

    protected override void Awake()
    {
        base.Awake();

        raycastTarget = false;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (curvePoints == null || curvePoints.Count < 2)
            return;

        float halfWidth = lineWidth * 0.5f;

        // Create two vertices at every point:
        for (int i = 0; i < curvePoints.Count; i++)
        {
            Vector2 point = curvePoints[i];

            Vector2 direction;

            if (i == 0)
            {
                // First point: use the direction toward the next point.
                direction =
                    curvePoints[1] - curvePoints[0];
            }
            else if (i == curvePoints.Count - 1)
            {
                // Last point: use the direction from the previous point.
                direction =
                    curvePoints[i] - curvePoints[i - 1];
            }
            else
            {
                // Middle point: use both neighboring directions.
                Vector2 directionToNext =
                    curvePoints[i + 1] - curvePoints[i];

                Vector2 directionFromPrevious =
                    curvePoints[i] - curvePoints[i - 1];

                direction =
                    directionToNext.normalized +
                    directionFromPrevious.normalized;
            }

            if (direction.sqrMagnitude < 0.0001f)
                direction = Vector2.right;

            direction.Normalize();

            // Get a vector perpendicular to the line.
            Vector2 normal =
                new Vector2(-direction.y, direction.x);

            // Create the two sides of the line.
            Vector2 left =
                point + normal * halfWidth;

            Vector2 right =
                point - normal * halfWidth;

            vh.AddVert(left, color, Vector2.zero);
            vh.AddVert(right, color, Vector2.zero);
        }

        // Connect neighboring pairs of vertices into quads.
        for (int i = 0; i < curvePoints.Count - 1; i++)
        {
            int current = i * 2;
            int next = (i + 1) * 2;

            vh.AddTriangle(
                current,
                next,
                next + 1
            );

            vh.AddTriangle(
                current,
                next + 1,
                current + 1
            );
        }
    }
}