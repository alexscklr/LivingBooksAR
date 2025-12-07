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
        GlobalAudioManager.Instance.PlayNarrator("speaker5");
        GlobalAudioManager.Instance.PlayAmbient("forest", .5f);
    }

    public void OnKnocking(GameObject doorObject)
    {
        GlobalAudioManager.Instance.PlaySmallSound("door");
        // Deaktiviere das angeklickte Türobjekt
        doorObject.SetActive(false);
        ShowHint("Tür angeklopft!");
        FinishSceneAfter(2f, "Glückwunsch! Du hast den Schatz für den Fuchs gefunden.");
    }
}
