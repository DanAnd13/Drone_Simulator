using Mapbox.Unity.Map;
using Mapbox.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRMapMovement : MonoBehaviour
{
    [SerializeField] private AbstractMap _map;

    [Header("Movement")]
    public float speed = 0.0005f;
    public float altitudeStep = 1f;

    private Vector2d _position;
    private float _altitude;

    private float _velocityX;
    private float _velocityY;

    private void Start()
    {
        _position = _map.CenterLatitudeLongitude;
        _altitude = _map.Zoom;
    }

    private void Update()
    {
        // LEFT STICK (move)
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        // simple inertia (drone feeling)
        _velocityX = Mathf.Lerp(_velocityX, inputX, Time.deltaTime * 5f);
        _velocityY = Mathf.Lerp(_velocityY, inputY, Time.deltaTime * 5f);

        _position.x += _velocityY * speed;
        _position.y += _velocityX * speed;

        // RIGHT TRIGGER / SCROLL = altitude (zoom)
        float zoomInput = Input.GetAxis("Mouse ScrollWheel");
        _altitude += zoomInput * altitudeStep;

        _altitude = Mathf.Clamp(_altitude, 2f, 18f);

        // APPLY
        _map.UpdateMap(_position);
        _map.SetZoom(_altitude);
    }
}
