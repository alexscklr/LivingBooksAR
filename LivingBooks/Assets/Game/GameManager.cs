using UnityEngine;

/// <summary>
/// Zentraler Ablaufcontroller des Spiels.
/// Steuert Zustände, ruft die passenden Manager auf
/// (UI, ARScanner, Story, HelperUI) und verwaltet den Spielfluss.
/// </summary>
public class GameManager : MonoBehaviour
{
    // === Singleton ===
    public static GameManager Instance { get; private set; }

    // === Referenzen auf andere Manager ===
    [Header("Managers")]
    public UIManager uiManager;
    public HelperUIManager helperUI;
    public ARScannerManager arScanner;
    public StoryManager storyManager;

    // === Game State ===
    public enum GameState
    {
        None,
        StartMenu,
        Scanning,
        InStory,
        Paused,
        Ended,
    }

    public GameState currentState { get; private set; } = GameState.None;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Initialzustand
        GoToStartMenu();
    }

    // --------------------------------------------------------------
    // == Ablaufsteuerung ==
    // --------------------------------------------------------------

    public void GoToStartMenu()
    {
        currentState = GameState.StartMenu;

        uiManager.ShowStartMenu(true);
        uiManager.ShowScanningUI(false);
        helperUI.ShowHint("Tippe auf „Start“, um das Abenteuer zu beginnen!");
    }

    public void OnStartButtonPressed()
    {
        StartScanning();
    }

    public void StartScanning()
    {
        currentState = GameState.Scanning;

        uiManager.ShowStartMenu(false);
        uiManager.ShowPauseMenu(false);
        uiManager.ShowScanningUI(true);

        helperUI.ShowHint("Halte die Kamera auf ein Bild!");

        arScanner.EnableScanning(true);
        arScanner.OnMarkerDetected += HandleMarkerDetected;

        Debug.Log("GameManager: Scanning gestartet.");
    }

    private void HandleMarkerDetected(string markerID, Vector3 position, Quaternion rotation)
    {
        // Stoppe Scanning, starte Story
        arScanner.OnMarkerDetected -= HandleMarkerDetected;
        arScanner.EnableScanning(false);

        StartStory(markerID, position, rotation);
    }

    public void StartStory(string markerID, Vector3 position, Quaternion rotation)
    {
        currentState = GameState.InStory;
        uiManager.ShowScanningUI(false);

        helperUI.ShowHint("Hilf Finn, die Aufgabe zu lösen!");

        storyManager.LoadStoryScene(markerID);

        Debug.Log($"GameManager: Story für Marker {markerID} gestartet.");
    }

    public void OnStoryCompleted()
    {
        Debug.Log("GameManager: Story abgeschlossen – zurück zum Scanning.");

        helperUI.ShowHint("Sehr gut! Scanne das nächste Bild!");
        StartScanning();
    }

    public void PauseGame()
    {
        if (currentState != GameState.InStory)
            return;

        currentState = GameState.Paused;
        Time.timeScale = 0f;

        uiManager.ShowPauseMenu(true);
        helperUI.ShowHint("Spiel pausiert.");
    }

    public void ResumeGame()
    {
        if (currentState != GameState.Paused)
            return;

        Time.timeScale = 1f;
        uiManager.ShowPauseMenu(false);
        helperUI.HideHint();

        currentState = GameState.InStory;
    }

    public void EndGame()
    {
        currentState = GameState.Ended;

        Time.timeScale = 1f;
        uiManager.ShowEndScreen();

        helperUI.ShowHint("Danke, dass du Finn geholfen hast!");
    }
}
