using UnityEngine;
using UnityEngine.XR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;


public class XRCameraMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 1.0f;
    public XRNode inputSource = XRNode.LeftHand; // Can switch to RightHand

    private Vector2 inputAxis;
    private CharacterController character;
    private XRController controller;
    public GameObject rig;
    //public GameObject Stylus;

    void Start()
    {
        character = GetComponent<CharacterController>();
        controller = GetComponent<XRController>();
    }

    void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);

        // Calculate movement direction
        Vector3 direction = new Vector3(inputAxis.x, 0, inputAxis.y);
        // Transform direction to be relative to camera's forward direction
        direction = Camera.main.transform.TransformDirection(direction);
        direction.y = 0; // Keep movement on horizontal plane
        // Apply movement
        if (direction.magnitude > 0.1f) // Add dead zone
        {
            rig.transform.position += direction * moveSpeed * Time.deltaTime;
            //Stylus.transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }
}
