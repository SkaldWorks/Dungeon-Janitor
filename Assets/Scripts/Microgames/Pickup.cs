using UnityEngine;

public class Pickup : MonoBehaviour, IMinigame
{
    private StartGame currentSource;
    public AudioClip pickupSound;
    public AudioSource audioSource;
    public float volume = 0.25f;

    public void StartMinigame(StartGame source)
    {
        currentSource = source;

        if (currentSource != null)
        {
            audioSource.PlayOneShot(pickupSound, volume);
            currentSource.CompleteRepair();
            currentSource = null;
        }
    }

}
