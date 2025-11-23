using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Verwaltet die Logik und die Prefabs für eine Story-Szene.
/// Ist verantwortlich für das Instanziieren, Aktualisieren und Zerstören von Szenenobjekten.
/// </summary>
public class StoryManager : MonoBehaviour
{
    // Hält die Referenz auf das aktuell aktive Story-Prefab
    private GameObject _activeStoryInstance;

    /// <summary>
    /// Spawnt oder aktualisiert das Story-Prefab an der Position des AR-Markers.
    /// </summary>
    /// <param name="trackedImage">Der AR-Marker, an den das Prefab gebunden wird.</param>
    /// <param name="storyPrefab">Das zu instanziierende Prefab.</param>
    public void SpawnOrUpdateStoryPrefab(ARTrackedImage trackedImage, GameObject storyPrefab)
    {
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            if (_activeStoryInstance == null)
            {
                // Instanziiere das Prefab, wenn es noch nicht existiert
                _activeStoryInstance = Instantiate(storyPrefab, trackedImage.transform);
                Debug.Log(
                    $"[StoryManager] Prefab '{storyPrefab.name}' für Marker '{trackedImage.referenceImage.name}' instanziiert."
                );
            }
            else
            {
                // Stelle sicher, dass das Prefab aktiv und korrekt positioniert ist
                if (!_activeStoryInstance.activeSelf)
                {
                    _activeStoryInstance.SetActive(true);
                }
                _activeStoryInstance.transform.SetPositionAndRotation(
                    trackedImage.transform.position,
                    trackedImage.transform.rotation
                );
            }
        }
        else
        {
            // Wenn das Tracking verloren geht, deaktiviere das Prefab
            if (_activeStoryInstance != null && _activeStoryInstance.activeSelf)
            {
                _activeStoryInstance.SetActive(false);
                Debug.Log(
                    $"[StoryManager] Prefab für Marker '{trackedImage.referenceImage.name}' wegen Tracking-Verlust deaktiviert."
                );
            }
        }
    }

    /// <summary>
    /// Zerstört das aktuell aktive Story-Prefab und räumt auf.
    /// </summary>
    public void DestroyCurrentStory()
    {
        if (_activeStoryInstance != null)
        {
            Debug.Log($"[StoryManager] Zerstöre Story-Prefab '{_activeStoryInstance.name}'.");
            Destroy(_activeStoryInstance);
            _activeStoryInstance = null;
        }
    }
}
