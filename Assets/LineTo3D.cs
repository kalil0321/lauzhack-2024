using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTo3D : MonoBehaviour
{
    [Header("Settings")]
    private LineRenderer lineRenderer;      // Assign the Line Renderer here
    public float wallHeight = 2f;          // Height of the walls
    public Material wallMaterial;         // Material for walls
    public Color wallColor = Color.white; // Color for wall segments
    public Color doorColor = Color.red; // Color for door segments
    public GameObject doorPrefab;         // Assign a door prefab here (optional)
    public Material doorMaterial;
    public List<GameObject> lines;
    public bool toDraw = false;
    public bool hasDrawn = false;
    public Material floorMaterial;

    void Start()
    {
        //if (lineRenderer == null)
        //{
        //    Debug.LogError("Please assign a Line Renderer!");
        //    return;
        //}
        wallColor = Color.white;
        doorColor = Color.red;
        toDraw = false; //turn into a button
        hasDrawn = false;
        //Generate3DFloorPlan();
    }

    void Update()
    {
        if (toDraw && !hasDrawn) {
            Generate3DFloorPlan();
            toDraw = false;
            hasDrawn = true;
        }
    }


    void Generate3DFloorPlan()
    {
        // Get the points from the Line Renderer
        float minX=1000;
        float maxX= -1000;
        float minZ = 1000;
        float maxZ = -1000;
        float allY=100;
        foreach (GameObject obj in lines) {
            lineRenderer = obj.GetComponent<LineRenderer>();
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
                Vector3 start = points[i]*4;
                start = swapYZ(start);
                Vector3 end = points[i + 1]*4;
                end = swapYZ(end);
                allY = start.y;
                if (start.x <= minX) {
                    minX = start.x;
                }
                if (start.x >= maxX) {
                    maxX = start.x;
                }
                if (end.x <= minX)
                {
                    minX = end.x;
                }
                if (end.x >= maxX)
                {
                    maxX = end.x;
                }
                if (start.z <= minZ)
                {
                    minZ = start.z;
                }
                if (start.z >= maxZ)
                {
                    maxZ = start.z;
                }
                if (end.z <= minZ)
                {
                    minZ = end.z;
                }
                if (end.z >= maxZ)
                {
                    maxZ = end.z;
                }
                // Check the color of the current segment
                Color segmentColor = lineRenderer.startColor; // Line Renderer can only have one color per segment in simple mode
                if (segmentColor == wallColor)
                {
                    CreateWallSegment(floorPlan3D.transform, start, end);
                }
                else if (segmentColor == doorColor) // && doorPrefab != null
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
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        float width = Mathf.Abs(maxX - minX);
        float depth = Mathf.Abs(maxZ - minZ);
        float maxSize = Mathf.Max(width, depth);
        plane.transform.position = new Vector3((minX+maxX)/2, allY, (minZ+maxZ)/2);
        plane.transform.localScale = new Vector3(maxSize/10, 1, maxSize/10);
        Renderer planeRenderer = plane.GetComponent<Renderer>();
        planeRenderer.material = floorMaterial;
        /*Vector2 textureScale = new Vector2(
                5.0f * plane.transform.localScale.x,
                5.0f * plane.transform.localScale.z
            );
        planeRenderer.material.mainTextureScale = textureScale;
        */

    }

    Vector3 swapYZ(Vector3 v) {
        Vector3 result = new Vector3(0, 0, 0);
        result.x = v.x;
        result.y = v.z;
        result.z = v.y;
        return result;
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
        float length = direction.magnitude;

        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.parent = parent;
        wall.transform.position = start + direction / 2 + Vector3.up * wallHeight / 2;
        wall.transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        wall.transform.localScale = new Vector3(0.1f, wallHeight, length); // Scale the wall (width, height, length)

        if (doorMaterial != null)
        {
            Renderer renderer = wall.GetComponent<Renderer>();
            renderer.material = doorMaterial;

        }
        /*
        Quaternion rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);

        // Instantiate the door prefab
        GameObject door = Instantiate(doorPrefab, position, rotation, parent);

        // Optionally adjust the door size and position to match the segment
        Vector3 doorScale = door.transform.localScale;
        doorScale.z = direction.magnitude; // Set door width to match the segment length
        door.transform.localScale = doorScale;
        */
    }
}
