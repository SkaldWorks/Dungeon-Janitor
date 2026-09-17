using System.Text;
using TMPro;
using UnityEngine;

public class TaskListUI : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI roomNameText;
    public TextMeshProUGUI taskText;

    private RoomTasks currentRoom;

    private void Start()
    {
        panel.SetActive(false);
    }

    public void Show(RoomTasks room)
    {
        currentRoom = room;
        panel.SetActive(true);
        Refresh();
    }

    public void Hide(RoomTasks room)
    {
        if (currentRoom == room)
        {
            currentRoom = null;
            panel.SetActive(false);
        }
    }

    public void Refresh()
    {
        if (currentRoom == null)
            return;

        roomNameText.text = currentRoom.roomName;

        StringBuilder text = new StringBuilder();

        StartGame[] tasks = currentRoom.GetTasks();

        // First show tasks with no prerequisite.
        foreach (StartGame task in tasks)
        {
            if (task.hasPrerequisite)
                continue;

            // Don't show the same type twice.
            if (HasTaskTypeAlreadyShown(tasks, task.taskType, text))
                continue;

            AddTask(text, task, false);

            // show anything that depends on this task.
            ShowChildren(text, task.taskType, tasks);
        }

        taskText.text = text.ToString();
    }

    private void ShowChildren(
        StringBuilder text,
        TaskType parentType,
        StartGame[] tasks)
    {
        foreach (StartGame task in tasks)
        {
            if (!task.hasPrerequisite)
                continue;

            if (task.requiredTaskType != parentType)
                continue;

            // Don't show the same type twice.
            if (HasTaskTypeAlreadyShown(tasks, task.taskType, text))
                continue;

            AddTask(text, task, true);
        }
    }

    private void AddTask(
        StringBuilder text,
        StartGame task,
        bool indented)
    {
        int total =
            currentRoom.GetTotal(task.taskType);

        int complete =
            currentRoom.GetCompleted(task.taskType);

        bool locked =
            task.hasPrerequisite &&
            !currentRoom.AreAllComplete(
                task.requiredTaskType
            );

        string name = GetName(task.taskType);

        if (indented)
            text.Append("    ");

        if (locked)
            text.Append("<color=#777777>");

        text.Append(
            name + "   " +
            complete + "/" + total
        );

        if (locked)
            text.Append("</color>");

        text.AppendLine();
    }

    private bool HasTaskTypeAlreadyShown(
        StartGame[] tasks,
        TaskType type,
        StringBuilder text)
    {
        string name = GetName(type);

        return text.ToString().Contains(name);
    }

    private string GetName(TaskType type)
    {
        switch (type)
        {
            case TaskType.SetTrap:
                return "Set Traps";

            case TaskType.Revive:
                return "Revive";

            case TaskType.FixBrokenObject:
                return "Fix Broken Objects";

            case TaskType.PickUpBones:
                return "Pick Up Bones";
        }

        return type.ToString();
    }
}