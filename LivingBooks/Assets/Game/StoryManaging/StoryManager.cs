using UnityEngine;

public class StoryManager : MonoBehaviour
{
    // Variante B: Prefabs werden über TrackedImagePrefabRouter per Marker-Name erzeugt

    /// <summary>
    /// Lädt die StoryScene zu einem Marker.
    /// </summary>
    public void LoadStoryScene(string markerID, Vector3 position, Quaternion rotation)
    {
        // Variante B: Instanziierung übernimmt ARTrackedImageManager per Router (TrackedImagePrefabRouter)
        // Daher hier KEINE manuelle Instanziierung mehr.

        // HelperUI Hinweis anzeigen
        GameManager.Instance.helperUI.ShowHint($"Story {markerID} gestartet!", 1f);

        Debug.Log($"StoryManager: {markerID} geladen.");
    }
}
