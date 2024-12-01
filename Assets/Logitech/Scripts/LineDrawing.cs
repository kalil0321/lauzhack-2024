using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawing : MonoBehaviour
{
    public GameObject plane; // Reference to the plane
    public Material lineMaterial; // Material for the line renderer
    public float gridSize = 5.0f;//10.0f; // Size of each grid square
    public LineTo3D buildingGen;
    private LineRenderer currentLine; // The current line being drawn
    private bool isDrawing = false; // Tracks whether the stylus is currently drawing
    public MxInkHandler Stylus;
    public GameObject pointer; // Prefab of the point (e.g., small sphere or cube)
    public GameObject pointPrefab; // Prefab of the point (e.g., small sphere or cube)
    public Vector3 epsilon = new Vector3(0f, 0f, -0.02f);
    private List<GameObject> lines;


    private Color[] colors;

    public int colorIndex = 0;

    void Start()
    {
        lines = new List<GameObject>();
        colors = new Color[] { Color.white, Color.red };
        pointer = Instantiate(pointPrefab, new Vector3(0, 0, 0), Quaternion.identity);
    }

    void Update()
    {
        //fix the flickering
        //changing colour
        //erasing
        //bring grid up and smaller
        //change height
        if (!Stylus) return;
        // Check if the stylus is ready to draw
        Vector3 stylusPosition = Stylus.hitPosition;
        pointer.transform.position = stylusPosition;
        // Snap the position to the nearest grid point
        Vector3 snappedPosition = FindNearestGridCorner(stylusPosition.x, stylusPosition.y);//bit hacky for now, change to z if necessary
        if (IsStylusReadyToDraw())
        {
            TriggerHaptics();
            if (!isDrawing)
            {
                // Start a new line
                StartNewLine(snappedPosition+epsilon);
                isDrawing = true;
            }
            else
            {
                // Update the second point of the current line
                UpdateLine(stylusPosition+epsilon);
            }
        }
        else
        {
            // Reset drawing state when the stylus is not ready to draw
            if (isDrawing)
            {
                isDrawing = false;
                UpdateLine(snappedPosition+epsilon);
                currentLine = null;
            }
        }
        if (Stylus.CurrentState.cluster_back_value)
        {
            TriggerHaptics();
            colorIndex = 1;
        }
        if (Stylus.CurrentState.cluster_front_value) {
            TriggerHaptics();
            colorIndex = 0;
        }
    }

    bool IsStylusReadyToDraw()
    {
        // Replace this with your stylus input check (e.g., analog input > 0)
        return Mathf.Max(Stylus.CurrentState.tip_value, Stylus.CurrentState.cluster_middle_value)>0; // Placeholder: Replace with actual stylus analog input
    }

    private void TriggerHaptics()
    {
        const float dampingFactor = 0.6f;
        const float duration = 0.01f;
        float middleButtonPressure = Stylus.CurrentState.cluster_middle_value * dampingFactor;
        Stylus.TriggerHapticPulse(middleButtonPressure, duration);
    }

    Vector3 FindNearestGridCorner(float x, float z)
    {
        float snappedX = Mathf.Round(4*x)/4;
        float snappedZ = Mathf.Round(4*z)/4;
        //fix this maths for grid snap
        //return new Vector3(snappedX, 0, snappedZ);
        buildingGen.lines = lines;
        return new Vector3(snappedX, snappedZ, plane.transform.position.z-0.05f);
    }

    void StartNewLine(Vector3 startPosition)
    {
        GameObject lineObject = new GameObject("Line");
        lines.Add(lineObject);
        currentLine = lineObject.AddComponent<LineRenderer>();
        currentLine.sortingLayerName = "DrawnLine";
        currentLine.sortingOrder = 1;
        currentLine.useWorldSpace = true;
        // Configure the LineRenderer
        currentLine.positionCount = 2;
        currentLine.startWidth = 0.05f;
        currentLine.endWidth = 0.05f;
        currentLine.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));
        currentLine.startColor = colors[colorIndex];
        currentLine.endColor = colors[colorIndex];

        // Set the initial points
        currentLine.SetPosition(0, startPosition);
        currentLine.SetPosition(1, startPosition); // Second point will be updated
    }

    void UpdateLine(Vector3 currentPosition)
    {
        if (currentLine != null)
        {
            // Update the second point of the line
            currentLine.SetPosition(1, currentPosition);
        }
    }
}


