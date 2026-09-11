using UnityEngine;

public class Tripwire : MonoBehaviour, IInteractable
{
    [Header("Tripwire Visuals")]
    [SerializeField] private GameObject brokenVisual;
    [SerializeField] private GameObject repairedVisual;

    [Header("Minigame")]
    [SerializeField] private TripwireMinigame minigame;

    private bool repaired = false;

    public void Interact()
    {
        if (repaired)
            return;

        if (minigame == null)
        {
            Debug.LogError("Tripwire has no TripwireMinigame assigned.", this);
            return;
        }

        minigame.StartMinigame(this);
    }

    public void OnTouchingPlayer()
    {
        // You can add highlighting here later.
    }

    public void OnNotTouchingPlayer()
    {
        // Stop highlighting here later.
    }

    public void CompleteRepair()
    {
        if (repaired)
            return;

        repaired = true;

        if (brokenVisual != null)
            brokenVisual.SetActive(false);

        if (repairedVisual != null)
            repairedVisual.SetActive(true);

        Debug.Log("Tripwire repaired!");
    }
}