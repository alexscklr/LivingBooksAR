using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Zentraler Ablaufcontroller des Spiels.
/// Steuert Zustände, delegiert das Scannen und verwaltet die Story-Logik.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Managers")]
    public UIManager uiManager;
    public HelperUIManager helperUI;
    public StoryManager storyManager;

    [Tooltip("Router, der nur scannt und meldet")]
    public TrackedImagePrefabRouter prefabRouter;

    [Header("AR-Komponenten")]
    [Tooltip("Der ARTrackedImageManager von AR Foundation. Muss im Inspector zugewiesen werden.")]
    public ARTrackedImageManager trackedImageManager;

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
    [SerializeField]
    private float restartScanDelay = 3f;

    private string _currentStoryMarkerName;
    private string _lastCompletedMarkerName;
    private float _suppressSameMarkerUntil = 0f;

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
        GoToStartMenu();
        if (prefabRouter != null)
            prefabRouter.OnMarkerPrefabFound += HandleMarkerFound;
    }

    public void GoToStartMenu()
    {
        currentState = GameState.StartMenu;
        uiManager.ShowStartMenu(true);
        uiManager.ShowScanningUI(false);
        helperUI.ShowHint("Tippe auf „Start“, um das Abenteuer zu beginnen!");

        if (prefabRouter != null)
            prefabRouter.enabled = false;
        if (trackedImageManager != null)
            trackedImageManager.enabled = false;

        if (storyManager != null)
            storyManager.DestroyCurrentStory();
        _currentStoryMarkerName = null;
    }

    public void OnStartButtonPressed()
    {
        StartCoroutine(DelayedStartScanning());
    }

    private IEnumerator DelayedStartScanning()
    {
        yield return new WaitForSeconds(0.25f);
        StartScanning();
    }

    public void StartScanning()
    {
        currentState = GameState.Scanning;
        uiManager.ShowStartMenu(false);
        uiManager.ShowPauseMenu(false);
        uiManager.ShowScanningUI(true);
        helperUI.ShowHint("Halte die Kamera auf ein Bild!");

        if (trackedImageManager != null)
            trackedImageManager.enabled = true;
        if (prefabRouter != null)
            prefabRouter.enabled = true;
    }

    private void HandleMarkerFound(ARTrackedImage trackedImage, GameObject prefab)
    {
        if (trackedImage.trackingState != TrackingState.Tracking)
            return;

        var markerName = trackedImage.referenceImage.name;

        // Unterdrücke kurzzeitig denselben Marker nach Story-Ende
        if (!CanStartMarker(markerName))
            return;

        // Wenn gleiche Story -> nur Update
        if (_currentStoryMarkerName == markerName)
        {
            storyManager.SpawnOrUpdateStoryPrefab(trackedImage, prefab);
            return;
        }

        // Alte Story zerstören, wenn eine lief
        if (_currentStoryMarkerName != null)
        {
            storyManager.DestroyCurrentStory();
        }

        _currentStoryMarkerName = markerName;
        StartStory(markerName, trackedImage, prefab);
    }

    private void StartStory(string markerName, ARTrackedImage trackedImage, GameObject prefab)
    {
        currentState = GameState.InStory;
        uiManager.ShowScanningUI(false);
        helperUI.ShowHint($"Story für Marker '{markerName}' gestartet.");
        storyManager.SpawnOrUpdateStoryPrefab(trackedImage, prefab);
    }

    private bool CanStartMarker(string markerName)
    {
        return _lastCompletedMarkerName != markerName || Time.time >= _suppressSameMarkerUntil;
    }

    public void OnStoryCompleted()
    {
        if (_currentStoryMarkerName != null)
        {
            _lastCompletedMarkerName = _currentStoryMarkerName;
            _suppressSameMarkerUntil = Time.time + 1.5f; // kurzzeitige Unterdrückung
        }

        _currentStoryMarkerName = null;

        if (storyManager != null)
            storyManager.DestroyCurrentStory();

        if (trackedImageManager != null)
        {
            trackedImageManager.enabled = false;
            StartCoroutine(ReenableTrackedImageManager());
        }
    }

    private IEnumerator ReenableTrackedImageManager()
    {
        yield return null; // einen Frame warten
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
