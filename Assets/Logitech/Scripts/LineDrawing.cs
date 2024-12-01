using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LineDrawing : MonoBehaviour
{
    public GameObject plane; // Reference to the plane
    public Material lineMaterial; // Material for the line renderer
    public float gridSize = 5.0f; // Size of each grid square
    public RenderTexture renderTexture; // Render texture for capturing the drawing
    public string savePath = "Drawings"; // Directory for saving drawings
    private LineRenderer currentLine; // The current line being drawn
    private bool isDrawing = false; // Tracks whether the stylus is currently drawing
    public MxInkHandler Stylus;
    private List<GameObject> lines;

    void Start()
    {
        lines = new List<GameObject>();
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
    }

    void Update()
    {
        if (!Stylus) return;

        Vector3 stylusPosition = Stylus.hitPosition;

        if (IsStylusReadyToDraw())
        {
            if (!isDrawing)
            {
                StartNewLine(stylusPosition);
                isDrawing = true;
            }
            else
            {
                UpdateLine(stylusPosition);
            }
        }
        else
        {
            if (isDrawing)
            {
                isDrawing = false;
                currentLine = null;
            }
        }

        // Trigger save with a specific stylus button press
        if (Stylus.CurrentState.cluster_back_double_tap_value)
        {
            SaveDrawingAsPNG();
        }
    }

    bool IsStylusReadyToDraw()
    {
        return Mathf.Max(Stylus.CurrentState.tip_value, Stylus.CurrentState.cluster_middle_value) > 0;
    }

    void StartNewLine(Vector3 startPosition)
    {
        GameObject lineObject = new GameObject("Line");
        lines.Add(lineObject);
        currentLine = lineObject.AddComponent<LineRenderer>();
        currentLine.useWorldSpace = true;
        currentLine.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));
        currentLine.positionCount = 2;
        currentLine.startWidth = 0.05f;
        currentLine.endWidth = 0.05f;
        currentLine.SetPosition(0, startPosition);
        currentLine.SetPosition(1, startPosition); // Second point will be updated
    }

    void UpdateLine(Vector3 currentPosition)
    {
        if (currentLine != null)
        {
            currentLine.SetPosition(1, currentPosition);
        }
    }

    public void SaveDrawingAsPNG()
    {
        Camera drawingCamera = GetComponent<Camera>();
        if (!drawingCamera)
        {
            Debug.LogError("No camera attached for rendering.");
            return;
        }

        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        drawingCamera.targetTexture = renderTexture;
        drawingCamera.Render();
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();
        drawingCamera.targetTexture = null;

        byte[] bytes = texture.EncodeToPNG();
        string filePath = Path.Combine(savePath, $"Drawing_{System.DateTime.Now:yyyyMMdd_HHmmss}.png");
        File.WriteAllBytes(filePath, bytes);

        Debug.Log($"Drawing saved to {filePath}");
    }
}
