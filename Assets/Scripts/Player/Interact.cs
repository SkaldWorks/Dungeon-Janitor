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
    private IInteractable currentInteractable;
    public GameObject interactionUI;
    public TextMeshProUGUI interactionText;

    void Start()
    {
        interactionUI.SetActive(false);
    }
    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame && currentInteractable != null)
        {
            currentInteractable.Interact();
            currentInteractable = null;
            interactionUI.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();

        if (interactable != null)
        {
            interactionUI.SetActive(true);
            currentInteractable = interactable;
            currentInteractable.OnTouchingPlayer();


        }
    }
    private void OnTriggerExit(Collider collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();

        if (interactable != null && interactable == currentInteractable)
        {
            interactionUI.SetActive(false);

            currentInteractable.OnNotTouchingPlayer();
            currentInteractable = null;

        }
    }
}