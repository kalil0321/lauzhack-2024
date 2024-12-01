using UnityEngine;

public class LineRendererTo3D : MonoBehaviour
{
    [Header("Settings")]
    public LineRenderer lineRenderer;      // Assign the Line Renderer here
    public float wallHeight = 3f;          // Height of the walls
    public Material wallMaterial;         // Material for walls
    public Color wallColor = Color.black; // Color for wall segments
    public Color doorColor = Color.yellow; // Color for door segments
    public GameObject doorPrefab;         // Assign a door prefab here (optional)

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

        // Generate walls based on the Line Renderer points and colors
        for (int i = 0; i < pointCount - 1; i++) // Do not loop back to the first point
        {
            Vector3 start = points[i];
            Vector3 end = points[i + 1];

            // Check the color of the current segment
            Color segmentColor = lineRenderer.startColor; // Line Renderer can only have one color per segment in simple mode

            if (segmentColor == wallColor)
            {
                CreateWallSegment(floorPlan3D.transform, start, end);
            }
            else if (segmentColor == doorColor && doorPrefab != null)
            {
                PlaceDoor(floorPlan3D.transform, start, end);
            }
            else
            {
                // Default to creating a wall for any other color
                CreateWallSegment(floorPlan3D.transform, start, end);
            }
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

    void PlaceDoor(Transform parent, Vector3 start, Vector3 end)
    {
        // Calculate door position and orientation
        Vector3 direction = end - start;
        Vector3 position = start + direction / 2;
        Quaternion rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);

        // Instantiate the door prefab
        GameObject door = Instantiate(doorPrefab, position, rotation, parent);

        // Optionally adjust the door size and position to match the segment
        Vector3 doorScale = door.transform.localScale;
        doorScale.x = direction.magnitude; // Set door width to match the segment length
        door.transform.localScale = doorScale;
    }
}
