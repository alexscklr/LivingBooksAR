using UnityEngine;

public class Scene6Manager : SceneManagerBase
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
        ShowHint("Szene 6: Klopfe an Mia's Tür!");
    }

    public void OnKnocking(GameObject doorObject)
    {
        // Deaktiviere das angeklickte Türobjekt
        doorObject.SetActive(false);
        ShowHint("Tür angeklopft!");
        FinishSceneAfter(2f, "Glückwunsch! Du hast den Schatz für den Fuchs gefunden.");
    }
}
