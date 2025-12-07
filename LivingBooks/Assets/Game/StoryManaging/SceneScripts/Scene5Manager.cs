using UnityEngine;

public class Scene5Manager : SceneManagerBase
{
    [Header("Referenzen")]
    [Tooltip("Das Spielfigur-GameObject in der Szene")]
    public GameObject fox;

    [Tooltip("Runtime Animator Controller für den Fuchs")]
    public RuntimeAnimatorController foxAnimatorController;

    [Tooltip("Anzahl an zu sammelnden Blumen")]
    public int numberOfFlowersToCollect = 3;

    private Animator foxAnimator;

    void Start()
    {
        if (fox != null)
        {
            foxAnimator = fox.GetComponent<Animator>();
        }
        ShowHint("Szene 5: Sammle Blumen für Mia!");
        GlobalAudioManager.Instance.PlayNarrator("speaker5");
        GlobalAudioManager.Instance.PlayAmbient("forest", .5f);
    }

    public void OnClickedOnFlower(GameObject flowerObject)
    {
        // Deaktiviere das angeklickte Blumenobjekt
        flowerObject.SetActive(false);
        numberOfFlowersToCollect--;
        ShowHint("Blume gesammelt!");
        GlobalAudioManager.Instance.PlaySmallSound("collect");
        if (numberOfFlowersToCollect <= 0)
        {
            FinishSceneAfter(2f, "Toll! Du hast alle Blumen für Mia gesammelt.");
        }
    }
}
