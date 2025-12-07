using UnityEngine;

public class Scene1Manager : SceneManagerBase
{
    [Header("Referenzen")]
    [Tooltip("Das Spielfigur-GameObject in der Szene")]
    public GameObject fox;

    [Tooltip("Runtime Animator Controller für den Fuchs")]
    public RuntimeAnimatorController foxAnimatorController;

    private Animator foxAnimator;

    private void Awake()
    {
        if (fox != null)
        {
            foxAnimator = fox.GetComponent<Animator>();
        }
        ShowHint("Szene 1: Hilf dem Fuchs, das Abenteuer zu bestehen!");
    }

    private void Start()
    {
        if (foxAnimator == null && fox != null)
            foxAnimator = fox.GetComponent<Animator>();

        if (foxAnimator != null && foxAnimatorController != null)
        {
            foxAnimator.runtimeAnimatorController = foxAnimatorController;
        }

        GlobalAudioManager.Instance.PlayNarrator("speaker1");
        GlobalAudioManager.Instance.PlayAmbient("forest", .05f);
    }

    public void OnFoxClicked()
    {
        if (GlobalAudioManager.Instance.IsNarratorPlaying)
            return;
        if (foxAnimator != null)
            foxAnimator.SetTrigger("FoxClicked");
    }

    // Wird vom AnimatorExitListener (UnityEvent) aufgerufen
    // Dadurch kommuniziert der Listener nur nach oben zum Scene1Manager,
    // und NUR dieser spricht mit dem Story/Game-Management.
    public void OnAnimatorExitComplete()
    {
        FinishSceneNow();
        GlobalAudioManager.Instance.StopNarrator();
        GlobalAudioManager.Instance.StopAmbient();
    }
}
