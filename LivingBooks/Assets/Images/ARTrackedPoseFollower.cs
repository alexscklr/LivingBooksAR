using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARTrackedPoseFollower : MonoBehaviour
{
    [Tooltip("Name des ReferenceImage in der Library, dem gefolgt werden soll.")]
    public string markerID;

    [Tooltip("Position vom Trackable übernehmen")] 
    public bool followPosition = true;

    [Tooltip("Rotation vom Trackable übernehmen (bei false bleibt die aktuelle Rotation erhalten)")] 
    public bool followRotation = false;

    [Range(0f, 1f)]
    [Tooltip("Smoothing pro Frame (0 = direkt, 1 = sehr stark)")]
    public float positionLerpPerFrame = 0.2f;

    private Transform _target; // cached transform des ARTrackedImage
    private Quaternion _initialRotation;

    private void Awake()
    {
        _initialRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        if (string.IsNullOrEmpty(markerID))
            return;

        if (!TrackedImageRegistry.TryGet(markerID, out ARTrackedImage tracked) || tracked == null)
            return;

        if (_target == null || _target != tracked.transform)
            _target = tracked.transform;

        // Position folgen
        if (followPosition)
        {
            Vector3 targetPos = _target.position;
            if (positionLerpPerFrame <= 0f)
            {
                transform.position = targetPos;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, targetPos, positionLerpPerFrame);
            }
        }

        // Rotation folgen (optional)
        if (followRotation)
        {
            transform.rotation = _target.rotation;
        }
        else
        {
            // Rotation eingefroren (auf initiale Rotation halten)
            transform.rotation = _initialRotation;
        }
    }
}
