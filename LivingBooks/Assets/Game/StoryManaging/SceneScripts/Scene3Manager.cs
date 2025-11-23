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
    void Start() { }

    // Update is called once per frame
    void Update() { }

    public void OnClickedOnFood(GameObject foodObject)
    {
        if (numberOfFoodToCollect <= 0)
        {
            ShowHint("Du hast schon genug Essen gesammelt!");
            return;
        }

        // Deaktiviere das angeklickte Essensobjekt
        foodObject.SetActive(false);
        numberOfFoodToCollect--;

        ShowHint($"Essen gesammelt: {numberOfFoodToCollect}");

        if (numberOfFoodToCollect <= 0)
        {
            FinishSceneAfter(2f, "Fantastisch! Finn ist nun satt.");
        }
    }
}
