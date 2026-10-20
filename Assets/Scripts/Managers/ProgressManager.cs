
using TMPro;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    [Header("UI (Optional)")]
    public TMP_Text progressText;

    [Header("Dungeon Exit")]
    public Door[] exitDoors;

    public int TotalTasks { get; private set; }
    public int CompletedTasks { get; private set; }

    public bool IsDungeonComplete =>
        TotalTasks > 0 && CompletedTasks >= TotalTasks;

    private StartGame[] tasks;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Multiple ProgressManagers found.", this);
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        RefreshProgress();
    }

    public void RefreshProgress()
    {
        tasks = FindObjectsByType<StartGame>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        TotalTasks = 0;
        CompletedTasks = 0;

        foreach (StartGame task in tasks)
        {
            if (task == null)
                continue;

            task.LoadSavedState();

            if (!task.requiredForDungeonCompletion)
                continue;

            TotalTasks++;

            if (task.repaired)
                CompletedTasks++;
        }

        UpdateUI();
        UpdateExitDoors();
    }
    private void UpdateUI()
    {
        if (progressText != null)
        {
            progressText.text =
                $"Dungeon Progress: {CompletedTasks}/{TotalTasks}";
        }
    }

    private void UpdateExitDoors()
    {
        if (exitDoors == null)
            return;

        foreach (Door door in exitDoors)
        {
            if (door != null)
                door.SetCanOpen(IsDungeonComplete);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
