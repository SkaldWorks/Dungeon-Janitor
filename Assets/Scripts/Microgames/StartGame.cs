using UnityEngine;

public class StartGame : MonoBehaviour, IInteractable
{
    [Header("Task")]
    public TaskType taskType;
    public bool hasPrerequisite;
    public TaskType requiredTaskType;

    public bool repaired;

    [Header("Visuals")]
    public GameObject brokenVisual;
    public GameObject repairedVisual;

    [Header("Minigame")]
    public MonoBehaviour minigame;

    public Interact interactScript;
    public BrokenCount BrokenCount;

    [HideInInspector]
    public RoomTasks roomTasks;

    private IMinigame minigameInterface;

    private void Awake()
    {
        minigameInterface = minigame as IMinigame;
        roomTasks = GetComponentInParent<RoomTasks>();
    }

    public void Interact()
    {
        if (repaired)
            return;

        if (hasPrerequisite &&
            roomTasks != null &&
            !roomTasks.AreAllComplete(requiredTaskType))
        {
            return;
        }

        if (minigameInterface != null)
            minigameInterface.StartMinigame(this);
    }

    public void OnTouchingPlayer()
    {
        if (!repaired)
            interactScript.interactionUI.SetActive(true);
    }

    public void OnNotTouchingPlayer()
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

        if (BrokenCount != null)
            BrokenCount.recount();

        if (roomTasks != null)
            roomTasks.Refresh();
    }

    public void resetgame()
    {
        repaired = false;

        if (brokenVisual != null)
            brokenVisual.SetActive(true);

        if (repairedVisual != null)
            repairedVisual.SetActive(false);

        if (roomTasks != null)
            roomTasks.Refresh();
    }
}

public interface IMinigame
{
    void StartMinigame(StartGame source);
}