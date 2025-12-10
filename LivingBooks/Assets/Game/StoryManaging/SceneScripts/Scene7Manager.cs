using UnityEngine;

public class Scene7Manager : SceneManagerBase
{
    [Header("Referenzen")]
    [Tooltip("Das Spielfigur-GameObject in der Szene")]
    public GameObject fox;
    public GameObject girl;

    [Tooltip("Runtime Animator Controller für den Fuchs")]
    public RuntimeAnimatorController foxAnimatorController;
    public RuntimeAnimatorController girlAnimatorController;

    private Animator foxAnimator;
    private Animator girlAnimator;

    void Start()
    {
        if (fox != null)
        {
            foxAnimator = fox.GetComponent<Animator>();
        }

        if (girl != null)
        {
            girlAnimator = girl.GetComponent<Animator>();
        }
        ShowHint("Szene 7: Happy End!");
        GlobalAudioManager.Instance.PlayNarrator("speaker7");
    }

    public void OnAnimatorExitComplete()
    {
        if (GlobalAudioManager.Instance.IsNarratorFinished)
            FinishSceneAfter(2f, "Happy End!");
    }
}
