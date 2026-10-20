using UnityEngine;

public class BrokenCount : MonoBehaviour
{
    public GameObject[] BrokenObjects;
    void Start()
    {
        recount();
    }

    public void recount()
    {
        BrokenObjects = GameObject.FindGameObjectsWithTag("Broken");

        if (BrokenObjects.Length == 0)
        {
            Debug.Log("No GameObjects found with that tag.");
            return;
        }

        foreach (GameObject obj in BrokenObjects)
        {
            Debug.Log("Found object: " + obj.name);
        }

    }
}
