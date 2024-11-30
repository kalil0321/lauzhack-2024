using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueprintController : MonoBehaviour
{
    public int numGridLines = 10; // Number of grid divisions
    public float gridSize = 10.0f; // Size of the grid (length and width of the plane)
    public GameObject plane; // Reference to the plane GameObject
    public Material lineMaterial; // Material for the grid lines
    public float planeSize = 10.0f; // Size of the plane (10x10 units)
    public GameObject pointPrefab; // Prefab of the point (e.g., small sphere or cube)

    // Start is called before the first frame update
    void Start()
    {
        gridSize *= plane.transform.localScale.x;
        InitGridLines();
        //DrawPointsAtGridCorners();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void InitGridLines()
    {
        if (plane == null)
        {
            Debug.LogError("Plane is not assigned!");
            return;
        }

        // Get the plane's position and size
        Vector3 planePosition = plane.transform.position;
        float halfGridSize = gridSize / 2.0f; // Half size for centering the grid
        float spacing = gridSize / (numGridLines); // Distance between lines

        // Draw vertical and horizontal lines
        for (int i = 0; i <= numGridLines; i++)
        {
            float offset = -halfGridSize + i * spacing;

            // Draw vertical line
            DrawLine(
                new Vector3(planePosition.x + offset, planePosition.y, planePosition.z - halfGridSize),
                new Vector3(planePosition.x + offset, planePosition.y, planePosition.z + halfGridSize)
            );

            // Draw horizontal line
            DrawLine(
                new Vector3(planePosition.x - halfGridSize, planePosition.y, planePosition.z + offset),
                new Vector3(planePosition.x + halfGridSize, planePosition.y, planePosition.z + offset)
            );
        }
    }

    void DrawLine(Vector3 start, Vector3 end)
    {
        // Create a new GameObject for the line
        GameObject lineObject = new GameObject("GridLine");
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

        // Set up the LineRenderer
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.startColor = Color.black;
        lineRenderer.endColor = Color.black;
        lineRenderer.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));
        lineRenderer.useWorldSpace = true;
    }
    void DrawPointsAtGridCorners()
    {
        float cellSize = planeSize / numGridLines;

        for (int i = 0; i <= numGridLines; i++)
        {
            for (int j = 0; j <= numGridLines; j++)
            {
                // Calculate the grid corner positions
                float x = -planeSize / 2 + i * cellSize;
                float z = -planeSize / 2 + j * cellSize;
                Vector3 gridPoint = new Vector3(x, 0, z);

                // Instantiate a point at each grid corner
                Instantiate(pointPrefab, gridPoint, Quaternion.identity);
            }
        }
    }
}