using UnityEngine;

public class RoomTasks : MonoBehaviour
{
    public string roomName;
    public TaskListUI taskUI;

    private StartGame[] tasks;

    private void Awake()
    {
        tasks = GetComponentsInChildren<StartGame>(true);

        foreach (StartGame task in tasks)
            task.roomTasks = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerController>() != null)
            taskUI.Show(this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<PlayerController>() != null)
            taskUI.Hide(this);
    }

    public bool AreAllComplete(TaskType type)
    {
        bool found = false;

        foreach (StartGame task in tasks)
        {
            if (task.taskType != type)
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
            if (task.taskType == type)
                count++;
        }

        return count;
    }

    public int GetCompleted(TaskType type)
    {
        int count = 0;

        foreach (StartGame task in tasks)
        {
            if (task.taskType == type && task.repaired)
                count++;
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
        Debug.LogWarning("RoomTasks has not initialized its task list.", this);
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
            $"Task '{task.name}' does not belong to room '{roomName}'.",
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
}