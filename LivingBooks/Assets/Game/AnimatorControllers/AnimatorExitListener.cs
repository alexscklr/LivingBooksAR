using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class AnimatorExitListener : MonoBehaviour
{
    private Animator animator;

    [Tooltip("Name der Ziel-Animation/State. Hinweis: 'Exit' ist KEIN Runtime-State.")]
    public string targetStateName;

    [Header("Optional: Statt Name per Tag erkennen")]
    [Tooltip("Wenn aktiviert, wird nach einem State mit diesem Tag gesucht statt nach dem Namen.")]
    public bool useStateTag = false;
    public string targetStateTag;

    [Header("Erweitert")]
    [Tooltip("Animator auf Elternobjekten suchen, falls nicht direkt vorhanden.")]
    public bool searchAnimatorInParents = false;

    [Tooltip("Animator-Layer, der überwacht werden soll (Standard: 0)")]
    public int layerIndex = 0;

    [Tooltip("Früher Exit zulassen (löst beim Verlassen des States aus).")]
    public bool invokeOnEarlyExit = true;

    [Tooltip("Mindest-NormalizedTime, ab der ein früher Exit akzeptiert wird (0..1).")]
    [Range(0f, 1f)]
    public float minNormalizedTimeForEarlyExit = 0.0f;

    public UnityEvent onExitTransitionComplete;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null && searchAnimatorInParents)
        {
            animator = GetComponentInParent<Animator>();
        }
        if (animator == null)
        {
            Debug.LogWarning(
                $"{nameof(AnimatorExitListener)}: Kein Animator auf diesem GameObject gefunden.",
                this
            );
            return;
        }

        if (!useStateTag && string.Equals(targetStateName, "Exit"))
        {
            Debug.LogWarning(
                $"{nameof(AnimatorExitListener)}: 'Exit' ist kein echter Animator-State. Bitte den Namen des letzten States angeben oder 'useStateTag' verwenden und im Animator einen passenden Tag setzen.",
                this
            );
        }

        StartCoroutine(CheckForExitTransitionComplete());
    }

    private IEnumerator CheckForExitTransitionComplete()
    {
        // 1) Warten bis Ziel-State aktiv ist (per Name ODER Tag)
        while (true)
        {
            var state = animator.GetCurrentAnimatorStateInfo(layerIndex);
            bool isTarget = useStateTag
                ? (!string.IsNullOrEmpty(targetStateTag) && state.IsTag(targetStateTag))
                : (!string.IsNullOrEmpty(targetStateName) && state.IsName(targetStateName));
            if (isTarget)
                break;
            yield return null;
        }

        // 2) Warten bis dieser State vollständig abgespielt ist und keine Transition mehr läuft
        float lastNormalizedTime = 0f;
        while (true)
        {
            var state = animator.GetCurrentAnimatorStateInfo(layerIndex);
            bool isTarget = useStateTag
                ? (!string.IsNullOrEmpty(targetStateTag) && state.IsTag(targetStateTag))
                : (!string.IsNullOrEmpty(targetStateName) && state.IsName(targetStateName));

            // Fortschritt merken, solange wir im Zielstate sind
            if (isTarget)
            {
                lastNormalizedTime = state.normalizedTime;
            }

            bool finished = state.normalizedTime >= 1.0f && !animator.IsInTransition(layerIndex);
            if (finished)
                break;

            // Früher Exit: Wir haben den Zielstate verlassen, bevor er vollständig war
            bool earlyExit =
                !isTarget
                && invokeOnEarlyExit
                && lastNormalizedTime >= minNormalizedTimeForEarlyExit;
            if (earlyExit)
                break;

            yield return null;
        }

        Debug.Log("Transition zum Exit State abgeschlossen!");
        onExitTransitionComplete.Invoke();
    }
}
