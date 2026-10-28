using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    /// <summary>
    /// The singleton instance of the GameStateManager.
    /// </summary>
    public static GameStateManager Instance { get; private set; }

    /// <summary>
    /// Defines the available states of the game.
    /// </summary>
    public enum GameState
    {
        Playing,
        Paused
    }

    [Header("Pause Menu")]
    [Tooltip("The UI panel displayed when the game is paused.")]
    public GameObject pausePanel;

    [Tooltip("The name of the scene loaded when returning to the main menu.")]
    public string mainMenuSceneName = "MainMenu";

    /// <summary>
    /// Gets the current state of the game.
    /// </summary>
    public GameState CurrentState { get; private set; } = GameState.Playing;

    /// <summary>
    /// Gets whether the game is currently in the Playing state.
    /// </summary>
    public bool IsPlaying => CurrentState == GameState.Playing;

    // Player movement is allowed only when playing with no game panel open.
    /// <summary>
    /// Gets whether the player is allowed to move.
    /// Movement is allowed only when the game is playing and no game panel is open.
    /// </summary>
    public bool CanPlayerMove => IsPlaying && !IsPanelOpen();

    private GameObject[] gamePanels;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        RefreshGamePanels();

        if (pausePanel != null)
            pausePanel.SetActive(false);

        ResumeGame();
    }

    private void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (CurrentState == GameState.Paused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    /// <summary>
    /// Finds all GameObjects tagged GamePanel and refreshes the gamePanels array.
    /// Includes inactive GameObjects when searching.
    /// </summary>
    public void RefreshGamePanels()
    {
        GameObject[] allObjects = FindObjectsByType<GameObject>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        System.Collections.Generic.List<GameObject> panels =
            new System.Collections.Generic.List<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj != null && obj.CompareTag("GamePanel"))
                panels.Add(obj);
        }

        gamePanels = panels.ToArray();
    }

    private bool IsPanelOpen()
    {
        if (gamePanels == null)
            RefreshGamePanels();

        if (gamePanels == null)
            return false;

        foreach (GameObject panel in gamePanels)
        {
            if (panel != null && panel.activeInHierarchy)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Pauses the game by setting the game state to Paused, stopping time,
    /// and displaying the pause panel if it is assigned.
    /// </summary>
    public void PauseGame()
    {
        CurrentState = GameState.Paused;
        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    /// <summary>
    /// Resumes the game by setting the game state to Playing, restoring the
    /// normal time scale, and hiding the pause panel if it is assigned.
    /// </summary>
    public void ResumeGame()
    {
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    /// <summary>
    /// Saves the game if a SaveManager instance exists, restores the normal
    /// time scale, and loads the scene specified by mainMenuSceneName.
    /// </summary>
    public void ReturnToMainMenu()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.SaveGame();

        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}