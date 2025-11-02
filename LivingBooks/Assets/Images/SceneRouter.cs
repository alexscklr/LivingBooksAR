using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SceneRouter : MonoBehaviour
{
    [SerializeField]
    private ARTrackedImageManager manager;
    [SerializeField]
    private ARScannerManager scanerManager;

    private void Awake()
    {
        
    }

    private void Update()
    {
        if(!manager)
            return;

        foreach (var trackablesImage in manager.trackables)
        {
            if (trackablesImage == null || trackablesImage.trackingState != TrackingState.Tracking)
                continue;

            if (trackablesImage.trackingState == TrackingState.Tracking && trackablesImage != null)
                scanerManager.MarkerFound(trackablesImage.referenceImage.name, trackablesImage.transform.localPosition, trackablesImage.transform.localRotation);
        }
    }
}