using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue UI")]
    [Tooltip("The panel displayed while a dialogue is active.")]
    public GameObject dialoguePanel;

    [Tooltip("The UI text component that displays the current speaker's name.")]
    public TMP_Text speakerText;

    [Tooltip("The UI text component that displays the current dialogue line.")]
    public TMP_Text dialogueText;

    [Header("Dialogue Audio")]
    [Tooltip("The audio source used to play voice lines during dialogue.")]
    public AudioSource voiceSource;

    private NPCDialogue currentNPC;
    private int currentLineIndex;

    private void Awake()
    {
        dialoguePanel.SetActive(false);
    }

    /// <summary>
    /// Checks whether the specified NPC currently has an active dialogue.
    /// </summary>
    /// <param name="npc">The NPC whose dialogue activity will be checked.</param>
    /// <returns>True if the specified NPC is currently in dialogue; otherwise, false.</returns>
    public bool IsDialogueActiveFor(NPCDialogue npc)
    {
        return currentNPC == npc;
    }

    /// <summary>
    /// Starts a dialogue with the specified NPC and displays its first line.
    /// If the NPC has no dialogue lines, the current dialogue is ended.
    /// </summary>
    /// <param name="npc">The NPC whose dialogue will be started.</param>
    public void StartDialogue(NPCDialogue npc)
    {
        if (npc.lines == null || npc.lines.Length == 0)
        {
            EndDialogue();
            return;
        }

        EndDialogue();

        currentNPC = npc;
        currentLineIndex = 0;

        dialoguePanel.SetActive(true);
        ShowLine();
    }

    /// <summary>
    /// Advances the dialogue to the next line for the specified NPC.
    /// Invokes the current line's completion event before advancing.
    /// Ends the dialogue when no more lines remain.
    /// </summary>
    /// <param name="npc">The NPC whose dialogue will advance.</param>
    public void NextLine(NPCDialogue npc)
    {
        if (currentNPC != npc)
            return;

        NPCDialogue.DialogueLine line =
            currentNPC.lines[currentLineIndex];

        if (line != null)
            line.onLineFinished.Invoke();

        if (currentNPC != npc)
            return;

        currentLineIndex++;

        if (currentLineIndex >= currentNPC.lines.Length)
        {
            EndDialogue();
            return;
        }

        ShowLine();
    }

    /// <summary>
    /// Ends the dialogue if the specified NPC is currently in dialogue.
    /// </summary>
    /// <param name="npc">The NPC whose dialogue should be ended.</param>
    public void EndDialogue(NPCDialogue npc)
    {
        if (currentNPC == npc)
            EndDialogue();
    }

    private void EndDialogue()
    {
        if (voiceSource != null)
            voiceSource.Stop();

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        currentNPC = null;
        currentLineIndex = 0;
    }

    private void ShowLine()
    {
        NPCDialogue.DialogueLine line =
            currentNPC.lines[currentLineIndex];

        if (line == null)
        {
            EndDialogue();
            return;
        }

        speakerText.text = line.speaker;
        dialogueText.text = line.text;

        voiceSource.Stop();
        voiceSource.clip = line.voiceLine;

        if (line.voiceLine != null)
            voiceSource.Play();
    }
}