using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class SpawnerListener : MonoBehaviour
{
    [Tooltip("Das ObjectSpawner-Objekt, das aufgerufen wird, wenn ein neues Objekt gespawnt wird.")]
    public ObjectSpawner spawner;

    [Tooltip("Reactive Object")]
    public GameObject reactiveObject;

    [Tooltip("Wird ausgelöst, sobald ein neues Objekt gespawnt wurde.")]
    public UnityEvent<GameObject> onObjectSpawned;

    void Start()
    {
        if (spawner != null)
        {
            spawner.objectSpawned += HandleObjectSpawned;
        }
        else
        {
            Debug.LogWarning($"{nameof(SpawnerListener)}: Kein Spawner zugewiesen.");
        }
    }

    void HandleObjectSpawned(GameObject spawned)
    {
        // MoveOnSpline-Komponente suchen
        var moveOnSpline = spawned.GetComponent<MoveOnSpline>();
        Console.WriteLine("HandleObjectSpawned: MoveOnSpline-Komponente gefunden: " + (moveOnSpline != null));
        if (moveOnSpline != null && reactiveObject != null)
        {
            moveOnSpline.SetObjectToMove(reactiveObject);
            moveOnSpline.TriggerMovingAlongPath();
        }

        // UnityEvent auslösen und das neu platzierte Objekt übergeben
        onObjectSpawned?.Invoke(spawned);
    }

    void OnDestroy()
    {
        if (spawner != null)
            spawner.objectSpawned -= HandleObjectSpawned;
    }
}
