using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SceneRouter : MonoBehaviour
{
    [System.Serializable]
    public class SceneBinding
    {
        public string referenceImageName;
        public string sceneName;
    }

    [SerializeField]
    private ARTrackedImageManager manager;
    [SerializeField]
    private float unloadDelay = 1.5f;
    [SerializeField]
    [Header("Bindings between Scene and Image")]
    private List<SceneBinding> sceneBindings = new List<SceneBinding>();

    private Dictionary<string, string> _sceneByName;
    private HashSet<string> _loadedScenes = new HashSet<string>();
    private Dictionary<string, float> _lostTimers = new Dictionary<string, float>();

    private void Awake()
    {
        _sceneByName = new Dictionary<string, string>();
        foreach (var b in sceneBindings)
        {
            if (!string.IsNullOrEmpty(b.referenceImageName) && !string.IsNullOrEmpty(b.sceneName))
                _sceneByName[b.referenceImageName] = b.sceneName;
        }
    }

    private void Update()
    {
        foreach (var trackablesImage in manager.trackables)
        {

            if (trackablesImage == null || trackablesImage.trackingState != TrackingState.Tracking)
                continue;

            string refName = trackablesImage.referenceImage.name;
            if (string.IsNullOrEmpty(refName) || !_sceneByName.TryGetValue(refName, out var sceneName))
                continue;

            if (trackablesImage.trackingState == TrackingState.Tracking)
            {
                _lostTimers[sceneName] = 0f;
                if (!_loadedScenes.Contains(sceneName))
                    StartCoroutine(LoadScene(sceneName));
            }
            else
            {
                if (!_loadedScenes.Contains(sceneName)) 
                    continue;

                _lostTimers.TryGetValue(sceneName, out float t);
                t += Time.deltaTime;
                _lostTimers[sceneName] = t;

                if (t >= unloadDelay)
                    StartCoroutine(UnloadScene(sceneName));
            }
        }
    }

    private System.Collections.IEnumerator LoadScene(string sceneName)
    {
        var loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
       
        while (!loadOperation.isDone) 
            yield return null;

        _loadedScenes.Add(sceneName);
    }

    private System.Collections.IEnumerator UnloadScene(string sceneName)
    {
        var loadOperation = SceneManager.UnloadSceneAsync(sceneName);

        while (!loadOperation.isDone)
            yield return null;

        _loadedScenes.Remove(sceneName);
    }
}