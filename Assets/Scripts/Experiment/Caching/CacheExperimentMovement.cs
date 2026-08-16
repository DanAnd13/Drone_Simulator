using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CacheExperimentMovement : MonoBehaviour
{
    [Header("Object")]
    [SerializeField] private Transform _target;
    
    [Header("Movement")]
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _duration = 30f;
    
    public bool IsRunning { get; private set; }
    
    private Coroutine _movementRoutine;
    
    public void StartMovement()
    {
        if (IsRunning)
        {
            return;
        }
    
        _movementRoutine = StartCoroutine(MoveRoutine());
    }
    
    public void StopMovement()
    {
        if (_movementRoutine != null)
        {
            StopCoroutine(_movementRoutine);
        }
    
        IsRunning = false;
    }
    
    private IEnumerator MoveRoutine()
    {
        IsRunning = true;
    
        float timer = 0f;
    
        while (timer < _duration)
        {
            timer += Time.deltaTime;
    
            _target.position += _target.forward * _speed * Time.deltaTime;
    
            yield return null;
        }
    
        IsRunning = false;
    }
}
