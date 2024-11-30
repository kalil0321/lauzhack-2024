using UnityEngine;

public class LineRendererTo3D : MonoBehaviour
{
    [Header("Settings")]
    public LineRenderer lineRenderer;  // Assign the Line Renderer here
    public float wallHeight = 3f;      // Height of the walls
    public Material wallMaterial;     // Optional material for the walls

    void Start()
    {
        if (lineRenderer == null)
        {
            Debug.LogError("Please assign a Line Renderer!");
            return;
        }

        Generate3DFloorPlan();
    }

    void Generate3DFloorPlan()
    {
        // Get the points from the Line Renderer
        int pointCount = lineRenderer.positionCount;
        if (pointCount < 2)
        {
            Debug.LogError("The Line Renderer must have at least 2 points to create a 3D plan.");
            return;
        }

        Vector3[] points = new Vector3[pointCount];
        lineRenderer.GetPositions(points);

        // Create a new GameObject to hold the 3D floor plan
        GameObject floorPlan3D = new GameObject("3D Floor Plan");

        // Generate walls based on the Line Renderer points
        for (int i = 0; i < pointCount; i++)
        {
            Vector3 start = points[i];
            Vector3 end = points[(i + 1) % pointCount]; // Loop back to the first point if needed

            CreateWallSegment(floorPlan3D.transform, start, end);
        }
    }

    void CreateWallSegment(Transform parent, Vector3 start, Vector3 end)
    {
        // Calculate wall dimensions
        Vector3 direction = end - start;
        float length = direction.magnitude;

        // Create a wall object
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.parent = parent;
        wall.transform.position = start + direction / 2 + Vector3.up * wallHeight / 2; // Position in 3D space
        wall.transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        wall.transform.localScale = new Vector3(0.1f, wallHeight, length); // Scale the wall (width, height, length)

        // Apply material if available
        if (wallMaterial != null)
        {
            Renderer renderer = wall.GetComponent<Renderer>();
            renderer.material = wallMaterial;
        }
    }
}
