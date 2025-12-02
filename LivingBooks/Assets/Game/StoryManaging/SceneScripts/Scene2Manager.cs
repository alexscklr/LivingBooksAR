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
        GlobalAudioManager.Instance.PlayAmbient("water", .5f);
    }

    // Update is called once per frame
    void Update() { }

    public void OnPathClicked(bool isCorrect)
    {
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
    }

    public void OnClickedOnCorrectPath()
    {
        ShowHint("Sehr gut! Das ist der richtige Weg.");
        //FinishSceneAfter(2f, "Super! Du hast den richtigen Weg gefunden.");
    }

    public void OnAnimatorExitComplete()
    {
        FinishSceneNow();

        GlobalAudioManager.Instance.StopAmbient();
        FinishSceneAfter(2f, "Super! Du hast den richtigen Weg gefunden.");
    }
}
