using System.Collections.Generic;
using UnityEngine;

public class ReviveMinigame : MonoBehaviour, IMinigame
{
    public GameObject minigamePanel;

    public PlayerController player;
    public Bezier_Viz playerBezier;

    public UIBezierLine targetLine;
    public List<Vector2> targetControlPoints = new List<Vector2>
    {
        new Vector2(-400, -100),
        new Vector2(-150, 250),
        new Vector2(150, -250),
        new Vector2(400, 100)
    };

    public int sampleCount = 100;
    public float allowedDistance = 50;
    public float requiredAccuracy = 0.85f;

    StartGame source;
    bool playing;

    void Start()
    {
        minigamePanel.SetActive(false);
        ShowTarget();
        ResetPlayer();
    }

    void Update()
    {
        if (playing)
            CheckCurve();
    }

    public void StartMinigame(StartGame newSource)
    {
        if (playing)
            return;

        source = newSource;
        playing = true;

        if (player != null)
            player.freeze = true;

        minigamePanel.SetActive(true);

        ShowTarget();
        ResetPlayer();
    }

    void ShowTarget()
    {
        targetLine.SetPoints(
            BezierCurve.Sample(targetControlPoints, sampleCount)
        );
    }

    void ResetPlayer()
    {
        playerBezier.ResetPoints(
            targetControlPoints[0],
            targetControlPoints[targetControlPoints.Count - 1]
        );
    }

    void CheckCurve()
    {
        float accuracy = GetAccuracy(
            playerBezier.GetControlPoints(),
            targetControlPoints
        );

        if (accuracy >= requiredAccuracy)
            Finish();
    }

    float GetAccuracy(
        List<Vector2> playerPoints,
        List<Vector2> targetPoints)
    {
        List<Vector2> playerCurve =
            BezierCurve.Sample(playerPoints, sampleCount);

        List<Vector2> targetCurve =
            BezierCurve.Sample(targetPoints, sampleCount);

        float distance =
            (NearestAverage(playerCurve, targetCurve) +
             NearestAverage(targetCurve, playerCurve)) / 2f;

        return 1f - Mathf.Clamp01(distance / allowedDistance);
    }

    float NearestAverage(
        List<Vector2> from,
        List<Vector2> to)
    {
        float total = 0;

        foreach (Vector2 a in from)
        {
            float closest = float.MaxValue;

            foreach (Vector2 b in to)
                closest = Mathf.Min(closest, Vector2.Distance(a, b));

            total += closest;
        }

        return total / from.Count;
    }

    void Finish()
    {
        playing = false;
        ResetPlayer();

        if (source != null)
            source.CompleteRepair();

        minigamePanel.SetActive(false);

        if (player != null)
            player.freeze = false;

        source = null;
    }

    public void CancelMinigame()
    {
        if (!playing)
            return;

        playing = false;
        ResetPlayer();

        minigamePanel.SetActive(false);

        if (player != null)
            player.freeze = false;

        source = null;
    }
}