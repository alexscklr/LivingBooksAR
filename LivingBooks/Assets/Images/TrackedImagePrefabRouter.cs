using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Scannt nach AR-Bildern und meldet gefundene Marker-Prefab-Paare an den GameManager.
/// Dieses Skript instanziiert selbst keine Objekte mehr.
/// </summary>
public class TrackedImagePrefabRouter : MonoBehaviour
{
    // Event: Marker wurde erkannt und Prefab zugeordnet
    public event System.Action<ARTrackedImage, GameObject> OnMarkerPrefabFound;

    [System.Serializable]
    public class Mapping
    {
        public string markerID;
        public GameObject prefab;
    }

    [Header("References")]
    [SerializeField]
    private ARTrackedImageManager manager;

    [Header("Per-Marker Prefabs")]
    [SerializeField]
    private List<Mapping> mappings;

    [Header("Options")]
    [Tooltip("Zusätzliche Debug-Logs in der Konsole ausgeben")]
    [SerializeField]
    private bool logDebug = true;

    // Schneller Zugriff auf Prefabs über den Marker-Namen
    private Dictionary<string, GameObject> _prefabByMarkerName;

    private void Awake()
    {
        _prefabByMarkerName = new Dictionary<string, GameObject>();
        if (mappings != null)
        {
            foreach (var m in mappings)
            {
                if (m == null || string.IsNullOrEmpty(m.markerID) || m.prefab == null)
                {
                    continue;
                }
                var key = m.markerID.Trim();
                _prefabByMarkerName[key] = m.prefab;
                if (logDebug)
                {
                    Debug.Log($"[Router] Mapping registriert: '{key}' -> {m.prefab.name}");
                }
            }
        }
    }

    private void OnEnable()
    {
        if (manager != null)
        {
            manager.trackedImagesChanged += OnTrackedImagesChanged;
        }
    }

    private void OnDisable()
    {
        if (manager != null)
        {
            manager.trackedImagesChanged -= OnTrackedImagesChanged;
        }
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // Verarbeite alle neu erkannten und aktualisierten Bilder
        foreach (var trackedImage in eventArgs.added)
        {
            ProcessTrackedImage(trackedImage);
        }
        foreach (var trackedImage in eventArgs.updated)
        {
            ProcessTrackedImage(trackedImage);
        }
    }

    private void ProcessTrackedImage(ARTrackedImage trackedImage)
    {
        // Nur Bilder im Zustand "Tracking" oder "Limited" sind für uns relevant
        if (trackedImage.trackingState == TrackingState.None)
        {
            return;
        }

        var markerName = trackedImage.referenceImage.name;

        // Finde das passende Prefab für den erkannten Marker
        if (_prefabByMarkerName.TryGetValue(markerName, out var prefab))
        {
            if (logDebug)
            {
                Debug.Log(
                    $"[Router] Marker '{markerName}' mit Prefab '{prefab.name}' gefunden. Melde an GameManager."
                );
            }
            // Melde das Bild und das zugehörige Prefab an den GameManager
            OnMarkerPrefabFound?.Invoke(trackedImage, prefab);
        }
    }
}
