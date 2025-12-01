using UnityEngine;

public class Fox_JumpOnStone : StateMachineBehaviour
{
    [Tooltip("Wie weit der Fuchs während der Animation nach vorne springen soll.")]
    public float jumpDistance = 0.5f;

    private float lastFrameNormalizedTime = 0f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex
    )
    {
        // Setze den Fortschritt zurück, wenn der State neu betreten wird.
        lastFrameNormalizedTime = 0f;
    }

    // OnStateUpdate is called every frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex
    )
    {
        // Hole den normalisierten Fortschritt der Animation (0 am Anfang, 1 am Ende)
        float currentFrameNormalizedTime = stateInfo.normalizedTime;

        // Berechne den Fortschritt, der seit dem letzten Frame vergangen ist.
        // Wir klemmen den Wert, um Probleme bei Loops zu vermeiden.
        float deltaProgress = Mathf.Clamp01(currentFrameNormalizedTime - lastFrameNormalizedTime);

        // Berechne die Distanz, die in diesem Frame zurückgelegt werden soll
        float distanceThisFrame = jumpDistance * deltaProgress;

        // Bewege das GameObject im lokalen Raum nach vorne.
        // 'transform.forward' ist hier entscheidend, da es die "Vorwärts"-Richtung
        // relativ zur aktuellen Rotation des Objekts ist.
        animator.transform.Translate(animator.transform.forward * distanceThisFrame, Space.World);

        // Speichere den Fortschritt für den nächsten Frame
        lastFrameNormalizedTime = currentFrameNormalizedTime;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Optional: Hier könnte man sicherstellen, dass die Gesamtstrecke exakt zurückgelegt wurde,
        // aber bei diesem Ansatz ist das meist nicht nötig.
    }
}
