using System.Collections.Generic;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    [System.Serializable]
    public class StoryMapping
    {
        public string markerID; // z. B. "Marker1"
        public GameObject storyPrefab; // z. B. StoryScene1 Prefab
    }

    [Header("Story Mapping")]
    public List<StoryMapping> storyMappings = new List<StoryMapping>();

    private GameObject currentStoryInstance;

    [Header("AR Placement Optionen")]
    public bool followMarkerRotation = false;
    [Tooltip("Wie schnell die Story-Szene der Marker-Position folgt, wenn Rotation nicht übernommen wird. 0 = direkt, 1 = sehr stark")]
    public float positionLerpPerFrame = 0.2f;

    /// <summary>
    /// Lädt die StoryScene zu einem Marker.
    /// </summary>
    public void LoadStoryScene(string markerID, Vector3 position, Quaternion rotation)
    {
        // Bestehende Story entfernen
        if (currentStoryInstance != null)
            Destroy(currentStoryInstance);

        // Suche StoryPrefab für Marker
        StoryMapping mapping = storyMappings.Find(m => m.markerID == markerID);
        if (mapping == null)
        {
            Debug.LogWarning($"StoryManager: Kein Prefab für Marker {markerID} gefunden!");
            return;
        }

        // Prefab instanziieren
        currentStoryInstance = Instantiate(mapping.storyPrefab, Vector3.zero, Quaternion.identity);
        currentStoryInstance.name = mapping.storyPrefab.name;

        // AR-Following: Entweder direkt parenten (inkl. Rotation) oder nur Position folgen lassen
        if (TrackedImageRegistry.TryGet(markerID, out var trackedImage) && trackedImage != null)
        {
            if (followMarkerRotation)
            {
                // Volle Pose vom Marker übernehmen
                currentStoryInstance.transform.SetParent(trackedImage.transform, false);
                currentStoryInstance.transform.localPosition = Vector3.zero;
                currentStoryInstance.transform.localRotation = Quaternion.identity;
            }
            else
            {
                // Szene bleibt aufrecht: Nicht parenten, nur Position folgen (Rotation bleibt wie initial)
                currentStoryInstance.transform.SetParent(this.transform, true);
                currentStoryInstance.transform.SetPositionAndRotation(
                    trackedImage.transform.position,
                    currentStoryInstance.transform.rotation
                );

                var follower = currentStoryInstance.GetComponent<ARTrackedPoseFollower>();
                if (follower == null)
                    follower = currentStoryInstance.AddComponent<ARTrackedPoseFollower>();
                follower.markerID = markerID;
                follower.followPosition = true;
                follower.followRotation = false; // explizit NICHT rotieren
                follower.positionLerpPerFrame = positionLerpPerFrame;
            }
        }
        else
        {
            // Kein aktives Tracking: an übergebener Weltpose platzieren
            currentStoryInstance.transform.SetParent(this.transform, true);
            currentStoryInstance.transform.SetPositionAndRotation(position, rotation);
        }

        // HelperUI Hinweis anzeigen
        GameManager.Instance.helperUI.ShowHint($"Story {markerID} gestartet!", 1f);

        Debug.Log($"StoryManager: {markerID} geladen.");
    }

    /// <summary>
    /// Wird aufgerufen, wenn die Story abgeschlossen ist.
    /// </summary>
    public void CompleteStory()
    {
        if (currentStoryInstance != null)
            Destroy(currentStoryInstance);

        GameManager.Instance.OnStoryCompleted();
    }
}
