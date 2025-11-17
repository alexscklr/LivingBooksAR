using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TrackedImagePrefabRouter : MonoBehaviour
{
    [System.Serializable]
    public class Mapping
    {
        public string markerID;
        public GameObject prefab;
    }

    [Header("References")]
    [SerializeField]
    private ARTrackedImageManager manager;
    [SerializeField]
    [Tooltip("Meldet Marker-Erkennung an den GameFlow (wie zuvor SceneRouter)")]
    private ARScannerManager scanerManager;

    [Header("Per-Marker Prefabs")]
    [SerializeField]
    private List<Mapping> mappings;

    [Header("Options")]
    [Tooltip("Auch bei 'Limited' Tracking Zustand anzeigen (hilfreich zum Debuggen)")]
    [SerializeField]
    private bool showOnLimitedTracking = true;

    [Tooltip("Zusätzliche Debug-Logs in der Konsole ausgeben")]
    [SerializeField]
    private bool logDebug = true;

    // Built lookup for fast access
    private Dictionary<string, GameObject> _prefabByMarker;

    // Track spawned content per trackable id
    private Dictionary<TrackableId, GameObject> _spawnedById;
    // Einmaliges Melden beim Eintritt in (Limited/)Tracking
    private HashSet<TrackableId> _reportedIds;

    private HashSet<TrackableId> _warnedNoName;
    private HashSet<TrackableId> _warnedNoMapping;
    private bool _warnedNoManager;

    private void Awake()
    {
        _prefabByMarker = new Dictionary<string, GameObject>();
        _spawnedById = new Dictionary<TrackableId, GameObject>();
        _reportedIds = new HashSet<TrackableId>();
        _warnedNoName = new HashSet<TrackableId>();
        _warnedNoMapping = new HashSet<TrackableId>();

        if (mappings != null)
        {
            foreach (var m in mappings)
            {
                if (m == null || string.IsNullOrEmpty(m.markerID) || m.prefab == null)
                {
                    continue;
                }
                var key = m.markerID.Trim();
                _prefabByMarker[key] = m.prefab;
                if (logDebug)
                {
                    Debug.Log($"[Router] Mapping: '{key}' -> {m.prefab.name}");
                }
            }
        }
    }

    private void OnEnable()
    {
        // Ensure clean state
        foreach (var kv in _spawnedById)
        {
            if (kv.Value != null)
            {
                kv.Value.SetActive(false);
            }
        }
        _reportedIds?.Clear();
    }

    private void OnDisable()
    {
        // Do not destroy, keep for reuse; simply disable
        foreach (var kv in _spawnedById)
        {
            if (kv.Value != null)
            {
                kv.Value.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Versteckt alle aktuell gespawnten Inhalte (bleiben im Speicher für Wiederverwendung).
    /// </summary>
    public void HideAllContent()
    {
        if (_spawnedById == null)
            return;
        foreach (var kv in _spawnedById)
        {
            if (kv.Value != null && kv.Value.activeSelf)
            {
                kv.Value.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Versteckt Inhalte für einen bestimmten Marker-Namen (ReferenceImage.name, getrimmt).
    /// </summary>
    public void HideContentForMarker(string markerID)
    {
        if (manager == null || string.IsNullOrEmpty(markerID))
            return;

        string key = markerID.Trim();
        foreach (var img in manager.trackables)
        {
            if (img == null) continue;
            var name = img.referenceImage.name?.Trim();
            if (string.Equals(name, key, System.StringComparison.Ordinal))
            {
                var id = img.trackableId;
                if (_spawnedById != null && _spawnedById.TryGetValue(id, out var go) && go != null && go.activeSelf)
                {
                    go.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Zerstört die Instanz für einen Marker, sodass beim nächsten Tracking eine frische Instanz erzeugt wird.
    /// </summary>
    public void DestroyContentForMarker(string markerID)
    {
        if (manager == null || string.IsNullOrEmpty(markerID) || _spawnedById == null)
            return;

        string key = markerID.Trim();
        foreach (var img in manager.trackables)
        {
            if (img == null) continue;
            var name = img.referenceImage.name?.Trim();
            if (string.Equals(name, key, System.StringComparison.Ordinal))
            {
                var id = img.trackableId;
                if (_spawnedById.TryGetValue(id, out var go) && go != null)
                {
                    Destroy(go);
                }
                _spawnedById.Remove(id);
            }
        }
    }

    private void Update()
    {
        if (!ValidateManager())
            return;

        foreach (var img in manager.trackables)
        {
            if (img == null)
                continue;
            ProcessImage(img);
        }
    }

    private void TryReport(ARTrackedImage img)
    {
        if (img == null)
            return;
            
        if (img.trackingState != TrackingState.Tracking)
            return;

        var name = img.referenceImage.name?.Trim();
        if (string.IsNullOrEmpty(name))
            return;

        // Optional: Nur während des Scannens melden
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.currentState != GameManager.GameState.Scanning)
                return;
        }

        var pos = img.transform.position;
        var rot = img.transform.rotation;

        if (scanerManager != null)
        {
            if (logDebug)
            {
                Debug.Log($"[Router] Report '{name}' at state={img.trackingState}");
            }
            scanerManager.MarkerFound(name, pos, rot);
        }
    }

    // === Helpers ===
    private bool ValidateManager()
    {
        if (manager != null)
            return true;
        if (!_warnedNoManager)
        {
            Debug.LogWarning("[Router] Kein ARTrackedImageManager zugewiesen. Bitte im Inspector setzen.");
            _warnedNoManager = true;
        }
        return false;
    }

    private void ProcessImage(ARTrackedImage img)
    {
        var id = img.trackableId;
        var state = img.trackingState;

        if (!TryGetMarkerName(img, out var name))
        {
            HideSpawned(id);
            _reportedIds.Remove(id);
            WarnNoNameOnce(id);
            return;
        }

        if (QualifiesState(state))
        {
            ReportIfFirstTime(img, id);
            var go = EnsureInstanceFor(img, id, name);
            if (go != null)
            {
                ShowIfHidden(go, state);
            }
        }
        else
        {
            _reportedIds.Remove(id);
            HideIfVisible(id, state);
        }
    }

    private bool TryGetMarkerName(ARTrackedImage img, out string name)
    {
        name = img.referenceImage.name?.Trim();
        return !string.IsNullOrEmpty(name);
    }

    private bool QualifiesState(TrackingState state)
    {
        return state == TrackingState.Tracking || (showOnLimitedTracking && state == TrackingState.Limited);
    }

    private void ReportIfFirstTime(ARTrackedImage img, TrackableId id)
    {
        if (_reportedIds.Add(id))
        {
            TryReport(img);
        }
    }

    private GameObject EnsureInstanceFor(ARTrackedImage img, TrackableId id, string name)
    {
        if (_spawnedById.TryGetValue(id, out var go) && go != null)
            return go;

        if (!_prefabByMarker.TryGetValue(name, out var prefab) || prefab == null)
        {
            if (!_warnedNoMapping.Contains(id))
            {
                Debug.LogWarning($"[Router] Kein Prefab für Marker '{name}' konfiguriert.");
                _warnedNoMapping.Add(id);
            }
            return null;
        }

        go = Instantiate(prefab, img.transform.position, img.transform.rotation);
        go.transform.SetParent(img.transform, worldPositionStays: true);
        go.name = $"{prefab.name}__{name}";
        go.transform.localPosition += Vector3.up * 0.01f; // kleine Anhebung
        _spawnedById[id] = go;
        if (logDebug)
        {
            Debug.Log($"[Router] Instanziiert '{go.name}' unter Marker '{name}' (state: {img.trackingState}).");
        }
        return go;
    }

    private void ShowIfHidden(GameObject go, TrackingState state)
    {
        if (!go.activeSelf)
        {
            go.SetActive(true);
            if (logDebug)
            {
                Debug.Log($"[Router] Aktiviert '{go.name}' (state: {state}).");
            }
        }
    }

    private void HideIfVisible(TrackableId id, TrackingState state)
    {
        if (_spawnedById.TryGetValue(id, out var go) && go != null && go.activeSelf)
        {
            go.SetActive(false);
            if (logDebug)
            {
                Debug.Log($"[Router] Deaktiviert '{go.name}' (state: {state}).");
            }
        }
    }

    private void HideSpawned(TrackableId id)
    {
        if (_spawnedById.TryGetValue(id, out var obj) && obj != null)
        {
            obj.SetActive(false);
        }
    }

    private void WarnNoNameOnce(TrackableId id)
    {
        if (_warnedNoName.Contains(id))
            return;
        Debug.LogWarning($"[Router] Referenzbild hat keinen Namen (id: {id}). Bitte im Reference Image Library setzen.");
        _warnedNoName.Add(id);
    }
}
