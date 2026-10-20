using UnityEngine;

public class Pickup : MonoBehaviour, IMinigame
{
    private StartGame currentSource;

    public void StartMinigame(StartGame source)
    {
        currentSource = source;

        if (currentSource != null)
            {
                currentSource.CompleteRepair();
                currentSource = null;
            }
    }

}
