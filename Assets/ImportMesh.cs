using UnityEngine;
using System.Collections;
using System.IO;

public class ObjLoader : MonoBehaviour
{
    // Path to your .obj file (relative to Assets folder)
    public string objFilePath = "Models/yourModel.obj";

    void Start()
    {
        // Load the .obj file
        LoadObjFile();
    }

    void LoadObjFile()
    {
        // Get the full path
        string fullPath = Path.Combine(Application.dataPath, objFilePath);

        if (File.Exists(fullPath))
        {
            // Create a new GameObject to hold the imported model
            GameObject importedObject = new GameObject("ImportedObj");

            // Import the obj file
            Mesh mesh = new ObjImporter().ImportFile(fullPath);

            // Add MeshFilter and MeshRenderer components
            MeshFilter meshFilter = importedObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = importedObject.AddComponent<MeshRenderer>();

            // Assign the imported mesh to the MeshFilter
            meshFilter.mesh = mesh;

            // Create and assign a default material
            Material material = new Material(Shader.Find("Standard"));
            meshRenderer.material = material;

            // Center the object (optional)
            CenterObject(importedObject);
        }
        else
        {
            Debug.LogError("File not found at path: " + fullPath);
        }
    }

    void CenterObject(GameObject obj)
    {
        // Get the mesh bounds
        Bounds bounds = obj.GetComponent<MeshFilter>().mesh.bounds;

        // Center the object based on its bounds
        obj.transform.position = -bounds.center;
    }
}

// ObjImporter class to handle the actual importing
public class ObjImporter
{
    public Mesh ImportFile(string filePath)
    {
        // Create lists to store the imported data
        ArrayList vertices = new ArrayList();
        ArrayList triangles = new ArrayList();
        ArrayList normals = new ArrayList();
        ArrayList uvs = new ArrayList();

        // Read the file
        try
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("v ")) // Vertex
                    {
                        string[] parts = line.Split(' ');
                        vertices.Add(new Vector3(
                            float.Parse(parts[1]),
                            float.Parse(parts[2]),
                            float.Parse(parts[3])));
                    }
                    else if (line.StartsWith("f ")) // Face
                    {
                        string[] parts = line.Split(' ');
                        // Handle face data (triangles)
                        // Note: This is a simplified version
                        triangles.Add(int.Parse(parts[1].Split('/')[0]) - 1);
                        triangles.Add(int.Parse(parts[2].Split('/')[0]) - 1);
                        triangles.Add(int.Parse(parts[3].Split('/')[0]) - 1);
                    }
                    // Add handling for other elements (normals, UVs) as needed
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error importing OBJ file: " + e.Message);
            return null;
        }

        // Create and populate the mesh
        Mesh mesh = new Mesh();

        // Convert ArrayLists to arrays
        Vector3[] verticesArray = new Vector3[vertices.Count];
        int[] trianglesArray = new int[triangles.Count];

        for (int i = 0; i < vertices.Count; i++)
            verticesArray[i] = (Vector3)vertices[i];

        for (int i = 0; i < triangles.Count; i++)
            trianglesArray[i] = (int)triangles[i];

        // Assign arrays to mesh
        mesh.vertices = verticesArray;
        mesh.triangles = trianglesArray;

        // Recalculate normals
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}
