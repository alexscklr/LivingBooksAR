using UnityEngine;

public class Scene3Manager : SceneManagerBase
{
    [Header("Referenzen")]
    [SerializeField]
    private GameObject[] foodItems;

    [Tooltip("Maximale Anzahl an Essensgegenständen, die eingesammelt werden können")]
    [SerializeField]
    private int numberOfFoodToCollect = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GlobalAudioManager.Instance.PlayNarrator("speaker3");
        GlobalAudioManager.Instance.PlayAmbient("forest", .5f);
    }

    // Update is called once per frame
    void Update() { }

    public void OnClickedOnFood(GameObject foodObject)
    {
        if (numberOfFoodToCollect > 0)
        {
            GlobalAudioManager.Instance.PlaySmallSound("food");
            // Deaktiviere das angeklickte Essensobjekt
            foodObject.SetActive(false);
            numberOfFoodToCollect--;
            ShowHint($"Essen gesammelt: {numberOfFoodToCollect}");
        }
        else if (numberOfFoodToCollect == 0)
        {
            GlobalAudioManager.Instance.StopNarrator();
            GlobalAudioManager.Instance.PlayNarrator("speakerFood");
            ShowHint("Du hast schon genug Essen gesammelt!");
            FinishSceneAfter(2f, "Fantastisch! Finn ist nun satt.");
        }
    }
}
