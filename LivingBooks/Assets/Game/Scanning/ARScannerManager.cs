using System;
using UnityEngine;

public class ARScannerManager : MonoBehaviour
{
    public event Action<string, Vector3, Quaternion> OnMarkerDetected;

    private bool scanningActive = false;

    public void EnableScanning(bool enable)
    {
        scanningActive = enable;
        Debug.Log("Scanning " + (enable ? "aktiviert" : "deaktiviert"));
    }

    // Diese Methode wird aus dem Bild-/QR-Erkennungssystem aufgerufen,
    // wenn ein Marker erkannt wurde
    public void MarkerFound(string markerID, Vector3 position, Quaternion rotation)
    {
        if (!scanningActive)
            return;

        Debug.Log($"Marker erkannt: {markerID} @ Position {position} with Rotation {rotation}");
        OnMarkerDetected?.Invoke(markerID, position, rotation);

        // Scanning kurz pausieren, bis Story abgeschlossen
        EnableScanning(false);
    }

    /// <summary>
    /// Simuliert das Finden eines Markers (für Testzwecke).
    /// </summary>
    public void SimulateMarkerFound(string markerID)
    {
        Vector3 simulatedPosition = Camera.main.transform.position + Camera.main.transform.forward * 0.5f;
        Quaternion simulatedRotation = Quaternion.identity;
        MarkerFound(markerID, simulatedPosition, simulatedRotation);
    }
}
