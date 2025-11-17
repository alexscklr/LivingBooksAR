using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

[RequireComponent(typeof(Collider))]
public class ARDragOnSpline : MonoBehaviour
{
    [Header("Spline Settings")]
    [Tooltip("Spline Container, der die Bahn enthält.")]
    public SplineContainer splineContainer;

    [Tooltip("Bewegtes Objekt – meist dieses GameObject.")]
    public GameObject objectToMove;

    [Header("Einstellungen")]
    [Tooltip("Wie schnell das Objekt der Bewegung folgen soll.")]
    public float followSpeed = 10f;

    private Camera mainCamera;
    private bool isDragging = false;
    private float currentT = 0f; // Fortschritt auf dem Spline (0–1)
    private float targetT = 0f;  // Zielposition beim Drag
    private Spline spline;

    void Start()
    {
        if (objectToMove == null)
            objectToMove = gameObject;

        if (splineContainer == null)
        {
            Debug.LogError("[ARDragOnSpline] Kein SplineContainer zugewiesen!");
            enabled = false;
            return;
        }

        spline = splineContainer.Spline;
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Touch- oder Mausklick-Events prüfen
        HandleInput();

        // Objekt weich zur Zielposition bewegen
        if (spline != null)
        {
            currentT = Mathf.Lerp(currentT, targetT, Time.deltaTime * followSpeed);
            Vector3 pos = spline.EvaluatePosition(currentT);
            Vector3 tangent = spline.EvaluateTangent(currentT);
            objectToMove.transform.SetPositionAndRotation(pos, Quaternion.LookRotation(tangent));
        }
    }

    private void HandleInput()
    {
#if UNITY_EDITOR
        // --- Maussteuerung für Tests im Editor ---
        if (Mouse.current.leftButton.wasPressedThisFrame)
            TryStartDrag(Mouse.current.position.ReadValue());

        if (Mouse.current.leftButton.isPressed && isDragging)
            UpdateDrag(Mouse.current.position.ReadValue());

        if (Mouse.current.leftButton.wasReleasedThisFrame)
            StopDrag();
#else
        // --- Touchsteuerung auf Mobilgerät ---
        if (Touchscreen.current == null)
            return;

        var touch = Touchscreen.current.primaryTouch;
        if (touch.press.wasPressedThisFrame)
            TryStartDrag(touch.position.ReadValue());

        if (touch.press.isPressed && isDragging)
            UpdateDrag(touch.position.ReadValue());

        if (touch.press.wasReleasedThisFrame)
            StopDrag();
#endif
    }

    private void TryStartDrag(Vector2 screenPos)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject == objectToMove)
                isDragging = true;
        }
    }

    private void UpdateDrag(Vector2 screenPos)
    {
        if (!isDragging || spline == null)
            return;

        // Ray auf Ebene oder Richtung Spline casten
        Ray ray = mainCamera.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Nächster Punkt auf der Spline finden (näherungsweise)
            float bestT = 0f;
            float bestDist = float.MaxValue;

            // Optional: einfache Sample-Suche (optimierbar)
            const int samples = 100;
            for (int i = 0; i <= samples; i++)
            {
                float t = i / (float)samples;
                Vector3 splinePos = spline.EvaluatePosition(t);
                float dist = Vector3.Distance(hit.point, splinePos);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestT = t;
                }
            }

            targetT = bestT;
        }
    }

    private void StopDrag()
    {
        isDragging = false;
    }
}
