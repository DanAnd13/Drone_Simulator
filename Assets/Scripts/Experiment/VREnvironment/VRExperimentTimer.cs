using System;
using System.Collections;
using UnityEngine;

public class VRExperimentTimer : MonoBehaviour
{
    [Header("Experiment")]
    [SerializeField] private float _experimentDuration = 30f;

    [Header("Sensor Activation")]
    [SerializeField] private float _activationInterval = 3f;

    public bool IsRunning { get; private set; }

    public event Action OnExperimentFinished;
    public event Action OnActivationInterval;

    private Coroutine _experimentRoutine;
    private Coroutine _activationRoutine;

    public void StartExperimentTimer()
    {
        if (IsRunning)
        {
            return;
        }

        IsRunning = true;

        _experimentRoutine = StartCoroutine(ExperimentRoutine());
        _activationRoutine = StartCoroutine(ActivationRoutine());
    }

    public void Stop()
    {
        if (_experimentRoutine != null)
        {
            StopCoroutine(_experimentRoutine);
        }

        if (_activationRoutine != null)
        {
            StopCoroutine(_activationRoutine);
        }

        _experimentRoutine = null;
        _activationRoutine = null;
        IsRunning = false;
    }

    private IEnumerator ExperimentRoutine()
    {
        yield return new WaitForSeconds(_experimentDuration);

        IsRunning = false;

        if (_activationRoutine != null)
        {
            StopCoroutine(_activationRoutine);
            _activationRoutine = null;
        }

        OnExperimentFinished?.Invoke();

        _experimentRoutine = null;
    }

    private IEnumerator ActivationRoutine()
    {
        while (IsRunning)
        {
            OnActivationInterval?.Invoke();

            yield return new WaitForSeconds(_activationInterval);
        }
    }
}
