using System.Collections.Generic;
using UnityEngine;

public class ReviveMinigame : MonoBehaviour, IMinigame
{
    [Header("Canvas")]
    public GameObject minigamePanel;

    [Header("Player")]
    public PlayerController player;
    public Bezier_Viz playerBezier;

    [Header("Target")]
    public UIBezierLine targetLine;

    public List<Vector2> targetControlPoints =
        new List<Vector2>
        {
            new Vector2(-400f, -100f),
            new Vector2(-150f, 250f),
            new Vector2(150f, -250f),
            new Vector2(400f, 100f)
        };

    [Header("Settings")]
    public int sampleCount = 100;

    // Maximum average distance that can still count as perfect.
    public float allowedDistance = 50f;

    // Accuracy needed to complete the minigame.
    public float requiredAccuracy = 0.85f;

    [Header("Debug")]
    public bool showDebugMessages = true;

    private StartGame currentSource;

    private bool isPlaying = false;

    private void Start()
    {
        if (minigamePanel != null)
            minigamePanel.SetActive(false);

        GenerateTargetCurve();
    }

    private void Update()
    {
        if (!isPlaying)
            return;

        CheckCurve();
    }

    public void StartMinigame(StartGame source)
    {
        if (isPlaying)
            return;

        if (source == null)
        {
            Debug.LogError(
                "ReviveMinigame was started without a source.",
                this
            );

            return;
        }

        if (playerBezier == null)
        {
            Debug.LogError(
                "Player Bezier is not assigned.",
                this
            );

            return;
        }

        currentSource = source;
        isPlaying = true;

        if (player != null)
            player.freeze = true;

        if (minigamePanel != null)
            minigamePanel.SetActive(true);

        GenerateTargetCurve();
    }

    private void GenerateTargetCurve()
    {
        if (targetLine == null)
        {
            Debug.LogError(
                "Target line is not assigned.",
                this
            );

            return;
        }

        if (targetControlPoints.Count < 2)
        {
            Debug.LogError(
                "Target needs at least 2 control points.",
                this
            );

            return;
        }

        List<Vector2> targetCurve =
            BezierCurve.Sample(
                targetControlPoints,
                sampleCount
            );

        targetLine.SetPoints(targetCurve);
    }

    private void CheckCurve()
    {
        List<Vector2> playerPoints =
            playerBezier.GetControlPoints();

        if (playerPoints.Count < 2)
            return;

        float accuracy =
            CalculateCurveAccuracy(
                playerPoints,
                targetControlPoints
            );

        if (showDebugMessages)
        {
            Debug.Log(
                "Curve Accuracy: " + accuracy
            );
        }

        if (accuracy >= requiredAccuracy)
        {
            CompleteMinigame();
        }
    }

    private float CalculateCurveAccuracy(
        List<Vector2> playerPoints,
        List<Vector2> targetPoints)
    {
        List<Vector2> playerCurve =
            BezierCurve.Sample(
                playerPoints,
                sampleCount
            );

        List<Vector2> targetCurve =
            BezierCurve.Sample(
                targetPoints,
                sampleCount
            );

        if (playerCurve.Count == 0 ||
            targetCurve.Count == 0)
        {
            return 0f;
        }

        // Measure how close the player's curve is to the target.
        float playerToTarget =
            AverageNearestDistance(
                playerCurve,
                targetCurve
            );

        // Do the same thing in the opposite direction.
        // to prevent a small part of the target from being ignored.
        float targetToPlayer =
            AverageNearestDistance(
                targetCurve,
                playerCurve
            );

        // Take both measurements into account.
        float averageDistance =
            (playerToTarget + targetToPlayer) * 0.5f;

        // Convert distance into a 0-1 accuracy value.
        // 1 = perfect, 0 = too far away.
        float accuracy =
            1f -
            Mathf.Clamp01(
                averageDistance / allowedDistance
            );

        return accuracy;
    }

    private float AverageNearestDistance(
        List<Vector2> from,
        List<Vector2> to)
    {
        float totalDistance = 0f;

        for (int i = 0; i < from.Count; i++)
        {
            // Start with a very large distance.
            float closestDistance =
                float.MaxValue;

            for (int j = 0; j < to.Count; j++)
            {
                float distance =
                    Vector2.Distance(
                        from[i],
                        to[j]
                    );

                // Keep whichever target point is closest.
                if (distance < closestDistance)
                    closestDistance = distance;
            }

            totalDistance += closestDistance;
        }

        // Return the average distance between the two curves.
        return totalDistance / from.Count;
    }

    private void CompleteMinigame()
    {
        if (!isPlaying)
            return;

        isPlaying = false;

        if (currentSource != null)
        {
            currentSource.CompleteRepair();
        }

        if (minigamePanel != null)
            minigamePanel.SetActive(false);

        if (player != null)
            player.freeze = false;

        currentSource = null;

        Debug.Log("Revive minigame complete!");
    }

    public void CancelMinigame()
    {
        if (!isPlaying)
            return;

        isPlaying = false;

        if (minigamePanel != null)
            minigamePanel.SetActive(false);

        if (player != null)
            player.freeze = false;

        currentSource = null;
    }
}