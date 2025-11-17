using UnityEngine;

public class Scene1Manager : MonoBehaviour
{
    [Header("Referenzen")]
    [Tooltip("Das Spielfigur-GameObject in der Szene")]
    public GameObject fox;

    [Tooltip("Runtime Animator Controller für den Fuchs (build-sicher)")]
    public RuntimeAnimatorController foxAnimatorController;

    private Animator foxAnimator;

    private void Awake()
    {
        if (fox != null)
        {
            foxAnimator = fox.GetComponent<Animator>();
        }
        FindAnyObjectByType<HelperUIManager>().ShowHint("Szene 1: Hilf dem Fuchs, das Abenteuer zu bestehen!");
    }

    private void Start()
    {
        if (foxAnimator == null && fox != null)
            foxAnimator = fox.GetComponent<Animator>();

        if (foxAnimator != null && foxAnimatorController != null)
        {
            foxAnimator.runtimeAnimatorController = foxAnimatorController;
        }
    }

    public void OnFoxClicked()
    {
        if (foxAnimator != null)
            foxAnimator.SetTrigger("FoxClicked");
    }

    // Wird vom AnimatorExitListener (UnityEvent) aufgerufen
    // Dadurch kommuniziert der Listener nur nach oben zum Scene1Manager,
    // und NUR dieser spricht mit dem Story/Game-Management.
    public void OnAnimatorExitComplete()
    {
        // Optional: weitere Aufräum-/Abschlusslogik für Szene 1
        if (GameManager.Instance != null && GameManager.Instance.storyManager != null)
        {
            GameManager.Instance.OnStoryCompleted();
        }
    }
}
