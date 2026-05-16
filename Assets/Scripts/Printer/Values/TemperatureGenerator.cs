using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemperatureGenerator : MonoBehaviour
{
    [SerializeField] private TemperatureGauge _thermometer;

    [Header("Temperature settings")]
    [SerializeField] private float _minTemp = 20f;         
    [SerializeField] private float _targetTemp = 220f;     
    [SerializeField] private float _heatingRate = 50f;     

    [Header("Temperarure offset settings")]
    [SerializeField] private float _offsetMin = 12f;        
    [SerializeField] private float _offsetMax = 30f;       
    [SerializeField] private float _changeSpeed = 5f;      
    [SerializeField] private float _updateInterval = 0.1f; 

    private float _currentTemp;
    private float _nextTargetTemp;
    private bool _reachedTarget = false;

    private void Start()
    {
        _currentTemp = _minTemp;
        _nextTargetTemp = _targetTemp;
        StartCoroutine(SimulateNozzle());
    }

    private IEnumerator SimulateNozzle()
    {
        while (true)
        {
            if (!_reachedTarget)
            {
                // нагрівання до робочої температури
                _currentTemp += _heatingRate * _updateInterval;
                if (_currentTemp >= _targetTemp)
                {
                    _currentTemp = _targetTemp;
                    _reachedTarget = true;
                    SetNextTarget();
                }
            }
            else
            {
                // плавне наближення до наступної цілі
                _currentTemp = Mathf.MoveTowards(_currentTemp, _nextTargetTemp, _changeSpeed * _updateInterval);

                // якщо досягли цільового значення, вибираємо нове всередині офсету
                if (Mathf.Approximately(_currentTemp, _nextTargetTemp))
                {
                    SetNextTarget();
                }
            }

            _thermometer.SetTemperature(_currentTemp);
            yield return new WaitForSeconds(_updateInterval);
        }
    }

    private void SetNextTarget()
    {
        // вибираємо випадковий офсет у межах [offsetMin, offsetMax]
        float randomOffset = Random.Range(_offsetMin, _offsetMax);
        _nextTargetTemp = Random.Range(_targetTemp - randomOffset, _targetTemp + randomOffset);
    }

    public float GetCurrentTemperature()
    {
        return _currentTemp;
    }
}
