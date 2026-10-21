using System;
using System.Collections.Generic;
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

    [Header("Barriers")]

    [SerializeField]
    private List<Barriers> barrier =
        new List<Barriers>();

    [Serializable]
    private class Barriers
    {
        [Tooltip("The type of task that controls this cleanup rule.")]
        public TaskType taskType;

        [Tooltip(
            "Objects to hide when all tasks of this type are repaired."
        )]
        public List<GameObject> objectsToHide =
            new List<GameObject>();

        public List<GameObject> objectsToShow =
            new List<GameObject>();
    }

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

    private void Start()
    {
        Refreshbarrier();
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

    // Returns true if at least one task of this type exists
    // and every matching task has been repaired.
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
        Refreshbarrier();

        if (taskUI != null)
            taskUI.Refresh();
    }

    private void Refreshbarrier()
    {
        if (tasks == null || barrier == null)
            return;

        foreach (Barriers rule in barrier)
        {
            if (rule == null || rule.objectsToHide == null)
                continue;

            bool hasTasks = GetTotal(rule.taskType) > 0;

            bool allComplete =
                hasTasks && AreAllComplete(rule.taskType);

            bool shouldBeActive = !allComplete;

            foreach (GameObject target in rule.objectsToHide)
            {
                if (target == null)
                    continue;

                if (target.activeSelf != shouldBeActive)
                    target.SetActive(shouldBeActive);
            }
            foreach (GameObject target in rule.objectsToShow)
            {
                if (target == null)
                    continue;

                if (target.activeSelf != allComplete)
                    target.SetActive(allComplete);
            }
        }
    }

    public bool SetTaskBroken(StartGame task)
    {
        if (task == null)
            return false;

        if (tasks == null)
        {
            Debug.LogWarning(
                "RoomTasks has not initialized its task list.",
                this
            );

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
