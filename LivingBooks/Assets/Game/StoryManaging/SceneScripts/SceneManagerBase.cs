using UnityEngine;

public abstract class SceneManagerBase : MonoBehaviour
{
    private const string CompleteMethodName = nameof(CompleteAfterDelay);

    // Optional: zentrale Hint-Ausgabe für alle Szenen
    protected void ShowHint(string message)
    {
        if (string.IsNullOrEmpty(message))
            return;
        GameManager.Instance?.helperUI?.ShowHint(message);
    }

    // Sofort beenden, optional mit Hint
    protected void FinishSceneNow(string message = null)
    {
        if (!string.IsNullOrEmpty(message))
        {
            ShowHint(message);
        }
        GameManager.Instance?.OnStoryCompleted();
    }

    // Beenden nach Delay (Sekunden), optional mit Hint. Mehrfache Aufrufe werden zusammengeführt.
    protected void FinishSceneAfter(float delaySeconds, string message = null)
    {
        if (!string.IsNullOrEmpty(message) && !IsInvoking(CompleteMethodName))
        {
            ShowHint(message);
        }

        // Neu planen – weitere Aufrufe verschieben nur den Zeitpunkt, ohne doppelt zu beenden
        CancelInvoke(CompleteMethodName);
        Invoke(CompleteMethodName, Mathf.Max(0f, delaySeconds));
    }

    // Geplantes Beenden abbrechen (falls benötigt)
    protected void CancelPlannedFinish()
    {
        if (IsInvoking(CompleteMethodName))
        {
            CancelInvoke(CompleteMethodName);
        }
    }

    private void CompleteAfterDelay()
    {
        GameManager.Instance?.OnStoryCompleted();
    }

    protected virtual void OnDisable()
    {
        // Sicherstellen, dass keine geplanter Abschluss mehr aussteht, wenn die Szene/Objekt deaktiviert wird
        CancelPlannedFinish();
    }
}
