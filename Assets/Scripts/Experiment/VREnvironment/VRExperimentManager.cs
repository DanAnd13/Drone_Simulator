using System.Collections;
using UnityEngine;

public class VRExperimentManager : MonoBehaviour
{
    [Header("Sensors")]
    [SerializeField] private Sensor[] _sensors;

    [Header("Experiment")]
    [SerializeField] private VRExperimentTimer _timer;
    [SerializeField] private VRExperimentCalculator _calculator;

    private int _nextSensorIndex;
    private bool _isRunning;

    private void Awake()
    {
        _timer.OnActivationInterval += ActivateNextSensor;
        _timer.OnExperimentFinished += StopExperiment;

        foreach (Sensor sensor in _sensors)
        {
            _calculator.RegisterSensor(sensor);
        }
    }

    private void OnDestroy()
    {
        _timer.OnActivationInterval -= ActivateNextSensor;
        _timer.OnExperimentFinished -= StopExperiment;

        foreach (Sensor sensor in _sensors)
        {
            _calculator.UnregisterSensor(sensor);
        }
    }

    public void StartExperimentTimer()
    {
        StartCoroutine(StartExperiment());
    }

    private IEnumerator StartExperiment()
    {
        if (_isRunning)
        {
           yield return null;
        }

        yield return new WaitForSeconds(4);

        _isRunning = true;
        _nextSensorIndex = 0;

        _calculator.Reset();

        _timer.StartExperimentTimer();

        Debug.Log("[EXPERIMENT] Started");
    }

    private void StopExperiment()
    {
        if (!_isRunning)
        {
            return;
        }

        _timer.Stop();

        FinishExperiment();
    }

    private void ActivateNextSensor()
    {
        if (!_isRunning)
        {
            return;
        }

        if (_sensors.Length == 0)
        {
            return;
        }

        Sensor sensor = _sensors[_nextSensorIndex];

        if (sensor != null)
        {
            sensor.Activate();
        }

        _nextSensorIndex++;

        if (_nextSensorIndex >= _sensors.Length)
        {
            _nextSensorIndex = 0;
        }
    }

    private void FinishExperiment()
    {
        if (!_isRunning)
        {
            return;
        }

        _isRunning = false;

        Debug.Log("[EXPERIMENT] Finished");
        Debug.Log($"[RESULT] Hits: {_calculator.Hits}");
        Debug.Log($"[RESULT] Misses: {_calculator.Misses}");
        Debug.Log($"[RESULT] Average reaction time: {_calculator.AverageReactionTime:F3} s");
    }
}
