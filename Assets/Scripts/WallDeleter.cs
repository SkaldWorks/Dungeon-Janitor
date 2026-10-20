using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class WallDeleter : MonoBehaviour
{
    public List<GameObject> wallsToDelete;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        foreach (GameObject wall in wallsToDelete)
        {
            if (wall == null)
                continue;

            Renderer[] renderers =
                wall.GetComponentsInChildren<Renderer>(true);

            foreach (Renderer renderer in renderers)
            {
                renderer.shadowCastingMode =
                    ShadowCastingMode.ShadowsOnly;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        foreach (GameObject wall in wallsToDelete)
        {
            if (wall == null)
                continue;

            Renderer[] renderers =
                wall.GetComponentsInChildren<Renderer>(true);

            foreach (Renderer renderer in renderers)
            {
                renderer.shadowCastingMode =
                    ShadowCastingMode.On;
            }
        }
    }
}
