using UnityEngine;

public class BrokenCount : MonoBehaviour
{
    [Header("Broken Objects")]
    [Tooltip("The GameObjects found with the Broken tag.")]
    public GameObject[] BrokenObjects;

    void Start()
    {
        recount();
    }

    /// <summary>
    /// Finds all GameObjects with the Broken tag and stores them in the BrokenObjects array.
    /// </summary>
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