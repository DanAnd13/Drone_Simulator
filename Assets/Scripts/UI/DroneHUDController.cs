using UnityEngine;
using TMPro;

public class DroneHUDController : MonoBehaviour
{
    [Header("Drone / Camera")]
    [SerializeField] private Transform _cameraTransform;

    [Header("Artificial Horizon")]
    [SerializeField] private RectTransform _rollImage;
    [SerializeField] private RectTransform _pitchImage;

    [Tooltip("Vertical UI movement in pixels for one degree of pitch.")]
    [SerializeField] private float _pitchPixelsPerDegree = 2f;

    [Tooltip("Maximum pitch displayed by the artificial horizon.")]
    [SerializeField] private float _pitchLimit = 30f;

    [Header("Compass")]
    [SerializeField] private RectTransform _compassImage;

    [Header("Altitude")]
    [SerializeField] private TMP_Text _altitudeText;
    [SerializeField] private float _altitudeMultiplier = 1f;

    [Header("Speed")]
    [SerializeField] private RectTransform _speedNeedle;

    private Vector3 _previousPosition;

    private void Start()
    {
        if (_cameraTransform != null)
        {
            _previousPosition = _cameraTransform.position;
        }
    }

    private void Update()
    {
        if (_cameraTransform == null)
        {
            return;
        }

        UpdateArtificialHorizon();
        UpdateCompass();
        UpdateAltitude();
        UpdateSpeed();

        _previousPosition = _cameraTransform.position;
    }

    private void UpdateArtificialHorizon()
    {
        Vector3 localEulerAngles = _cameraTransform.localEulerAngles;
        float pitch = NormalizeAngle(localEulerAngles.x);
        float roll = NormalizeAngle(localEulerAngles.z);
        pitch = Mathf.Clamp(pitch, -_pitchLimit, _pitchLimit);

        if (_pitchImage != null)
        {
            Vector2 position = _pitchImage.anchoredPosition;
            position.y = -pitch * _pitchPixelsPerDegree;
            _pitchImage.anchoredPosition = position;
        }

        if (_rollImage != null)
        {
            _rollImage.localRotation = Quaternion.Euler(0f, 0f, roll);
        }
    }

    private void UpdateCompass()
    {
        if (_compassImage == null)
        {
            return;
        }

        Vector3 forward = _cameraTransform.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.001f)
        {
            return;
        }

        forward.Normalize();
        float heading = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
        _compassImage.localRotation = Quaternion.Euler(0f, 0f, -heading);
    }

    private void UpdateAltitude()
    {
        if (_altitudeText == null)
        {
            return;
        }

        float altitude = _cameraTransform.position.y * _altitudeMultiplier;
        _altitudeText.text = $"{altitude:0} m";
    }

    private void UpdateSpeed()
    {
        if (_speedNeedle == null)
        {
            return;
        }

        float speedMetersPerSecond = Vector3.Distance(_cameraTransform.position, _previousPosition) / Mathf.Max(Time.deltaTime, 0.0001f);
        float speedKmh = speedMetersPerSecond * 3.6f;
        float needleAngle = -90f - speedKmh * 2f;
        _speedNeedle.localRotation = Quaternion.Euler(0f, 0f, needleAngle);
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f)
        {
            angle -= 360f;
        }

        return angle;
    }
}