using UnityEngine;
using UnityEngine.Splines;
using System;

public class MoveOnSpline : MonoBehaviour
{
    [Header("Pfad und Objekt")]
    [Tooltip("Das zu bewegende Objekt.")]
    [SerializeField] private GameObject objectToMove;

    [Tooltip("GameObject, das die Spline-Komponente enth�lt.")]
    [SerializeField] private GameObject splineObject;
    private Spline splinePath;

    [Header("Bewegungseinstellungen")]
    [Tooltip("Dauer der Bewegung in Sekunden.")]
    [SerializeField] private float moveDuration = 2f;

    [Tooltip("Soll die Bewegung r�ckw�rts erfolgen?")]
    [SerializeField] private bool moveReverse = false;

    // Events f�r Start und Ende der Bewegung
    public event Action OnMoveStarted;
    public event Action OnMoveFinished;

    private float elapsed = 0f;
    private bool isMoving = false;

    void Start()
    {
        if (objectToMove == null)
            Debug.LogWarning($"{nameof(MoveOnSpline)}: Kein Objekt zugewiesen.");
        if (splineObject == null)
            Debug.LogWarning($"{nameof(MoveOnSpline)}: Kein Spline-Objekt zugewiesen.");
        else
        {
            splinePath = splineObject.GetComponent<SplineContainer>()?.Spline;
        }
        if (splinePath == null)
            Debug.LogWarning($"{nameof(MoveOnSpline)}: Keine Spline-Komponente am Spline-Objekt gefunden.");
    }


    void Update()
    {
        if (isMoving)
        {
            MoveObjectAlongSpline();
        }
    }

    public void TriggerMovingAlongPath()
    {
        Debug.Log("TriggerMovingAlongPath aufgerufen");
        splinePath = splineObject.GetComponent<SplineContainer>()?.Spline;
        if (objectToMove == null || splinePath == null){
            Debug.LogWarning("Bewegung nicht gestartet: Objekt: " + (objectToMove != null).ToString() + ", Path: " + (splinePath != null).ToString());
            return;
        }

        Debug.Log("Bewegung gestartet");
        elapsed = 0f;
        isMoving = true;
        OnMoveStarted?.Invoke();
    }

    public void StopMoving()
    {
        isMoving = false;
    }

    public void ResetMovement()
    {
        elapsed = 0f;
        isMoving = false;
        if (objectToMove != null && splinePath != null)
            objectToMove.transform.position = splinePath.EvaluatePosition(moveReverse ? 1f : 0f);
    }

    private void MoveObjectAlongSpline()
    {
        if (objectToMove == null || splinePath == null) return;

        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / moveDuration);
        if (moveReverse) t = 1f - t;

        // Position auf dem Spline berechnen
        Vector3 position = splinePath.EvaluatePosition(t);
        objectToMove.transform.position = position;

        // Rotation entlang der Tangente setzen
        Vector3 tangent = splinePath.EvaluateTangent(t);
        if (tangent != Vector3.zero)
            objectToMove.transform.rotation = Quaternion.LookRotation(tangent);

        if (t >= 1f)
        {
            isMoving = false;
            OnMoveFinished?.Invoke();
        }
    }

    // Getter
    public GameObject GetObjectToMove() => objectToMove;
    public Spline GetSplinePath() => splinePath;
    public float GetMoveDuration() => moveDuration;
    public bool GetMoveReverse() => moveReverse;

    // Setter
    public void SetObjectToMove(GameObject obj) => objectToMove = obj;
    public void SetSplinePath(Spline spline) => splinePath = spline;
    public void SetMoveDuration(float duration) => moveDuration = duration;
    public void SetMoveReverse(bool reverse) => moveReverse = reverse;
}
