
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private string SavePath =>
        Path.Combine(Application.persistentDataPath, "dungeon_save.json");

    [Serializable]
    private class SaveData
    {
        public List<TaskSave> taskStates = new List<TaskSave>();
        public List<DoorSave> doorStates = new List<DoorSave>();
        public string currentRoomId;
    }

    [Serializable]
    private class TaskSave
    {
        public string taskId;
        public bool repaired;
    }

    [Serializable]
    private class DoorSave
    {
        public string doorId;
        public bool canOpen;
        public bool isOpen;
    }

    private SaveData saveData = new SaveData();

    public bool HasSaveData =>
        saveData != null &&
        (
            (saveData.taskStates != null &&
             saveData.taskStates.Count > 0) ||

            (saveData.doorStates != null &&
             saveData.doorStates.Count > 0) ||

            !string.IsNullOrWhiteSpace(saveData.currentRoomId)
        );

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadGame();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }


    public void SaveTaskState(string taskId, bool repaired)
    {
        if (string.IsNullOrWhiteSpace(taskId))
        {
            Debug.LogWarning("Cannot save a task with an empty ID.", this);
            return;
        }

        EnsureSaveData();

        foreach (TaskSave task in saveData.taskStates)
        {
            if (task.taskId == taskId)
            {
                task.repaired = repaired;
                SaveGame();
                return;
            }
        }

        saveData.taskStates.Add(new TaskSave
        {
            taskId = taskId,
            repaired = repaired
        });

        SaveGame();
    }

    public bool TryGetTaskState(string taskId, out bool repaired)
    {
        repaired = false;

        if (saveData == null || saveData.taskStates == null)
            return false;

        foreach (TaskSave task in saveData.taskStates)
        {
            if (task.taskId == taskId)
            {
                repaired = task.repaired;
                return true;
            }
        }

        return false;
    }

    public void SaveDoorState(
        string doorId,
        bool canOpen,
        bool isOpen)
    {
        if (string.IsNullOrWhiteSpace(doorId))
        {
            Debug.LogWarning("Cannot save a door with an empty ID.", this);
            return;
        }

        EnsureSaveData();

        foreach (DoorSave door in saveData.doorStates)
        {
            if (door.doorId == doorId)
            {
                door.canOpen = canOpen;
                door.isOpen = isOpen;
                SaveGame();
                return;
            }
        }

        saveData.doorStates.Add(new DoorSave
        {
            doorId = doorId,
            canOpen = canOpen,
            isOpen = isOpen
        });

        SaveGame();
    }

    public bool TryGetDoorState(
        string doorId,
        out bool canOpen,
        out bool isOpen)
    {
        canOpen = false;
        isOpen = false;

        if (saveData == null || saveData.doorStates == null)
            return false;

        foreach (DoorSave door in saveData.doorStates)
        {
            if (door.doorId == doorId)
            {
                canOpen = door.canOpen;
                isOpen = door.isOpen;
                return true;
            }
        }

        return false;
    }

    public void SetCurrentRoom(string roomId)
    {
        if (string.IsNullOrWhiteSpace(roomId))
            return;

        EnsureSaveData();

        if (saveData.currentRoomId == roomId)
            return;

        saveData.currentRoomId = roomId;
        SaveGame();
    }

    public bool TryGetCurrentRoomId(out string roomId)
    {
        roomId = null;

        if (saveData == null ||
            string.IsNullOrWhiteSpace(saveData.currentRoomId))
        {
            return false;
        }

        roomId = saveData.currentRoomId;
        return true;
    }


    public void ResetSaveData()
    {
        saveData = new SaveData();
        SaveGame();
    }

    public void SaveGame()
    {
        EnsureSaveData();

        try
        {
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(SavePath, json);
        }
        catch (Exception exception)
        {
            Debug.LogError(
                $"Failed to save the game: {exception.Message}",
                this
            );
        }
    }

    private void LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            saveData = new SaveData();
            return;
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            saveData = JsonUtility.FromJson<SaveData>(json);

            if (saveData == null)
                saveData = new SaveData();

            EnsureSaveData();
        }
        catch (Exception exception)
        {
            Debug.LogError(
                $"Failed to load the game: {exception.Message}",
                this
            );

            saveData = new SaveData();
        }
    }

    private void EnsureSaveData()
    {
        if (saveData == null)
            saveData = new SaveData();

        if (saveData.taskStates == null)
            saveData.taskStates = new List<TaskSave>();

        if (saveData.doorStates == null)
            saveData.doorStates = new List<DoorSave>();
    }
}
