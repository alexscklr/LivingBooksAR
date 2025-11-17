using UnityEngine;

public class SimulateMarkerOnKey : MonoBehaviour
{
    public ARScannerManager scanner;

    public string markerID = "Marker1";

    public KeyCode triggerKey = KeyCode.M;

    private void Update()
    {
        if (scanner == null)
            return;
        if (Input.GetKeyDown(triggerKey))
        {
            scanner.SimulateMarkerFound(string.IsNullOrEmpty(markerID) ? "Marker1" : markerID);
            if (GameManager.Instance != null && GameManager.Instance.helperUI != null)
            {
                GameManager.Instance.helperUI.ShowHint($"Simulierter Marker: {markerID}");
            }
        }
    }
}
