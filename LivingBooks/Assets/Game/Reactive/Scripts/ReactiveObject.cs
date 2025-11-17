using UnityEngine;

public class ReactiveObject : MonoBehaviour
{
    [Tooltip("Animation Duration in seconds.")]
    public float moveDuration = 0.5f;

    [Tooltip("Offset between current position and targeted position, when triggered.")]
    public Vector3 moveOffset = new Vector3(0, 0.3f, 0);

    private bool shouldMove = false;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float moveElapsed = 0f;

    void Update()
    {
        if (shouldMove)
        {
            moveElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(moveElapsed / moveDuration);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            if (t >= 1f)
            {
                shouldMove = false;
            }
        }
    }

    public void TriggerMovement()
    {
        startPosition = transform.position;
        targetPosition = startPosition + moveOffset;
        moveElapsed = 0f;
        shouldMove = true;
    }
}