using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MxInkHandler : StylusHandler
{
    public Color active_color = Color.black;
    public Color double_tap_active_color = Color.cyan;
    public Color default_color = Color.black;
    public GameObject blueprint;
    public Vector3 hitPosition;
    public bool hitWhite;
    public bool hitRed;
    public bool hitComplete;
    public Vector3 hitPosition2;
    [SerializeField]
    private InputActionReference _tipActionRef;
    [SerializeField]
    private InputActionReference _grabActionRef;
    [SerializeField]
    private InputActionReference _optionActionRef;
    [SerializeField]
    private InputActionReference _middleActionRef;
    private float _hapticClickDuration = 0.011f;
    private float _hapticClickAmplitude = 1.0f;
    public LayerMask buttonLayer;
    public bool hitButton;
    [SerializeField] private GameObject _tip;
    [SerializeField] private GameObject _cluster_front;
    [SerializeField] private GameObject _cluster_middle;
    [SerializeField] private GameObject _cluster_back;
    [SerializeField] private LayerMask planeLayer;  // Layer mask for the plane you want to raycast against
    [SerializeField] private float maxRaycastDistance = 100f;
    private void Awake()
    {
        _tipActionRef.action.Enable();
        _grabActionRef.action.Enable();
        _optionActionRef.action.Enable();
        _middleActionRef.action.Enable();

        InputSystem.onDeviceChange += OnDeviceChange;
        hitWhite = false;
        hitRed = false;
        hitButton = false;
        hitComplete = false;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device.name.ToLower().Contains("logitech"))
        {
            switch (change)
            {
                case InputDeviceChange.Disconnected:
                    _tipActionRef.action.Disable();
                    _grabActionRef.action.Disable();
                    _optionActionRef.action.Disable();
                    _middleActionRef.action.Disable();
                    break;
                case InputDeviceChange.Reconnected:
                    _tipActionRef.action.Enable();
                    _grabActionRef.action.Enable();
                    _optionActionRef.action.Enable();
                    _middleActionRef.action.Enable();
                    break;
            }
        }
    }
    void Update()
    {
        _stylus.inkingPose.position = transform.position;
        _stylus.inkingPose.rotation = transform.rotation;

        Ray stylusRay = new Ray(transform.position, transform.rotation * Vector3.forward);

        if(Physics.Raycast(stylusRay, out RaycastHit hitInfo, maxRaycastDistance, planeLayer))
        {
            // If the ray hits something, output the point of intersection
            //Debug.Log("Ray hit at: " + hitInfo.point);
            _stylus.tip_value = _tipActionRef.action.ReadValue<float>();
            _stylus.cluster_middle_value = _middleActionRef.action.ReadValue<float>();
            _stylus.cluster_front_value = _grabActionRef.action.IsPressed();
            _stylus.cluster_back_value = _optionActionRef.action.IsPressed();
            hitPosition = hitInfo.point;
            hitRed = false;
            hitWhite = false;
            hitButton = false;
        }
        else
        {
            if (Physics.Raycast(stylusRay, out RaycastHit hitInfo2, maxRaycastDistance, buttonLayer))
            {
                hitButton = true;
                _stylus.cluster_back_value = _optionActionRef.action.IsPressed();
                hitPosition2 = hitInfo2.point;
                if (hitInfo2.collider.CompareTag("SelW") && _stylus.cluster_back_value)
                {
                    hitWhite = true;
                    hitRed = false;
                }
                else if (hitInfo2.collider.CompareTag("SelR") && _stylus.cluster_back_value)
                {
                    hitRed = true;
                    hitWhite = false;
                }
                else if (_stylus.cluster_back_value)
                {
                    hitComplete = true;
                }
            }
            else {
                hitRed = false;
                hitWhite = false;
                hitButton = false;
            }
            // If no hit, log that the ray missed
            //Debug.Log("Ray did not hit any plane.");
            _stylus.tip_value = 0f;
            _stylus.cluster_middle_value = 0f;
            _stylus.cluster_front_value = false;
            _stylus.cluster_back_value = false;

        }

        //_stylus.tip_value = _tipActionRef.action.ReadValue<float>();
        //_stylus.cluster_middle_value = _middleActionRef.action.ReadValue<float>();
        //_stylus.cluster_front_value = _grabActionRef.action.IsPressed();
        //_stylus.cluster_back_value = _optionActionRef.action.IsPressed();

        _tip.GetComponent<MeshRenderer>().material.color = _stylus.tip_value > 0 ? active_color : default_color;
        _cluster_front.GetComponent<MeshRenderer>().material.color = _stylus.cluster_front_value ? active_color : default_color;
        _cluster_middle.GetComponent<MeshRenderer>().material.color = _stylus.cluster_middle_value > 0 ? active_color : default_color;
        _cluster_back.GetComponent<MeshRenderer>().material.color = _stylus.cluster_back_value ? active_color : default_color;
    }

    public void TriggerHapticPulse(float amplitude, float duration)
    {
        var device = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(_stylus.isOnRightHand ? UnityEngine.XR.XRNode.RightHand : UnityEngine.XR.XRNode.LeftHand);
        device.SendHapticImpulse(0, amplitude, duration);
    }

    public void TriggerHapticClick()
    {
        TriggerHapticPulse(_hapticClickAmplitude, _hapticClickDuration);
    }

    public override bool CanDraw()
    {
        return true;
    }
}
