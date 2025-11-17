using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;

public static class TrackedImageRegistry
{
    // Map: ReferenceImage.name -> aktuell getracktes ARTrackedImage
    private static readonly Dictionary<string, ARTrackedImage> _byName = new();

    public static void Set(string name, ARTrackedImage image)
    {
        if (string.IsNullOrEmpty(name) || image == null)
            return;
        _byName[name] = image;
    }

    public static void Remove(string name, ARTrackedImage image)
    {
        if (string.IsNullOrEmpty(name))
            return;
        if (_byName.TryGetValue(name, out var current) && current == image)
        {
            _byName.Remove(name);
        }
    }

    public static bool TryGet(string name, out ARTrackedImage image)
    {
        return _byName.TryGetValue(name, out image);
    }

    public static void Clear()
    {
        _byName.Clear();
    }
}
