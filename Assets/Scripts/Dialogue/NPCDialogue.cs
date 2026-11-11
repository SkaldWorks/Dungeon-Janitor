
using System;
using UnityEngine;
using UnityEngine.Events;

public class NPCDialogue : MonoBehaviour, IInteractable
{
    [Header("Speaker")]
    [Tooltip("The name of this NPC.")]
    public string speakerName;

    [Tooltip("The portrait displayed when this NPC speaks.")]
    public Sprite portrait;

    [Serializable]
    public class DialogueLine
    {
        [Tooltip("The name of the character speaking this dialogue line.")]
        public string speaker;

        [Tooltip("The dialogue text displayed to the player.")]
        [TextArea(2, 4)]
        public string text;

        [Tooltip("The voice audio clip played when this dialogue line is displayed.")]
        public AudioClip voiceLine;

        [Tooltip("The event invoked when the player advances past this dialogue line.")]
        public UnityEvent onLineFinished = new UnityEvent();
    }

    [Header("Dialogue Lines")]
    [Tooltip("The sequence of dialogue lines this NPC will display.")]
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
