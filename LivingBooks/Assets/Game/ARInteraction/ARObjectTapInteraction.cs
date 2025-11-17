using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

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
        // Touch input (mobile)
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;
            if (touch.press.wasPressedThisFrame)
            {
                Vector2 pos = touch.position.ReadValue();
                TryRaycastTap(pos);
            }
        }

        // Maus (Editor)
#if UNITY_EDITOR
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 pos = Mouse.current.position.ReadValue();
            TryRaycastTap(pos);
        }
#endif
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
