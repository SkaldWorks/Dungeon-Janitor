
using System;
using UnityEngine;
using UnityEngine.Events;

public class NPCDialogue : MonoBehaviour, IInteractable
{
    [Serializable]
    public class DialogueLine
    {
        public string speaker;

        [TextArea(2, 4)]
        public string text;

        public AudioClip voiceLine;

        public UnityEvent onLineFinished = new UnityEvent();
    }

    public DialogueLine[] lines;

    [HideInInspector]
    public DialogueManager dialogueManager;

    public bool IsDialogueActive =>
        dialogueManager != null &&
        dialogueManager.IsDialogueActiveFor(this);

    public void Interact()
    {
        if (dialogueManager == null)
        {
            Debug.LogError("DialogueManager is not assigned.", this);
            return;
        }

        if (!IsDialogueActive)
            dialogueManager.StartDialogue(this);
    }

    void Update()
    {
        if (IsDialogueActive && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return)))
        {
            dialogueManager.NextLine(this);
        }
    }

    public void OnTouchingPlayer()
    {
    }

    public void OnNotTouchingPlayer()
    {
        if (dialogueManager != null)
            dialogueManager.EndDialogue(this);
    }
}
