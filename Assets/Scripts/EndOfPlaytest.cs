
using UnityEngine;

public class EndOfPlaytest : MonoBehaviour, IInteractable
{
    public GameObject endOfPlaytestPanel;

    public void Interact()
    {
        Debug.Log("End of playtest Interact() called!");

        if (endOfPlaytestPanel == null)
        {
            Debug.LogError("End of Playtest Panel is not assigned!", this);
            return;
        }

        endOfPlaytestPanel.SetActive(true);

        Debug.Log("End of Playtest Panel activated!");
    }

    public void OnNotTouchingPlayer()
    {
    }

    public void OnTouchingPlayer()
    {
    }
}
