
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
public class StartGame : MonoBehaviour, IInteractable
{
    [Header("Task")]
    public TaskType taskType;
    public bool hasPrerequisite;
    public TaskType requiredTaskType;
    public bool repaired;

    [Tooltip("Automatically generated. Keep this ID unchanged to preserve saves.")]
    [SerializeField, HideInInspector]
    private string saveId;

    [Tooltip("If enabled, this task counts toward dungeon completion.")]
    public bool requiredForDungeonCompletion = true;

    [Header("Visuals")]
    public GameObject brokenVisual;
    public GameObject repairedVisual;
    public GameObject ExitButton;

    [Header("Minigame")]
    public MonoBehaviour minigame;

    public Interact interactScript;
    public BrokenCount BrokenCount;

    [HideInInspector]
    public RoomTasks roomTasks;

    private IMinigame minigameInterface;

    public string SaveId => saveId;

    private void Awake()
    {
        minigameInterface = minigame as IMinigame;
        roomTasks = GetComponentInParent<RoomTasks>();

        if (ExitButton == null)
            ExitButton = GameObject.Find("ExitButton");

        if (ExitButton != null)
            ExitButton.SetActive(false);
    }

    private void Start()
    {
        LoadSavedState();

        if (roomTasks != null)
            roomTasks.Refresh();

        if (ProgressManager.Instance != null)
            ProgressManager.Instance.RefreshProgress();
    }

    public void LoadSavedState()
    {
        if (SaveManager.Instance != null &&
            SaveManager.Instance.TryGetTaskState(
                saveId, out bool savedRepaired))
        {
            repaired = savedRepaired;
        }
        else
        {
            repaired = false;
        }

        UpdateRepairVisuals();
    }
    private void UpdateRepairVisuals()
    {
        if (brokenVisual != null)
            brokenVisual.SetActive(!repaired);

        if (repairedVisual != null)
            repairedVisual.SetActive(repaired);
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
        {
            minigameInterface.StartMinigame(this);

            if (ExitButton != null)
                ExitButton.SetActive(true);
        }
    }

    public void OnTouchingPlayer()
    {
        if (!repaired &&
            interactScript != null &&
            interactScript.interactionUI != null)
        {
            interactScript.interactionUI.SetActive(true);
        }
    }

    public void OnNotTouchingPlayer()
    {
    }

    public void CompleteRepair()
    {
        if (repaired)
            return;

        repaired = true;
        UpdateRepairVisuals();

        if (SaveManager.Instance != null)
            SaveManager.Instance.SaveTaskState(saveId, true);

        if (BrokenCount != null)
            BrokenCount.recount();

        if (roomTasks != null)
            roomTasks.Refresh();

        if (ProgressManager.Instance != null)
            ProgressManager.Instance.RefreshProgress();
    }

    public void resetgame()
    {
        repaired = false;
        UpdateRepairVisuals();

        if (SaveManager.Instance != null)
            SaveManager.Instance.SaveTaskState(saveId, false);

        if (roomTasks != null)
            roomTasks.Refresh();

        if (ProgressManager.Instance != null)
            ProgressManager.Instance.RefreshProgress();
    }

#if UNITY_EDITOR
    private void Reset()
    {
        GenerateSaveIdIfNeeded();
    }

    private void OnValidate()
    {
        GenerateSaveIdIfNeeded();
    }

    private void GenerateSaveIdIfNeeded()
    {
        if (!string.IsNullOrEmpty(saveId))
            return;

        saveId = System.Guid.NewGuid().ToString("N");
        EditorUtility.SetDirty(this);
    }
#endif
}

public interface IMinigame
{
    void StartMinigame(StartGame source);
}

public enum TaskType
{
    FixBrokenObject,
    SetTrap,
    Revive,
    ReviveSkeleton,
    PickUpBones,
    AssembleSkeleton,
    CleanGoop,
    ReviveSlime
}
