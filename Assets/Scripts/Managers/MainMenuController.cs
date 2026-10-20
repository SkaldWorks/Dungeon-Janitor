
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Buttons")]
    public Button continueButton;
    public TMP_Text continueButtonText;

    public Button newGameButton;
    public TMP_Text newGameButtonText;

    [Header("Scene")]
    public string dungeonSceneName = "Dungeon";

    private void Start()
    {
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        bool hasSave =
            SaveManager.Instance != null &&
            SaveManager.Instance.HasSaveData;

        if (continueButton != null)
            continueButton.gameObject.SetActive(hasSave);

        if (continueButtonText != null)
            continueButtonText.text = "Continue";

        if (newGameButtonText != null)
            newGameButtonText.text = "New Game";
    }

    public void ContinueGame()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManager was not found.");
            return;
        }

        if (!SaveManager.Instance.HasSaveData)
        {
            StartNewGame();
            return;
        }

        LoadDungeon();
    }

    public void StartNewGame()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError(
                "SaveManager was not found. " +
                "Make sure it exists in the main menu scene."
            );

            return;
        }

        SaveManager.Instance.ResetSaveData();
        LoadDungeon();
    }

    private void LoadDungeon()
    {
        if (string.IsNullOrWhiteSpace(dungeonSceneName))
        {
            Debug.LogError("Dungeon scene name has not been set.");
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(dungeonSceneName);
    }
}
