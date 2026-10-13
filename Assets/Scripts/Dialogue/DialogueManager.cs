
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TMP_Text speakerText;
    public TMP_Text dialogueText;
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

        speakerText.text = line.speaker;
        dialogueText.text = line.text;

        voiceSource.Stop();
        voiceSource.clip = line.voiceLine;

        if (line.voiceLine != null)
            voiceSource.Play();
    }
}
