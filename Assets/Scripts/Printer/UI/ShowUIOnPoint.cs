using System.Collections;
using UnityEngine;

public class ShowUIOnPoint : MonoBehaviour
{
    [SerializeField] private Transform _camera;
    [Header("Objects to hide")]
    [SerializeField] private GameObject _mainUI;
    [Header("FootSteps detection settings")]
    [SerializeField] private float _checkInterval = 1f;
    [SerializeField] private float _sizeX = 1f;
    [SerializeField] private float _sizeY = 0.5f;

    private float _rayDistance = 3f;

    private Coroutine _footStepTracking;
    private bool _isUserInside = false;

    private void Start()
    {
        StartFootStepTracking(true);
    }

    private void StartFootStepTracking(bool isStart)
    {
        if (isStart)
        {
            if (_footStepTracking == null)
            {
                _footStepTracking = StartCoroutine(CheckPlayerPositionRoutine());
            }

            bool inside = CheckPlayerPositionImmediate();
            ApplyFootstepState(inside);
        }
        else
        {
            if (_footStepTracking != null)
            {
                StopCoroutine(_footStepTracking);
                _footStepTracking = null;
            }

            _isUserInside = false;
        }
    }

    private bool CheckPlayerPositionImmediate()
    {
        if (_camera == null)
            return false;

        Vector3 origin = _camera.position;
        Ray ray = new Ray(origin, Vector3.down);

        if (!Physics.Raycast(ray, out RaycastHit hit, _rayDistance))
            return false;

        return IsInsideZone(hit.point);
    }

    private IEnumerator CheckPlayerPositionRoutine()
    {
        while (true)
        {
            CheckPlayerPosition();
            yield return new WaitForSeconds(_checkInterval);
        }
    }

    private void CheckPlayerPosition()
    {
        if (_camera == null)
        {
            return;
        }

        Vector3 origin = _camera.position;
        Ray ray = new Ray(origin, Vector3.down);

        if (!Physics.Raycast(ray, out RaycastHit hit, _rayDistance))
        {
            HandlePlayerOut();
            return;
        }

        bool inside = IsInsideZone(hit.point);

        if (inside)
        {
            HandlePlayerInside();
        }
        else
        {
            HandlePlayerOut();
        }
    }

    private bool IsInsideZone(Vector3 point)
    {
        Vector3 center = transform.position;

        float dx = Mathf.Abs(point.x - center.x);
        float dz = Mathf.Abs(point.z - center.z);

        return dx <= (_sizeX / 2) && dz <= (_sizeY / 2);
    }

    private void ApplyFootstepState(bool inside)
    {
        if (inside)
        {
            _isUserInside = true;
            _mainUI.SetActive(true);
            Debug.Log("Player in zone");
        }
        else
        {
            _isUserInside = false;
            Debug.Log("Player out of zone");
            _mainUI.SetActive(false);
        }
    }

    private void HandlePlayerInside()
    {
        if (_isUserInside) return;
        ApplyFootstepState(true);
    }

    private void HandlePlayerOut()
    {
        if (!_isUserInside) return;
        ApplyFootstepState(false);
    }
}