/*using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LineDrawing : MonoBehaviour
{
    private List<GameObject> _lines = new List<GameObject>();
    private LineRenderer _currentLine;
    private List<float> _currentLineWidths = new List<float>(); //list to store line widths

    [SerializeField] float _maxLineWidth = 0.1f;
    [SerializeField] float _minLineWidth = 0.05f;

    [SerializeField] Material _material;

    [SerializeField] private Color _currentColor;
    public Color CurrentColor
    {
        get { return _currentColor; }
        set
        {
            _currentColor = value;
        }
    }

    public float MaxLineWidth
    {
        get { return _maxLineWidth; }
        set { _maxLineWidth = value; }
    }

    private bool _lineWidthIsFixed = false;
    public bool LineWidthIsFixed
    {
        get { return _lineWidthIsFixed; }
        set { _lineWidthIsFixed = value; }
    }

    private bool _isDrawing = false;
    private bool _doubleTapDetected = false;

    [SerializeField]
    private float longPressDuration = 1.0f;
    private float buttonPressedTimestamp = 0;

    public MxInkHandler Stylus;

    private Vector3 _previousLinePoint;
    private const float _minDistanceBetweenLinePoints = 0.0005f;

    public float gridSize;

    private void StartNewLine()
    {
        var gameObject = new GameObject("line");
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        _currentLine = lineRenderer;
        _currentLine.positionCount = 0;
        _currentLine.material = _material;
        _currentLine.material.color = _currentColor;
        _currentLine.loop = false;
        _currentLine.startWidth = _minLineWidth;
        _currentLine.endWidth = _minLineWidth;
        _currentLine.useWorldSpace = true;
        _currentLine.alignment = LineAlignment.View;
        _currentLine.widthCurve = new AnimationCurve();
        _currentLineWidths = new List<float>();
        _currentLine.shadowCastingMode = ShadowCastingMode.Off;
        _currentLine.receiveShadows = false;
        _lines.Add(gameObject);
        _previousLinePoint = new Vector3(0, 0, 0);
    }
    private void TriggerHaptics()
    {
        const float dampingFactor = 0.6f;
        const float duration = 0.01f;
        float middleButtonPressure = Stylus.CurrentState.cluster_middle_value * dampingFactor;
        Stylus.TriggerHapticPulse(middleButtonPressure, duration);
    }
    private void AddPoint(Vector3 position, float width)
    {
        if (Vector3.Distance(position, _previousLinePoint) > _minDistanceBetweenLinePoints)
        {
            TriggerHaptics();
            _previousLinePoint = position;
            _currentLine.positionCount++;
            _currentLineWidths.Add(Math.Max(width * _maxLineWidth, _minLineWidth));
            _currentLine.SetPosition(_currentLine.positionCount - 1, position);

            //create a new AnimationCurve
            AnimationCurve curve = new AnimationCurve();

            //populate the curve with keyframes based on the widths list
            if (_currentLineWidths.Count > 1)
            {
                for (int i = 0; i < _currentLineWidths.Count; i++)
                {
                    curve.AddKey(i / (float)(_currentLineWidths.Count - 1), _currentLineWidths[i]);
                }
            }
            else
            {
                curve.AddKey(0, _currentLineWidths[0]);
            }

            //assign the curve to the widthCurve
            _currentLine.widthCurve = curve;
        }
    }

    private void RemoveLastLine()
    {
        GameObject lastLine = _lines[_lines.Count - 1];
        _lines.RemoveAt(_lines.Count - 1);

        Destroy(lastLine);
    }

    private void ClearAllLines()
    {
        foreach (var line in _lines)
        {
            Destroy(line);
        }
        _lines.Clear();
    }

    void Update()
    {

        if (!Stylus) return;

        float analogInput = Mathf.Max(Stylus.CurrentState.tip_value, Stylus.CurrentState.cluster_middle_value);
        if (analogInput > 0 && Stylus.CanDraw())
        {
            if (!_isDrawing)
            {
                StartNewLine();
                _isDrawing = true;
            }
            AddPoint(Stylus.hitPosition, _lineWidthIsFixed ? 1.0f : analogInput);
            //change point here
        }
        else
        {
            _isDrawing = false;
        }

        //Undo by double tapping or clicking on cluster_back button on stylus
        if (Stylus.CurrentState.cluster_back_double_tap_value ||
        Stylus.CurrentState.cluster_back_value)
        {
            if (_lines.Count > 0 && !_doubleTapDetected)
            {
                buttonPressedTimestamp = Time.time;
                RemoveLastLine();
            }
            _doubleTapDetected = true;
            if (_lines.Count > 0 && Time.time >= (buttonPressedTimestamp + longPressDuration))
            {
                ClearAllLines();
            }
        }
        else
        {
            _doubleTapDetected = false;
        }
    }
    public Vector3 FindNearestGridCorner(float x, float z)
    {
        // Snap x and z to the nearest multiple of gridSize
        float snappedX = Mathf.Round(x / gridSize) * gridSize;
        float snappedZ = Mathf.Round(z / gridSize) * gridSize;

        // Return the nearest corner as a Vector3
        return new Vector3(snappedX, 0, snappedZ);
    }
}
*/
