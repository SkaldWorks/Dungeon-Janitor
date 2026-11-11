
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UIBezierLine : MaskableGraphic
{
    public float lineWidth = 5f;

    [Header("Sine Wave")]
    public float waveAmplitude = 8f;
    public float waveCycles = 2f;
    public float waveSpeed = 2f;

    private List<Vector2> curvePoints = new List<Vector2>();

    public void SetPoints(List<Vector2> points)
    {
        if (points == null)
            return;

        curvePoints = points;
        SetVerticesDirty();
    }

    protected override void Awake()
    {
        base.Awake();
        raycastTarget = false;
    }

    void Update()
    {
        // Rebuild the mesh so the wave animates.
        if (waveAmplitude != 0)
            SetVerticesDirty();
    }

    Vector2 GetWavePoint(int index)
    {
        float t = index / (float)(curvePoints.Count - 1);

        float wave = Mathf.Sin(
            t * waveCycles * Mathf.PI * 2f +
            Time.unscaledTime * waveSpeed
        );

        // Keep the two ends of the line in their original positions.
        float envelope = Mathf.Sin(t * Mathf.PI);

        return curvePoints[index] +
               Vector2.up * wave * waveAmplitude * envelope;
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (curvePoints == null || curvePoints.Count < 2)
            return;

        float halfWidth = lineWidth * 0.5f;

        for (int i = 0; i < curvePoints.Count; i++)
        {
            Vector2 point = GetWavePoint(i);

            Vector2 direction;

            if (i == 0)
            {
                direction = GetWavePoint(1) - point;
            }
            else if (i == curvePoints.Count - 1)
            {
                direction = point - GetWavePoint(i - 1);
            }
            else
            {
                Vector2 directionToNext =
                    GetWavePoint(i + 1) - point;

                Vector2 directionFromPrevious =
                    point - GetWavePoint(i - 1);

                direction =
                    directionToNext.normalized +
                    directionFromPrevious.normalized;
            }

            if (direction.sqrMagnitude < 0.0001f)
                direction = Vector2.right;

            direction.Normalize();

            Vector2 normal =
                new Vector2(-direction.y, direction.x);

            Vector2 left = point + normal * halfWidth;
            Vector2 right = point - normal * halfWidth;

            vh.AddVert(left, color, Vector2.zero);
            vh.AddVert(right, color, Vector2.zero);
        }

        for (int i = 0; i < curvePoints.Count - 1; i++)
        {
            int current = i * 2;
            int next = (i + 1) * 2;

            vh.AddTriangle(current, next, next + 1);
            vh.AddTriangle(current, next + 1, current + 1);
        }
    }
}
