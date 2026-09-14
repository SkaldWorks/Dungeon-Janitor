using System.Collections.Generic;
using UnityEngine;

public class ReviveMinigame : MonoBehaviour
{
    [Header("Player Curve")]
    [SerializeField] private Bezier_Viz bezierViz;

    [Header("Target Curve")]
    [SerializeField] private LineRenderer targetLine;

    [Header("Target Points")]
    [SerializeField] private List<Vector2> targetControlPoints = new List<Vector2>();

    [Header("Settings")]
    [SerializeField] private float sampleInterval = 0.02f;

    [SerializeField] private float allowedDistance = 50f;

    [SerializeField] private float requiredAccuracy = 0.85f;

    [Header("Debug")]
    [SerializeField] private bool showDebugMessages = true;

    private bool completed = false;

    private void Start()
    {
        GenerateTargetCurve();
    }

    private void Update()
    {
        if (completed)
            return;

        CheckCurve();
    }

    private void GenerateTargetCurve()
    {
        if (targetLine == null)
        {
            Debug.LogError("Target LineRenderer is not assigned.");
            return;
        }

        if (targetControlPoints.Count < 2)
        {
            Debug.LogError("Target needs at least 2 control points.");
            return;
        }

        List<Vector2> targetCurve =
            BezierCurve.PointList2(targetControlPoints, sampleInterval);

        targetLine.positionCount = targetCurve.Count;

        for (int i = 0; i < targetCurve.Count; i++)
        {
            targetLine.SetPosition(
                i,
                new Vector3(
                    targetCurve[i].x,
                    targetCurve[i].y,
                    0f
                )
            );
        }
    }

    private void CheckCurve()
    {
        if (bezierViz == null)
            return;

        List<Vector2> playerPoints = bezierViz.GetControlPoints();

        if (playerPoints.Count < 2)
            return;

        float accuracy = CalculateCurveAccuracy(
            playerPoints,
            targetControlPoints
        );

        if (showDebugMessages)
        {
            Debug.Log("Curve Accuracy: " + accuracy);
        }

        if (accuracy >= requiredAccuracy)
        {
            CompleteChallenge();
        }
    }

    private float CalculateCurveAccuracy(
        List<Vector2> playerPoints,
        List<Vector2> targetPoints)
    {
        List<Vector2> playerCurve =
            BezierCurve.PointList2(playerPoints, sampleInterval);

        List<Vector2> targetCurve =
            BezierCurve.PointList2(targetPoints, sampleInterval);

        int sampleCount = Mathf.Min(
            playerCurve.Count,
            targetCurve.Count
        );

        if (sampleCount == 0)
            return 0f;

        float totalDistance = 0f;

        for (int i = 0; i < sampleCount; i++)
        {
            float distance = Vector2.Distance(
                playerCurve[i],
                targetCurve[i]
            );

            totalDistance += distance;
        }

        float averageDistance = totalDistance / sampleCount;

        /*
         * Convert distance into an accuracy value.

         * 0 distance = 1.0 accuracy
         * allowedDistance = 0.0 accuracy
         */
        float accuracy = 1f -
            Mathf.Clamp01(averageDistance / allowedDistance);

        return accuracy;
    }

    private void CompleteChallenge()
    {
        completed = true;

        Debug.Log("You win!");
    }
}