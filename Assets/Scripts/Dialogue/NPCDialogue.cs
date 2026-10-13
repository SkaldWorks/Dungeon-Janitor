
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

        if (IsDialogueActive)
            dialogueManager.NextLine(this);
        else
            dialogueManager.StartDialogue(this);
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
