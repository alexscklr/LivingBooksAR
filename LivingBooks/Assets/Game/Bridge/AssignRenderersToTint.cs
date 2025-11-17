using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables.Visuals;
using System.Collections.Generic;

[RequireComponent(typeof(XRTintInteractableVisual))]
public class AssignRenderersToTint : MonoBehaviour
{
    void Awake()
    {
        var tint = GetComponent<XRTintInteractableVisual>();
        if (tint == null) return;

        // Alle MeshRenderer in Child-Objekten finden
        var renderers = GetComponentsInChildren<MeshRenderer>(includeInactive: true);

        // Renderer per öffentlicher Property zuweisen
        tint.tintRenderers = new List<Renderer>(renderers);

        Debug.Log($"Assigned {renderers.Length} renderers to {tint.name}");
    }
}
