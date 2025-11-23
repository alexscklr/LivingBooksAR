using UnityEngine;

public class Scene4Manager : SceneManagerBase
{
    [System.Serializable]
    public struct StonePair
    {
        [Tooltip("Voreingestellter (inaktiver) Stein")]
        public GameObject preset;

        [Tooltip("Aktivierter Stein (wird sichtbar nach Klick)")]
        public GameObject activated;
    }

    [Header("Referenzen")]
    [Tooltip("Das Spielfigur-GameObject in der Szene")]
    public GameObject fox;

    [SerializeField]
    [Tooltip("Die Steine, die Finn überqueren muss (preset → activated)")]
    public StonePair[] stonePairs;

    [Tooltip("Runtime Animator Controller für den Fuchs")]
    public RuntimeAnimatorController foxAnimatorController;

    private Animator foxAnimator;

    void Start()
    {
        if (fox != null)
        {
            foxAnimator = fox.GetComponent<Animator>();
        }
        ShowHint("Szene 4: Hilf Finn, über den Fluss zu kommen!");

        foreach (var stonePair in stonePairs)
        {
            if (stonePair.preset != null)
                stonePair.preset.SetActive(true);
            if (stonePair.activated != null)
                stonePair.activated.SetActive(false);
        }
    }

    // Update is called once per frame
    private void CheckIfAllStonesActivated()
    {
        bool allActivated = true;
        foreach (var stonePair in stonePairs)
        {
            if (stonePair.activated == null || !stonePair.activated.activeSelf)
            {
                allActivated = false;
                break;
            }
        }

        if (allActivated)
        {
            OnAllStonesActivated();
        }
    }

    public void OnClickedOnStone(GameObject stoneObject)
    {
        // Finde das passende Stein-Paar und aktiviere den zweiten Stein
        foreach (var stonePair in stonePairs)
        {
            if (
                stonePair.preset == stoneObject
                && stonePair.preset != null
                && stonePair.preset.activeSelf
            )
            {
                stonePair.preset.SetActive(false);
                if (stonePair.activated != null)
                    stonePair.activated.SetActive(true);
                ShowHint("Gut gemacht! Weiter so.");
                CheckIfAllStonesActivated();
                return;
            }
        }
    }

    public void OnAllStonesActivated()
    {
        FinishSceneAfter(2f, "Fantastisch! Finn hat den Fluss überquert.");
    }
}
