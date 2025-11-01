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

    /// <summary>
    /// Lädt die StoryScene zu einem Marker.
    /// </summary>
    public void LoadStoryScene(string markerID)
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

        // Optional: Story-Szene parenten unter einem Empty GameObject z. B. "StoryRoot"
        currentStoryInstance.transform.SetParent(this.transform, false);

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
