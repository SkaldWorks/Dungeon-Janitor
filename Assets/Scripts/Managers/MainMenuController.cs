using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Buttons")]
    [Tooltip("The button used to continue a previously saved game.")]
    public Button continueButton;

    [Tooltip("The button used to start a new game.")]
    public Button newGameButton;

    [Tooltip("The button used to quit the game.")]
    public Button quitButton;

    [Header("Button Labels")]
    [Tooltip("The text displayed on the Continue button and its child text objects.")]
    public string continueText = "Continue";

    [Tooltip("The text displayed on the New Game button and its child text objects.")]
    public string newGameText = "New Game";

    [Tooltip("The text displayed on the Quit button and its child text objects.")]
    public string quitText = "Quit";

    [Header("Scene")]
    [Tooltip("The name of the scene loaded when continuing or starting a game.")]
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

    /// <summary>
    /// Continues a saved game if save data exists.
    /// Logs an error if the SaveManager is unavailable.
    /// Starts a new game if no save data exists.
    /// </summary>
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

    /// <summary>
    /// Resets the saved game data and loads the dungeon scene.
    /// Logs an error if the SaveManager is unavailable.
    /// </summary>
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

    /// <summary>
    /// Logs a message and quits the application.
    /// </summary>
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