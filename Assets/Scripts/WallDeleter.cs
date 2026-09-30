using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class WallDeleter : MonoBehaviour
{
    public List<GameObject> wallsToDelete;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject wall in wallsToDelete)
            {
                if (wall != null)
                {
                    MeshRenderer[] renderers =
                        wall.GetComponentsInChildren<MeshRenderer>(true);

                    foreach (MeshRenderer renderer in renderers)
                    {
                        renderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject wall in wallsToDelete)
            {
                if (wall != null)
                {
                    MeshRenderer[] renderers =
                        wall.GetComponentsInChildren<MeshRenderer>(true);

                    foreach (MeshRenderer renderer in renderers)
                    {
                        renderer.shadowCastingMode = ShadowCastingMode.On;
                    }
                }
            }
        }
    }
}
