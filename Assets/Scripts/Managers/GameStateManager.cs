
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public enum GameState
    {
        Playing,
        Paused
    }

    [Header("Pause Menu")]
    public GameObject pausePanel;
    public string mainMenuSceneName = "MainMenu";

    public GameState CurrentState { get; private set; } = GameState.Playing;

    public bool IsPlaying => CurrentState == GameState.Playing;

    // Player movement is allowed only when playing with no game panel open.
    public bool CanPlayerMove => IsPlaying && !IsAnyGamePanelOpen();

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

    private bool IsAnyGamePanelOpen()
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

    public void PauseGame()
    {
        CurrentState = GameState.Paused;
        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        CurrentState = GameState.Playing;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void ReturnToMainMenu()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.SaveGame();

        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
