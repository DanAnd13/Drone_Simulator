using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera _cam;
    [SerializeField] private GameObject _objectToMove;

    [Header("Settings:")]
    [SerializeField]
    private float _navigationSpeed = 0.4f;

    [SerializeField]
    private float _shiftMultiplier = 2f;

    [SerializeField]
    private float _sensitivity = 0.2f;

    [SerializeField]
    private float _panSensitivity = 0.5f;

    [SerializeField]
    private bool _useMouseWheelZoom = true;

    [SerializeField]
    private float _mouseWheelZoomSpeed = 1.0f;

    private Vector3 _anchorPoint;
    private Quaternion _anchorRot;

    private bool _isPanning;

    private float _panX;
    private float _panY;
    private Vector3 _panComplete;

    private void Awake()
    {
        if (_cam == null)
        {
            _cam = GetComponent<Camera>();
        }

        Assert.IsNotNull(_cam, "Can't find camera reference");
    }

    private void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null)
        {
            return;
        }

        MousePanning(mouse);

        if (_isPanning)
        {
            return;
        }

        if (Mouse.current.rightButton.isPressed)
        {
            Vector3 move = Vector3.zero;
            float speed = _navigationSpeed * (Keyboard.current.shiftKey.isPressed ? _shiftMultiplier : 1f) * Time.deltaTime * 9.1f;

            if (Keyboard.current.wKey.isPressed)
            {
                move += Vector3.forward * speed;
            }

            if (Keyboard.current.sKey.isPressed)
            {
                move -= Vector3.forward * speed;
            }

            if (Keyboard.current.dKey.isPressed)
            {
                move += Vector3.right * speed;
            }

            if (Keyboard.current.aKey.isPressed)
            {
                move -= Vector3.right * speed;
            }

            if (Keyboard.current.eKey.isPressed)
            {
                move += Vector3.up * speed;
            }

            if (Keyboard.current.qKey.isPressed)
            {
                move -= Vector3.up * speed;
            }

            _objectToMove.transform.Translate(move);
        }

        if (mouse.rightButton.wasPressedThisFrame)
        {
            _anchorPoint = new Vector3(mouse.position.ReadValue().y, -mouse.position.ReadValue().x);
            _anchorRot = _objectToMove.transform.rotation;
        }

        if (mouse.rightButton.isPressed)
        {
            Quaternion rot = _anchorRot;
            Vector3 dif = _anchorPoint - new Vector3(mouse.position.ReadValue().y, -mouse.position.ReadValue().x);
            rot.eulerAngles += dif * _sensitivity;
            _objectToMove.transform.rotation = rot;
        }

        MouseWheeling();
    }

    //Zoom with mouse wheel
    private void MouseWheeling()
    {
        if (!_useMouseWheelZoom)
        {
            return;
        }

        float speed = 10 * (_mouseWheelZoomSpeed * (Keyboard.current.shiftKey.isPressed ? _shiftMultiplier : 1f) *
                            Time.deltaTime * 9.1f);
        float scrollDelta = Mouse.current?.scroll.ReadValue().y ?? 0f;

        Vector3 pos = _cam.transform.position;
        if (scrollDelta < 0f)
        {
            pos = pos - (transform.forward * speed);
            _cam.transform.position = pos;
        }

        if (scrollDelta > 0f)
        {
            pos = pos + (transform.forward * speed);
            _cam.transform.position = pos;
        }
    }

    private void MousePanning(Mouse mouse)
    {
        Vector2 mouseDelta = mouse.delta.ReadValue();
        _panX = -mouseDelta.x * _panSensitivity;
        _panY = -mouseDelta.y * _panSensitivity;
        _panComplete = new Vector3(_panX, _panY, 0);

        if (mouse.middleButton.wasPressedThisFrame)
        {
            _isPanning = true;
        }

        if (mouse.middleButton.wasReleasedThisFrame)
        {
            _isPanning = false;
        }

        if (_isPanning)
        {
            _cam.transform.Translate(_panComplete);
        }
    }
}
