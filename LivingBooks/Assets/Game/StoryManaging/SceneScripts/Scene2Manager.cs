using UnityEngine;

public class Scene2Manager : SceneManagerBase
{
    private enum SceneStates
    {
        Start,
        StandingInMiddle,
        End,
    };

    private SceneStates currentState = SceneStates.Start;

    [Header("Referenzen")]
    [Tooltip("Das Spielfigur-GameObject in der Szene")]
    public GameObject fox;

    [Tooltip("Runtime Animator Controller für den Fuchs")]
    public RuntimeAnimatorController foxAnimatorController;

    private Animator foxAnimator;
    private bool onePathClicked = false;

    void Start()
    {
        currentState = SceneStates.Start;
        if (fox != null)
        {
            foxAnimator = fox.GetComponent<Animator>();
        }
        ShowHint("Szene 2: Hilf dem Fuchs, den richtigen Weg zu finden!");

        GlobalAudioManager.Instance.PlayNarrator("speaker2");
        GlobalAudioManager.Instance.PlayAmbient("forest", .05f);
    }

    // Update is called once per frame
    void Update() { }

    public void OnPathClicked(bool isCorrect)
    {
        if (GlobalAudioManager.Instance.IsNarratorPlaying)
            return;

        if (!onePathClicked)
        {
            foxAnimator.SetTrigger("onePathClicked");
            onePathClicked = true;
        }

        foxAnimator.SetBool("isCorrectPath", isCorrect);
        if (isCorrect)
        {
            OnClickedOnCorrectPath();
        }
        else
        {
            OnClickedOnWrongPath();
        }
    }

    public void OnClickedOnWrongPath()
    {
        ShowHint("Das ist der falsche Weg! Versuch es nochmal.");
        GlobalAudioManager.Instance.PlayNarrator("speakerWrongPath");
    }

    public void OnClickedOnCorrectPath()
    {
        ShowHint("Sehr gut! Das ist der richtige Weg.");
        GlobalAudioManager.Instance.PlayNarrator("speakerCorrectPath");
        //FinishSceneAfter(2f, "Super! Du hast den richtigen Weg gefunden.");
    }

    public void OnAnimatorExitComplete()
    {
        FinishSceneNow();
        if (GlobalAudioManager.Instance.IsNarratorFinished)
            FinishSceneAfter(2f, "Super! Du hast den richtigen Weg gefunden.");
    }
}
