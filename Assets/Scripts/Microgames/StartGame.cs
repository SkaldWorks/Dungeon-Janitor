using UnityEngine;

public class StartGame : MonoBehaviour, IInteractable
{
    [Header("Visuals")]
    [SerializeField] private GameObject brokenVisual;
    [SerializeField] private GameObject repairedVisual;

    [Header("Minigame")]
    [SerializeField] private MonoBehaviour minigame;

    public bool interacted = false;
    public bool repaired = false;

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
    }
}

public interface IMinigame
{
    void StartMinigame(StartGame source);
}