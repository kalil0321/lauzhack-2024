using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class VRControllerUI : MonoBehaviour
{
    public LineRenderer lineRenderer;      // Line Renderer for the ray
    public Transform rayOrigin;           // Origin point of the ray (e.g., controller)
    public float rayLength = 10f;         // Maximum ray length\
    public Transform controllerTransform;  // Transform of the controller (ray origin)


    void Update()
    {
        // Create a ray from the controller
        Ray ray = new Ray(controllerTransform.position, controllerTransform.forward);
        RaycastHit hit;


        // Update Line Renderer
        lineRenderer.SetPosition(0, rayOrigin.position);
        lineRenderer.SetPosition(1, rayOrigin.position + rayOrigin.forward * rayLength);

        // Check if the ray hits a UI element
        if (Physics.Raycast(ray, out hit, rayLength))
        {
            // If it's a UI element
            if (hit.collider.GetComponent<Selectable>())
            {
                // Highlight the UI element
                EventSystem.current.SetSelectedGameObject(hit.collider.gameObject);

                // Trigger interaction on button press
                if (Input.GetButtonDown("Submit"))
                {
                    ExecuteEvents.Execute(hit.collider.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
                }
            }
        }
    }
}
