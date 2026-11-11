
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue UI")]
    [Tooltip("The panel displayed while a dialogue is active.")]
    public GameObject dialoguePanel;

    [Tooltip("The UI text component that displays the current speaker's name.")]
    public TMP_Text speakerText;

    [Tooltip("The UI image that displays the current speaker's portrait.")]
    public Image speakerPortrait;

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

    public bool IsDialogueActiveFor(NPCDialogue npc)
    {
        return currentNPC == npc;
    }

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

        speakerText.text = string.IsNullOrWhiteSpace(line.speaker)
            ? currentNPC.speakerName
            : line.speaker;

        if (speakerPortrait != null)
        {
            speakerPortrait.sprite = currentNPC.portrait;
            speakerPortrait.enabled = currentNPC.portrait != null;
        }

        dialogueText.text = line.text;

        if (voiceSource != null)
        {
            voiceSource.Stop();
            voiceSource.clip = line.voiceLine;

            if (line.voiceLine != null)
                voiceSource.Play();
        }
    }
}
