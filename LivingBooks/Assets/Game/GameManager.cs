using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Zentraler Ablaufcontroller des Spiels.
/// Steuert Zustände, delegiert das Scannen und verwaltet die Story-Logik.
/// </summary>
public class GameManager : MonoBehaviour
{
    // === Singleton ===
    public static GameManager Instance { get; private set; }

    // === Referenzen auf andere Manager ===
    [Header("Managers")]
    public UIManager uiManager;
    public HelperUIManager helperUI;
    public StoryManager storyManager;

    [Tooltip("Router, der nur scannt und meldet")]
    public TrackedImagePrefabRouter prefabRouter;

    [Header("AR-Komponenten")]
    [Tooltip("Der ARTrackedImageManager von AR Foundation. Muss im Inspector zugewiesen werden.")]
    public ARTrackedImageManager trackedImageManager;

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

    [Header("Scanning Optionen")]
    [Tooltip("Verzögerung (Sekunden), bevor nach Story-Ende das Scannen erneut aktiviert wird")]
    [SerializeField]
    private float restartScanDelay = 1.5f;

    // Merkt sich den Marker der aktuellen Story
    private string _currentStoryMarkerName;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning(
                "Mehrere GameManager-Instanzen gefunden. Zerstöre diese neue Instanz."
            );
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        GoToStartMenu();
        if (prefabRouter != null)
        {
            prefabRouter.OnMarkerPrefabFound += HandleMarkerFound;
        }
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

        // Deaktiviere den Router UND den übergeordneten AR-Manager,
        // um jegliches Tracking im Hintergrund zu unterbinden.
        if (prefabRouter != null)
            prefabRouter.enabled = false;
        if (trackedImageManager != null)
            trackedImageManager.enabled = false;

        // Beim Zurückkehren ins Menü eine laufende Story beenden
        if (storyManager != null)
            storyManager.DestroyCurrentStory();
        _currentStoryMarkerName = null;
    }

    public void OnStartButtonPressed()
    {
        // Wir starten das Scannen über eine Coroutine, um eine saubere Trennung
        // vom Startmenü zu gewährleisten und sofortige Scans zu vermeiden.
        StartCoroutine(DelayedStartScanning());
    }

    public void StartScanning()
    {
        // Setze den Zustand und aktualisiere die UI jedes Mal, wenn diese Methode aufgerufen wird.
        // Das stellt sicher, dass die UI auch beim Neustart des Scannens nach einer Story korrekt ist.
        currentState = GameState.Scanning;
        uiManager.ShowStartMenu(false);
        uiManager.ShowPauseMenu(false);
        uiManager.ShowScanningUI(true);
        helperUI.ShowHint("Halte die Kamera auf ein Bild!");

        // Aktiviere zuerst den AR-Manager und dann unseren Router.
        if (trackedImageManager != null)
            trackedImageManager.enabled = true;
        if (prefabRouter != null)
            prefabRouter.enabled = true;

        Debug.Log("GameManager: Scanning gestartet.");
    }

    private IEnumerator DelayedStartScanning()
    {
        // Wir warten einen kurzen Moment, bevor wir den Router aktivieren.
        // Das verhindert, dass ein Marker, der zufällig schon im Bild ist,
        // sofort die nächste Story startet.
        yield return new WaitForSeconds(0.25f);

        // Nach der Verzögerung rufen wir die zentrale StartScanning-Methode auf.
        StartScanning();
        Debug.Log("GameManager: Scanning nach Verzögerung aktiviert.");
    }

    private void HandleMarkerFound(ARTrackedImage trackedImage, GameObject prefab)
    {
        var markerName = trackedImage.referenceImage.name;

        switch (currentState)
        {
            case GameState.Scanning:
                // Neuer Marker im Scanning-Modus gefunden -> Story starten
                Debug.Log(
                    $"GameManager: Neuer Marker '{markerName}' im Scanning-Modus gefunden. Starte Story."
                );
                StartStory(markerName, trackedImage, prefab);
                break;

            case GameState.InStory:
                // Ein Marker wurde gefunden, während bereits eine Story läuft
                if (markerName == _currentStoryMarkerName)
                {
                    // Es ist der Marker der aktuellen Story -> Szene stabil halten
                    storyManager.SpawnOrUpdateStoryPrefab(trackedImage, prefab);
                }
                else
                {
                    // Es ist ein anderer Marker -> ignorieren
                    Debug.Log(
                        $"GameManager: Anderer Marker '{markerName}' während Story '{_currentStoryMarkerName}' erkannt. Wird ignoriert."
                    );
                }
                break;

            // In allen anderen Zuständen (StartMenu, Paused, etc.) wird nichts getan.
            default:
                Debug.Log(
                    $"GameManager: Marker '{markerName}' im Zustand '{currentState}' erkannt. Wird ignoriert."
                );
                break;
        }
    }

    private void StartStory(string markerName, ARTrackedImage trackedImage, GameObject prefab)
    {
        currentState = GameState.InStory;
        _currentStoryMarkerName = markerName;

        uiManager.ShowScanningUI(false);
        helperUI.ShowHint($"Story für Marker '{markerName}' gestartet.");

        // Die Verantwortung zum Spawnen an den StoryManager übergeben
        storyManager.SpawnOrUpdateStoryPrefab(trackedImage, prefab);

        // Router kann weiterlaufen, um die Position der aktuellen Szene zu aktualisieren
        if (prefabRouter != null)
            prefabRouter.enabled = true;
    }

    public void OnStoryCompleted()
    {
        Debug.Log("GameManager: Story abgeschlossen – zurück zum Scanning.");

        if (storyManager != null)
        {
            storyManager.DestroyCurrentStory();
        }
        _currentStoryMarkerName = null;

        // Anstatt direkt das Scannen neu zu starten, deaktivieren wir zuerst den AR-Manager,
        // um seinen Zustand zurückzusetzen und "Geister-Marker" zu löschen.
        if (trackedImageManager != null)
            trackedImageManager.enabled = false;

        // Dann starten wir den Scan-Vorgang nach einer Verzögerung neu.
        StartCoroutine(RestartScanningAfterDelay());
    }

    private IEnumerator RestartScanningAfterDelay()
    {
        yield return new WaitForSeconds(restartScanDelay);
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
        currentState = GameState.InStory;
        uiManager.ShowPauseMenu(false);
        helperUI.HideHint();
    }

    public void EndGame()
    {
        currentState = GameState.Ended;
        Time.timeScale = 1f;
        uiManager.ShowEndScreen();
        helperUI.ShowHint("Danke, dass du Finn geholfen hast!");

        if (storyManager != null)
            storyManager.DestroyCurrentStory();
        _currentStoryMarkerName = null;
    }
}
