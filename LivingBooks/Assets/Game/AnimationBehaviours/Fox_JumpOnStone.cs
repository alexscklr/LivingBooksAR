using UnityEngine;

public class Fox_JumpOnStone : StateMachineBehaviour
{
    [Tooltip("Wie weit der Fuchs während der Animation nach vorne springen soll.")]
    public float jumpDistance = 1f;

    private float lastNormalizedTime = 0f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Deaktiviere Root Motion nur für die Dauer dieses States,
        // damit die manuelle Vorwärtsbewegung wirksam ist.
        animator.applyRootMotion = false;
        lastNormalizedTime = 0f;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Fortschritt der Animation (0–1) für diesen Frame
        float currentNormalizedTime = stateInfo.normalizedTime % 1f;
        float delta = currentNormalizedTime - lastNormalizedTime;

        // Vermeide negative Sprünge durch Looping
        if (delta < 0) delta = 1f + delta;

        // Bewege den Charakter
        animator.transform.position += animator.transform.forward * jumpDistance * delta;

        lastNormalizedTime = currentNormalizedTime;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Root Motion nach dem Sprung wieder aktivieren
        animator.applyRootMotion = true;
    }
}
