using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SceneRouter : MonoBehaviour
{
    [SerializeField]
    private ARTrackedImageManager manager;

    [SerializeField]
    private ARScannerManager scanerManager;

    // Track, welche Images aktuell im Status Tracking sind (um Mehrfachfeuer zu vermeiden)
    private readonly HashSet<TrackableId> _trackingNow = new HashSet<TrackableId>();

    private void OnEnable()
    {
        _trackingNow.Clear();
    }

    private void OnDisable()
    {
        _trackingNow.Clear();
    }

    private void Update()
    {
        if (manager == null)
            return;

        foreach (var img in manager.trackables)
        {
            if (img == null)
                continue;

            var id = img.trackableId;
            if (img.trackingState == TrackingState.Tracking)
            {
                // Nur melden, wenn der Name gesetzt ist und wir frisch in Tracking eingetreten sind
                var name = img.referenceImage.name;
                if (!string.IsNullOrEmpty(name))
                {
                    // Registry updaten, solange Tracking anliegt
                    TrackedImageRegistry.Set(name, img);
                    if (_trackingNow.Add(id))
                    {
                        TryReport(img);
                    }
                }
            }
            else
            {
                _trackingNow.Remove(id);
                var name = img.referenceImage.name;
                if (!string.IsNullOrEmpty(name))
                {
                    TrackedImageRegistry.Remove(name, img);
                }
            }
        }
    }

    private void TryReport(ARTrackedImage img)
    {
        if (img == null)
            return;
        if (img.trackingState != TrackingState.Tracking)
            return;

        // Name aus der ReferenceImageLibrary; ignorieren, wenn leer
        var name = img.referenceImage.name;
        if (string.IsNullOrEmpty(name))
            return;

        // Optional: Nur während des Scannens melden
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.currentState != GameManager.GameState.Scanning)
                return;
        }

        // Weltpose verwenden (nicht local), damit nach außen konsistent
        var pos = img.transform.position;
        var rot = img.transform.rotation;

        if (scanerManager != null)
        {
            scanerManager.MarkerFound(name, pos, rot);
        }
        if (GameManager.Instance != null && GameManager.Instance.helperUI != null)
        {
            GameManager.Instance.helperUI.ShowHint($"Marker erkannt: {name}");
        }
    }
}
