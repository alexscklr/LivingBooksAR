using UnityEngine;
using UnityEngine.Splines;

public class AlignSplineToTransform : MonoBehaviour
{
    public SplineContainer splineContainer;

    void Start()
    {
        if (splineContainer == null)
            splineContainer = GetComponent<SplineContainer>();

        // Transformiere die Punkte in den lokalen Raum
        for (int i = 0; i < splineContainer.Splines.Count; i++)
        {
            var spline = splineContainer.Splines[i];
            for (int j = 0; j < spline.Count; j++)
            {
                var knot = spline[j];
                // Transformiere die Punkte mit der lokalen Rotation/Position des Prefabs
                var rotatedPos = transform.TransformPoint(knot.Position);
                knot.Position = rotatedPos;
                spline[j] = knot;
            }
        }
    }
}
