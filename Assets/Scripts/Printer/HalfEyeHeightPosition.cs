using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfEyeHeightPosition : MonoBehaviour
{
    [SerializeField] private Transform _centerOfEye;
    [SerializeField] private Transform _targetObject;
    [SerializeField] private float heightOffsetFactor = 0.5f;

    private float initialX;
    private float initialZ;

    private void Start()
    {
        initialX = _targetObject.position.x;
        initialZ = _targetObject.position.z;
    }

    private void Update()
    {
        if (_centerOfEye == null) return;

        float newY = _centerOfEye.position.y * heightOffsetFactor;

        _targetObject.position = new Vector3(initialX, newY, initialZ);
    }
}
