using UnityEngine;

public class TestSceneManager : MonoBehaviour
{
    public GameObject bridge;
    public GameObject capsule;
    public HelperUIManager helperUI;

    void Start()
    {
        // Brücke und Capsule zu Beginn deaktivieren
        bridge.SetActive(false);
        capsule.SetActive(false);

        // Hinweis für Spieler
        helperUI = GameManager.Instance.helperUI;
        helperUI.ShowHint("Platziere die Brücke, um Finn den Weg zu ermöglichen.");
    }

    /// <summary>
    /// Wird aufgerufen, wenn Spieler die Brücke platzieren soll (z. B. Button)
    /// </summary>
    public void PlaceBridge()
    {
        if (bridge.activeSelf)
            return;

        bridge.SetActive(true);

        // Capsule aktivieren, sobald Brücke da ist
        capsule.SetActive(true);

        helperUI?.ShowHint("Ziehe Finn über die Brücke, um weiterzugehen!");
    }

    /// <summary>
    /// Wird aufgerufen, wenn Capsule das Ziel erreicht hat
    /// </summary>
    public void OnCapsuleEndReached()
    {
        helperUI?.ShowHint("Super! Finn ist sicher auf der anderen Seite!");
        GameManager.Instance.OnStoryCompleted(); // StoryScene beenden
    }
}
