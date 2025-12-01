using UnityEngine;

public class Scene7Manager : SceneManagerBase
{
    [Header("Referenzen")]
    [Tooltip("Das Spielfigur-GameObject in der Szene")]
    public GameObject fox;

    [Tooltip("Runtime Animator Controller für den Fuchs")]
    public RuntimeAnimatorController foxAnimatorController;

    private Animator foxAnimator;

    void Start()
    {
        if (fox != null)
        {
            foxAnimator = fox.GetComponent<Animator>();
            foxAnimator.SetTrigger("FinalAnim"); //kann anders gennant werden, "fox_jump"
        }
        ShowHint("Szene 7: Happy End!");
        GlobalAudioManager.Instance.PlayNarrator("speaker7");
        GlobalAudioManager.Instance.PlayAmbient("final", .5f);
    }
}
