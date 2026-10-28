using System;
using UnityEngine;
using UnityEngine.Events;

public class NPCDialogue : MonoBehaviour, IInteractable
{
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

    /// <summary>
    /// Checks whether this NPC currently has an active dialogue.
    /// </summary>
    /// <returns>True if this NPC has an active dialogue; otherwise, false.</returns>
    public bool IsDialogueActive =>
        dialogueManager != null &&
        dialogueManager.IsDialogueActiveFor(this);

    /// <summary>
    /// Starts a dialogue with this NPC if one is not already active.
    /// </summary>
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

    /// <summary>
    /// Defines the behavior to execute when the player touches this NPC.
    /// </summary>
    public void OnTouchingPlayer()
    {
    }

    /// <summary>
    /// Ends this NPC's dialogue when the player is no longer touching it.
    /// </summary>
    public void OnNotTouchingPlayer()
    {
        if (dialogueManager != null)
            dialogueManager.EndDialogue(this);
    }
}