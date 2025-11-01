using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class ARTapDebug : MonoBehaviour
{
    [Header("Hit Settings")]
    public LayerMask hittableLayers = ~0; // alle Layer standardmäßig
    public float maxDistance = 20f;
    [Tooltip("Wenn true, versucht bei Fehlschlag einen kleinen SphereCast (hilft bei kleinen Collidern).")]
    public bool useSphereFallback = true;
    public float sphereRadius = 0.05f;

    [Header("Behavior")]
    public bool ignoreIfPointerOverUI = true; // überspringt Tap wenn UI getroffen wurde
    public UnityEvent onTapped;

    [Header("Debug")]
    public bool logHits = true;
    public bool drawDebugRay = true;
    public Color debugRayColor = Color.cyan;
    public Color debugHitColor = Color.green;

    Camera cam;

    void Awake()
    {
        cam = Camera.main;
        if (cam == null)
        {
            // Versuch die aktive Kamera zu finden
            if (Camera.current != null) cam = Camera.current;
        }

        if (cam == null && logHits)
            Debug.LogWarning("[ARTapDebug] Keine Camera.main gefunden. Setze Camera manuell oder tagge AR Camera als MainCamera.");
    }

    void Update()
    {
        // UI check (Finger)
        if (ignoreIfPointerOverUI && IsTouchOverUI()) return;

        // Touch (Mobile)
        if (Input.touchCount > 0)
        {
            var t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
                ProcessPointer(t.position);
        }

        // Mouse (Editor/Test)
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            if (ignoreIfPointerOverUI && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;
            ProcessPointer(Input.mousePosition);
        }
#endif
    }

    void ProcessPointer(Vector2 screenPos)
    {
        if (cam == null) { if (logHits) Debug.LogWarning("[ARTapDebug] Keine Kamera verfügbar."); return; }

        Ray ray = cam.ScreenPointToRay(screenPos);
        if (drawDebugRay) Debug.DrawRay(ray.origin, ray.direction * maxDistance, debugRayColor, 2f);

        // 1) Normaler Raycast mit LayerMask
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, hittableLayers, QueryTriggerInteraction.Collide))
        {
            LogHit(hit, "Raycast");
            if (hit.collider.gameObject == gameObject)
            {
                if (logHits) Debug.Log($"[ARTapDebug] Direct hit on {name} via Raycast.");
                onTapped?.Invoke();
                if (drawDebugRay) Debug.DrawLine(hit.point, hit.point + Vector3.up * 0.05f, debugHitColor, 2f);
                return;
            }
        }

        // 2) Sphere fallback (hilft bei kleinen/fuzzy Collidern)
        if (useSphereFallback)
        {
            if (Physics.SphereCast(ray, sphereRadius, out RaycastHit sHit, maxDistance, hittableLayers, QueryTriggerInteraction.Collide))
            {
                LogHit(sHit, "SphereCast");
                if (sHit.collider.gameObject == gameObject)
                {
                    if (logHits) Debug.Log($"[ARTapDebug] Direct hit on {name} via SphereCast.");
                    onTapped?.Invoke();
                    if (drawDebugRay) Debug.DrawLine(sHit.point, sHit.point + Vector3.up * 0.05f, debugHitColor, 2f);
                    return;
                }
            }
        }

        if (logHits) Debug.Log($"[ARTapDebug] Tap at {screenPos} hit nothing relevant for {name}.");
    }

    void LogHit(RaycastHit hit, string method)
    {
        if (!logHits) return;
        var go = hit.collider != null ? hit.collider.gameObject.name : "<no collider>";
        Debug.Log($"[ARTapDebug] {method} hit: {go} (point {hit.point}) layer {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
    }

    bool IsTouchOverUI()
    {
        if (EventSystem.current == null) return false;

        // Touch
        if (Input.touchCount > 0)
        {
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }

        // Mouse pointer
#if UNITY_EDITOR
        return EventSystem.current.IsPointerOverGameObject();
#else
        return false;
#endif
    }

    // Visual debug: Collider bounds
    void OnDrawGizmosSelected()
    {
        if (!drawDebugRay) return;
        Gizmos.color = Color.yellow;
        var col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            if (col is BoxCollider box)
            {
                Gizmos.DrawWireCube(box.center, box.size);
            }
            else if (col is SphereCollider sph)
            {
                Gizmos.DrawWireSphere(sph.center, sph.radius);
            }
            else
            {
                // Generic bounds
                var b = col.bounds;
                Gizmos.DrawWireCube(b.center, b.size);
            }
        }
    }
}
