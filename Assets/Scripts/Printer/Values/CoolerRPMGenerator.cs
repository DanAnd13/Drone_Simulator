using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolerRPMGenerator : MonoBehaviour
{
    [SerializeField] private CoolerGauge _cooler;

    [Header("RPM Settings")]
    [SerializeField] private float _minTargetPercent = 0.65f; 
    [SerializeField] private float _maxTargetPercent = 0.8f;  
    [SerializeField] private float _errorPercent = 0.08f;     
    [SerializeField] private float _updateInterval = 1f;      

    private float _currentRPM = 0f;

    private void Start()
    {
        if (_cooler == null)
        {
            Debug.LogWarning("CoolerRPMGenerator: Cooler не призначено.");
            return;
        }

        StartCoroutine(UpdateRPMRoutine());
    }

    private IEnumerator UpdateRPMRoutine()
    {
        while (true)
        {
            float targetPercent = Random.Range(_minTargetPercent, _maxTargetPercent);

            float targetRPM = _cooler.GetMaxRpm() * targetPercent;

            float error = targetRPM * _errorPercent;
            float finalRPM = Mathf.Clamp(targetRPM + Random.Range(-error, error), 0f, _cooler.GetMaxRpm());

            DOTween.To(() => _currentRPM, x => {
                _currentRPM = x;
                _cooler.SetRPM(_currentRPM);
            }, finalRPM, _updateInterval);

            yield return new WaitForSeconds(_updateInterval);
        }
    }
}
