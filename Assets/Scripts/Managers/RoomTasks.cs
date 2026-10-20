using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Collider))]
public class RoomTasks : MonoBehaviour
{
    [Header("Room")]
    public string roomName;
    public TaskListUI taskUI;

    [Header("Saving and Spawn")]
    [SerializeField, HideInInspector]
    private string saveId;

    [Tooltip("Player spawn position when loading this room.")]
    [SerializeField]
    private Transform spawnPoint;

    public string SaveId => saveId;

    public Vector3 SpawnPosition =>
        spawnPoint != null ? spawnPoint.position : transform.position;

    private StartGame[] tasks;

    private void Awake()
    {
        tasks = GetComponentsInChildren<StartGame>(true);

        foreach (StartGame task in tasks)
        {
            if (task != null)
                task.roomTasks = this;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerController>() == null)
            return;

        if (SaveManager.Instance != null)
            SaveManager.Instance.SetCurrentRoom(saveId);

        if (taskUI != null)
            taskUI.Show(this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<PlayerController>() == null)
            return;

        if (taskUI != null)
            taskUI.Hide(this);
    }

    public bool AreAllComplete(TaskType type)
    {
        bool found = false;

        foreach (StartGame task in tasks)
        {
            if (task == null || task.taskType != type)
                continue;

            found = true;

            if (!task.repaired)
                return false;
        }

        return found;
    }

    public int GetTotal(TaskType type)
    {
        int count = 0;

        foreach (StartGame task in tasks)
        {
            if (task != null && task.taskType == type)
                count++;
        }

        return count;
    }

    public int GetCompleted(TaskType type)
    {
        int count = 0;

        foreach (StartGame task in tasks)
        {
            if (task != null &&
                task.taskType == type &&
                task.repaired)
            {
                count++;
            }
        }

        return count;
    }

    public StartGame[] GetTasks()
    {
        return tasks;
    }

    public void Refresh()
    {
        if (taskUI != null)
            taskUI.Refresh();
    }

    public bool SetTaskBroken(StartGame task)
    {
        if (task == null)
            return false;

        if (tasks == null)
        {
            Debug.LogWarning(
                "RoomTasks has not initialized its task list.", this);
            return false;
        }

        bool belongsToRoom = false;

        foreach (StartGame roomTask in tasks)
        {
            if (roomTask == task)
            {
                belongsToRoom = true;
                break;
            }
        }

        if (!belongsToRoom)
        {
            Debug.LogWarning(
                $"Task '{task.name}' does not belong to room " +
                $"'{roomName}'.",
                this
            );

            return false;
        }

        task.roomTasks = this;
        task.resetgame();

        if (task.BrokenCount != null)
            task.BrokenCount.recount();

        Refresh();

        return true;
    }

    public static RoomTasks FindById(string roomId)
    {
        if (string.IsNullOrWhiteSpace(roomId))
            return null;

        RoomTasks[] rooms = FindObjectsByType<RoomTasks>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (RoomTasks room in rooms)
        {
            if (room != null && room.saveId == roomId)
                return room;
        }

        return null;
    }

#if UNITY_EDITOR
    private void Reset()
    {
        SetColliderAsTrigger();
        GenerateSaveIdIfNeeded();
    }

    private void OnValidate()
    {
        SetColliderAsTrigger();
        GenerateSaveIdIfNeeded();
    }

    private void SetColliderAsTrigger()
    {
        Collider roomCollider = GetComponent<Collider>();

        if (roomCollider != null)
            roomCollider.isTrigger = true;
    }

    private void GenerateSaveIdIfNeeded()
    {
        if (!string.IsNullOrWhiteSpace(saveId))
            return;

        saveId = Guid.NewGuid().ToString("N");
        EditorUtility.SetDirty(this);
    }

    [ContextMenu("Regenerate Save ID")]
    private void RegenerateSaveId()
    {
        saveId = Guid.NewGuid().ToString("N");
        EditorUtility.SetDirty(this);
    }
#endif
}