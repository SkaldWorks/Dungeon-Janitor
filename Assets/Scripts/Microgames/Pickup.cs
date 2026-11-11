using UnityEngine;

public class Pickup : MonoBehaviour, IMinigame
{
    private StartGame currentSource;
    public AudioClip pickupSound;

    public void StartMinigame(StartGame source)
    {
        currentSource = source;

        if (currentSource != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            currentSource.CompleteRepair();
            currentSource = null;
        }
    }

}
