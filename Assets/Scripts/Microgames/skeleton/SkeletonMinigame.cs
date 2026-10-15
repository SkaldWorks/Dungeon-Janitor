
using System.Collections.Generic;
using UnityEngine;

public class SkeletonMinigame : MonoBehaviour, IMinigame
{
    public GameObject panel;
    public List<DraggableSprite> pieces = new();

    private StartGame source;
    private int placedCount;

    public bool IsRunning { get; private set; }

    public void StartMinigame(StartGame source)
    {
        if (pieces.Count == 0)
        {
            Debug.LogError("SkeletonMinigame: No pieces assigned.", this);
            return;
        }

        foreach (DraggableSprite piece in pieces)
        {
            if (piece == null || piece.Target == null)
            {
                Debug.LogError("Assign every piece and its target.", this);
                return;
            }
        }

        if (panel == null)
            panel = gameObject;

        this.source = source;
        placedCount = 0;

        foreach (DraggableSprite piece in pieces)
        {
            piece.Initialize(this);
            piece.ResetPiece();
        }

        IsRunning = true;
        panel.SetActive(true);
    }

    internal bool TryPlace(
        DraggableSprite piece,
        Vector2 position,
        Camera eventCamera)
    {
        return IsRunning &&
               piece != null &&
               !piece.IsPlaced &&
               piece.Target != null &&
               RectTransformUtility.RectangleContainsScreenPoint(
                   piece.Target, position, eventCamera);
    }

    internal void PiecePlaced()
    {
        if (!IsRunning)
            return;

        if (++placedCount >= pieces.Count)
        {
            IsRunning = false;

            if (source != null)
                source.CompleteRepair();

            source = null;
            panel.SetActive(false);
        }
    }
}
