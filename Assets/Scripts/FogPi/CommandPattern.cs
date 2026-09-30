using UnityEngine;
using System;

public class CommandPattern : MonoBehaviour
{
    public float moveDistance = 1.0f;
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.W))
        //    ExecuteMoveCommand(Vector3.forward)
    }
}

public interface ICommand
{
    void Execute();
    void Undo();
}

public class MoveCommand : ICommand
{
    private readonly Action executeAction;
    public void Execute()
    {
        
    }

    public void Undo()
    {
        
    }
}