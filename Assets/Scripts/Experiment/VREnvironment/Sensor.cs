using System;
using System.Collections;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Color _inactiveColor = Color.gray;
    [SerializeField] private Color _activeColor = Color.red;

    [Header("Activation")]
    [SerializeField] private float _activeDuration = 2f;

    public bool IsActive { get; private set; }
    public bool WasHit { get; private set; }

    public event Action<float, float> OnHit;
    public event Action OnMiss;

    private float _activationTime;
    private Coroutine _activationRoutine;

    private void Awake()
    {
        SetColor(_inactiveColor);
    }

    public void Activate()
    {
        if (IsActive)
        {
            return;
        }

        if (_activationRoutine != null)
        {
            StopCoroutine(_activationRoutine);
        }

        _activationRoutine = StartCoroutine(ActivationRoutine());
    }

    public void Interact()
    {
        if (!IsActive || WasHit)
        {
            return;
        }

        WasHit = true;

        float interactionTime = Time.realtimeSinceStartup;

        SetColor(_inactiveColor);

        OnHit?.Invoke(_activationTime, interactionTime);
    }

    private IEnumerator ActivationRoutine()
    {
        IsActive = true;
        WasHit = false;

        _activationTime = Time.realtimeSinceStartup;

        SetColor(_activeColor);

        yield return new WaitForSeconds(_activeDuration);

        IsActive = false;

        if (!WasHit)
        {
            OnMiss?.Invoke();
        }

        SetColor(_inactiveColor);

        _activationRoutine = null;
    }

    private void SetColor(Color color)
    {
        if (_renderer != null)
        {
            _renderer.material.color = color;
        }
    }
}