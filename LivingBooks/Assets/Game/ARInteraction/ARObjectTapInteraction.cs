using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class ARObjectTapInteraction : MonoBehaviour
{
    [Header("Tap Settings")]
    [Tooltip("Event wird ausgelöst, wenn auf das Objekt getippt wird.")]
    public UnityEvent onTapped;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Legacy Input (funktioniert ohne New Input System)
        // Maus (Editor/Standalone)
        if (Input.GetMouseButtonDown(0))
        {
            TryRaycastTap(Input.mousePosition);
        }

        // Touch (Mobile)
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                TryRaycastTap(touch.position);
            }
        }
    }

    void TryRaycastTap(Vector2 screenPos)
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        Ray ray = mainCamera.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                onTapped?.Invoke();
                Debug.Log($"[ARObjectTapInteraction] Tap hit {name}");
            }
        }
    }
}
