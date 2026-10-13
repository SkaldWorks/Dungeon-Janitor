
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    void Interact();
    void OnTouchingPlayer();
    void OnNotTouchingPlayer();
}

public class Interact : MonoBehaviour
{
    public DialogueManager dialogueManager;

    public GameObject interactionUI;
    public TextMeshProUGUI interactionText;
    public string interactionPrompt = "Press E to interact";

    private IInteractable currentInteractable;

    private void Awake()
    {
        interactionUI.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current == null ||
            !Keyboard.current.eKey.wasPressedThisFrame ||
            currentInteractable == null)
        {
            return;
        }

        currentInteractable.Interact();

        if (currentInteractable is NPCDialogue npc)
        {
            interactionUI.SetActive(!npc.IsDialogueActive);
        }
        else
        {
            currentInteractable = null;
            interactionUI.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IInteractable interactable =
            other.GetComponent<IInteractable>();

        if (interactable == null)
            return;

        currentInteractable = interactable;

        if (interactable is NPCDialogue npc)
            npc.dialogueManager = dialogueManager;

        interactionText.text = interactionPrompt;
        interactionUI.SetActive(true);

        interactable.OnTouchingPlayer();
    }

    private void OnTriggerExit(Collider other)
    {
        IInteractable interactable =
            other.GetComponent<IInteractable>();

        if (interactable == null ||
            interactable != currentInteractable)
        {
            return;
        }

        interactable.OnNotTouchingPlayer();
        currentInteractable = null;
        interactionUI.SetActive(false);
    }
}
