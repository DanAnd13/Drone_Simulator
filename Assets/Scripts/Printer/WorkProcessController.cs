using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkProcessController : MonoBehaviour
{
    [SerializeField] private ErrorDisplay _errorDisplay;
    [SerializeField] private TemperatureGenerator _temperature;
    [SerializeField] private CoolerRPMGenerator _coolerRpm;
    [SerializeField] private ExtruderSimulator _extruder;
    [SerializeField] private PrintProcessUI _printProcess;

    [SerializeField] private float _threshold = 230f;
    
    private bool _errorTriggered = false;

    private void Start()
    {
        StartCoroutine(CheckTemperature());
    }

    private IEnumerator CheckTemperature()
    {
        while (true)
        {
            float currentTemp = _temperature.GetCurrentTemperature();

            if (!_errorTriggered && currentTemp >= _threshold)
            {
                _errorDisplay.ShowError($"{DateTime.Now:HH:mm:ss} Too high temperature");
                _errorTriggered = true;
            }
            else if (_errorTriggered && currentTemp < _threshold - 5)
            {
                _errorTriggered = false;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}
