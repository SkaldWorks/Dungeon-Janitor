
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Buttons")]
    public Button continueButton;
    public Button newGameButton;
    public Button quitButton;

    [Header("Button Labels")]
    public string continueText = "Continue";
    public string newGameText = "New Game";
    public string quitText = "Quit";

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

        // Show Continue only if save data exists.
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(hasSave);
            SetAllButtonTexts(continueButton, continueText);
        }

        if (newGameButton != null)
        {
            SetAllButtonTexts(newGameButton, newGameText);
        }

        if (quitButton != null)
        {
            SetAllButtonTexts(quitButton, quitText);
        }
    }

    private void SetAllButtonTexts(Button button, string label)
    {
        if (button == null)
            return;

        // Includes all child TMP text objects, even nested children.
        TMP_Text[] texts =
            button.GetComponentsInChildren<TMP_Text>(true);

        foreach (TMP_Text textObject in texts)
        {
            textObject.text = label;
        }
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

    public void QuitGame()
    {
        Debug.Log("Quitting game...");

        Application.Quit();
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
