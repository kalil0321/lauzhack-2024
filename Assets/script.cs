using UnityEngine;

namespace MyUniqueNamespace
{
    [RequireComponent(typeof(LineRenderer))]
    public class SquareLineRenderer : MonoBehaviour
    {
        public float squareSize = 5f;         // Size of the square
        public Color wallColor = Color.black; // Color for wall segments
        public Color doorColor = Color.yellow; // Color for door segments

        private LineRenderer lineRenderer;

        void Start()
        {
            // Initialize the LineRenderer
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.loop = true; // Ensure the square is closed
            lineRenderer.positionCount = 5; // 4 corners + 1 to close the loop

            // Define the square corners
            Vector3[] points = new Vector3[5];
            points[0] = new Vector3(0, 0, 0);                 // Bottom-left
            points[1] = new Vector3(squareSize, 0, 0);       // Bottom-right
            points[2] = new Vector3(squareSize, 0, squareSize); // Top-right
            points[3] = new Vector3(0, 0, squareSize);       // Top-left
            points[4] = points[0];                           // Close the square

            lineRenderer.SetPositions(points);

            // Set alternating colors (black, yellow, etc.)
            ApplySegmentColors();
        }

        void ApplySegmentColors()
        {
            // Ensure the LineRenderer supports per-segment colors
            lineRenderer.colorGradient = CreateColorGradient(new Color[] { wallColor, doorColor, wallColor, doorColor });
        }

        Gradient CreateColorGradient(Color[] colors)
        {
            Gradient gradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[colors.Length];
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[colors.Length];

            for (int i = 0; i < colors.Length; i++)
            {
                float t = (float)i / (colors.Length - 1); // Normalize between 0 and 1
                colorKeys[i] = new GradientColorKey(colors[i], t);
                alphaKeys[i] = new GradientAlphaKey(1.0f, t); // Fully opaque
            }

            gradient.SetKeys(colorKeys, alphaKeys);
            return gradient;
        }
    }
}

