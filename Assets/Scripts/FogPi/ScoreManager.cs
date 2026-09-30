using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
            
        }
        else
        {
            Destroy(this);
        }
    }
}
