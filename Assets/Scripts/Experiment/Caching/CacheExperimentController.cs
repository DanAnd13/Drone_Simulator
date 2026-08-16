using System.Collections;

using UnityEngine;

public class CacheExperimentController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CacheExperimentMovement _movement;
    [SerializeField] private NetworkScenario _networkScenario;
    [SerializeField] private CacheExperimentTimer _timer;
    [SerializeField] private CacheExperimentLogger _logger;
    [SerializeField] private CacheExperimentResultBuilder _resultBuilder;
    [SerializeField] private float _experimantStartDelay;

    private bool _isRunning;

    private void Start()
    {
        StartCoroutine(StartExperiment());
    }

    private IEnumerator StartExperiment()
    {
        if (_isRunning)
        {
            yield return null;
        }
        yield return new WaitForSeconds(_experimantStartDelay);
        StartCoroutine(ExperimentRoutine());
    }

    private IEnumerator ExperimentRoutine()
    {
        _isRunning = true;

        Debug.Log("[EXPERIMENT] Started");

        _logger.Clear();

        _timer.ResetTimer();
        _timer.StartTimer();

        _networkScenario.StartScenario();

        _movement.StartMovement();

        while (_movement.IsRunning)
        {
            yield return null;
        }

        _timer.StopTimer();

        CacheExperimentResult result = _resultBuilder.Build();

        Debug.Log($"[EXPERIMENT] Finished");
        Debug.Log($"Duration: {result.Duration} ms");
        Debug.Log($"Requests: {result.TotalRequests}");
        Debug.Log($"Success: {result.SuccessfulRequests}");
        Debug.Log($"Failed: {result.FailedRequests}");
        Debug.Log($"Cache: {result.CacheHits}");

        _isRunning = false;
    }
}