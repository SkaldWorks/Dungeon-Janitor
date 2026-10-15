
using System.Collections.Generic;
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
        if (currentRoom != room)
            return;

        currentRoom = null;
        panel.SetActive(false);
    }

    public void Refresh()
    {
        if (currentRoom == null)
            return;

        roomNameText.text = currentRoom.roomName;

        StringBuilder text = new StringBuilder();
        HashSet<TaskType> shown = new HashSet<TaskType>();
        StartGame[] tasks = currentRoom.GetTasks();

        // Display tasks that have no prerequisites first.
        foreach (StartGame task in tasks)
        {
            if (task == null || task.hasPrerequisite)
                continue;

            ShowTaskChain(text, task, tasks, shown, 0);
        }

        taskText.text = text.ToString();
    }

    private void ShowTaskChain(
        StringBuilder text,
        StartGame task,
        StartGame[] tasks,
        HashSet<TaskType> shown,
        int depth)
    {
        if (!shown.Add(task.taskType))
            return;

        AddTask(text, task, depth);

        foreach (StartGame child in tasks)
        {
            if (child == null ||
                !child.hasPrerequisite ||
                child.requiredTaskType != task.taskType)
                continue;

            ShowTaskChain(text, child, tasks, shown, depth + 1);
        }
    }

    private void AddTask(
        StringBuilder text,
        StartGame task,
        int depth)
    {
        int total = currentRoom.GetTotal(task.taskType);
        int complete = currentRoom.GetCompleted(task.taskType);

        bool locked = task.hasPrerequisite &&
            !currentRoom.AreAllComplete(task.requiredTaskType);

        if (locked)
            text.Append("<color=#777777>");

        text.Append(' ', depth * 4);
        text.Append(GetName(task.taskType));
        text.Append("   ");
        text.Append(complete);
        text.Append('/');
        text.Append(total);

        if (locked)
            text.Append("</color>");

        text.AppendLine();
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

            default:
                return type.ToString();
        }
    }
}
