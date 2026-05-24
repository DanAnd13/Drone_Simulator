using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRDroneInput : MonoBehaviour
{
    private VRControls _controls;

    private Vector2 _moveInput;
    private float _zoomInput;

    private void Awake()
    {
        _controls = new VRControls();
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    private void Start()
    {
        _controls.Drone.Move.performed += ctx =>
        {
            _moveInput = ctx.ReadValue<Vector2>();
        };

        _controls.Drone.Move.canceled += _ =>
        {
            _moveInput = Vector2.zero;
        };

        _controls.Drone.Zoom.performed += ctx =>
        {
            _zoomInput = ctx.ReadValue<float>();
        };
    }

    public Vector2 GetMove() => _moveInput;
    public float GetZoom() => _zoomInput;
}
