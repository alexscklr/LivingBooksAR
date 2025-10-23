using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class ImageRouter : MonoBehaviour
{
    [SerializeField] ARTrackedImageManager manager;
    public GameObject cubePrefab;
    public GameObject circlePrefab;

    void OnEnable() => manager.trackedImagesChanged += OnChanged;
    void OnDisable() => manager.trackedImagesChanged -= OnChanged;

    void OnChanged(ARTrackedImagesChangedEventArgs e)
    {
        foreach (var img in e.added) 
            Handle(img);
        foreach (var img in e.updated) 
            Handle(img);
    }

    void Handle(ARTrackedImage img)
    {
        if (img.trackingState != TrackingState.Tracking) return;

        string refName = img.referenceImage.name;
        GameObject prefab = refName switch
        {
            "1" => cubePrefab,
            "2" => circlePrefab,
            _ => null
        };
        if (prefab == null) return;

        if (img.transform.childCount == 0)
        {
            var go = Instantiate(prefab, img.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = new Vector3(img.size.x, 1f, img.size.y);
        }
        else
        {
            img.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}