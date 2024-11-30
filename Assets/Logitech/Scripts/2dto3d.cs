// using UnityEngine;
// using System.Collections.Generic;

// public class FloorPlanExtruder : MonoBehaviour
// {
//     [Header("Settings")]
//     public PolygonCollider2D floorSketch;  // Assign a 2D floor plan with a PolygonCollider2D
//     public float wallHeight = 3f;          // Height of the walls
//     public Material wallMaterial;         // Optional material for the walls

//     void Start()
//     {
//         if (floorSketch == null)
//         {
//             Debug.LogError("Please assign a PolygonCollider2D with the floor plan!");
//             return;
//         }

//         // Generate the 3D mesh from the 2D floor sketch
//         Generate3DFloorPlan();
//     }

//     void Generate3DFloorPlan()
//     {
//         // Get the points from the PolygonCollider2D
//         Vector2[] points = floorSketch.points;

//         // Create a new GameObject for the 3D model
//         GameObject floorPlan3D = new GameObject("3D Floor Plan");
//         floorPlan3D.transform.position = floorSketch.transform.position;

//         // Generate walls
//         for (int i = 0; i < points.Length; i++)
//         {
//             Vector2 start = points[i];
//             Vector2 end = points[(i + 1) % points.Length]; // Loop back to the first point

//             // Create a wall segment between two points
//             CreateWallSegment(floorPlan3D.transform, start, end);
//         }
//     }

//     void CreateWallSegment(Transform parent, Vector2 start, Vector2 end)
//     {
//         // Calculate wall dimensions
//         Vector3 start3D = new Vector3(start.x, 0, start.y); // Convert 2D to 3D
//         Vector3 end3D = new Vector3(end.x, 0, end.y);
//         Vector3 direction = end3D - start3D;
//         float length = direction.magnitude;

//         // Create a wall object
//         GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
//         wall.transform.parent = parent;
//         wall.transform.position = start3D + direction / 2 + Vector3.up * wallHeight / 2; // Position wall in 3D space
//         wall.transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
//         wall.transform.localScale = new Vector3(0.1f, wallHeight, length); // Scale the wall (width, height, length)

//         // Apply material if available
//         if (wallMaterial != null)
//         {
//             Renderer renderer = wall.GetComponent<Renderer>();
//             renderer.material = wallMaterial;
//         }
//     }
// }
