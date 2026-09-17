using UnityEngine;

public class StartGame : MonoBehaviour, IInteractable
{
    [Header("Visuals")]
    public GameObject brokenVisual;
    public GameObject repairedVisual;

    [Header("Minigame")]
    public MonoBehaviour minigame;

    public bool interacted = false;
    public bool repaired = false;

    public Interact interactScript;

    public BrokenCount BrokenCount;

    private IMinigame minigameInterface;

    private void Awake()
    {
        minigameInterface = minigame as IMinigame;

        if (minigameInterface == null)
        {
            Debug.LogError(
                "Assigned minigame does not implement IMinigame.",
                this
            );
        }
    }

    public void Interact()
    {
        if (repaired)
            return;

        interacted = true;

        if (minigameInterface == null)
            return;

        minigameInterface.StartMinigame(this);
    }

    public void OnNotTouchingPlayer()
    {
    }

    public void OnTouchingPlayer()
    {
        if (!repaired)
        {
            interactScript.interactionUI.SetActive(true);
        }
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

        Debug.Log("Repaired!");

        BrokenCount.recount();
    }
    public void resetgame()
    {
        repaired = false;

        if (repairedVisual != null)
            repairedVisual.SetActive(false);

        if (brokenVisual != null)
            brokenVisual.SetActive(true);

        interacted = false;

        BrokenCount.recount();
    }
}

public interface IMinigame
{
    void StartMinigame(StartGame source);
}