using UnityEngine;

public class Scene2Manager : SceneManagerBase
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
        }
        ShowHint("Szene 2: Hilf dem Fuchs, den richtigen Weg zu finden!");
    }

    // Update is called once per frame
    void Update() { }

    public void OnClickedOnWrongPath()
    {
        ShowHint("Das ist der falsche Weg! Versuch es nochmal.");
    }

    public void OnClickedOnCorrectPath()
    {
        // ToDo: let fox move
        FinishSceneAfter(2f, "Super! Du hast den richtigen Weg gefunden.");
    }

    public void OnAnimatorExitComplete()
    {
        FinishSceneNow();
    }
}
