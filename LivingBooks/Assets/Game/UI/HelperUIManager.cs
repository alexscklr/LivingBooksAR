using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HelperUIManager : MonoBehaviour
{
    [Header("Helper UI Elements")]
    public GameObject helperPanel;
    public Button showHintButton;
    public TMP_Text helperText;
    public float autoHideDelay = 3f; // 0 = bleibt sichtbar

    private Coroutine currentRoutine;

    void Awake()
    {
        if (helperPanel)
            helperPanel.SetActive(false);
    }

    public void ShowHint(bool show)
    {
        if (show)
        {
            if (helperPanel)
                helperPanel.SetActive(true);
        }
        else
        {
            HideHint();
        }
    }

    public void ShowHint(string message)
    {
        ShowHint(message, 3f); // 0 = Standardverhalten: bleibt sichtbar
    }

    public void ShowHint(string message, float duration)
    {
        if (showHintButton)
            showHintButton.gameObject.SetActive(false);
        if (helperText)
            helperText.text = message;

        if (helperPanel)
            helperPanel.SetActive(true);

        // Stoppe vorherige Coroutine, sonst verschwindet der neue Hint zu früh
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        float delay = (duration > 0f) ? duration : autoHideDelay;

        if (delay > 0f)
            currentRoutine = StartCoroutine(HideAfterDelay(delay));
    }

    public void HideHint()
    {
        if (showHintButton)
            showHintButton.gameObject.SetActive(true);
        if (helperPanel)
            helperPanel.SetActive(false);
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideHint();
        currentRoutine = null;
    }
}
