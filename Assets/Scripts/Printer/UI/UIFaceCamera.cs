using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Metadata;

public class UIFaceCamera : MonoBehaviour
{
    private Transform _camera;
    private Transform _uiCanvas;

    private void Start()
    {
        if (_camera == null)
        {
            _camera = Camera.main.transform;
        }

        _uiCanvas = gameObject.transform;
    }

    private void LateUpdate()
    {
        if (_camera == null) return;

        Vector3 direction = _camera.position - _uiCanvas.position;

        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            _uiCanvas.rotation = Quaternion.LookRotation(direction);

            _uiCanvas.Rotate(0f, 180f, 0f);
        }
    }
}
