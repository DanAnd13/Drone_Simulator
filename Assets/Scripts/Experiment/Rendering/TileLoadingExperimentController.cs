using System.Collections;
using UnityEngine;

public class TileLoadingExperimentController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CacheExperimentMovement _movement;
    [SerializeField] private TileExperimentTracker _tracker;

    [Header("Experiment")]
    [SerializeField] private float _experimentStartDelay = 5f;

    private bool _isRunning;

    public bool IsRunning => _isRunning;

    private void Start()
    {
        StartCoroutine(StartExperiment());
    }

    private IEnumerator StartExperiment()
    {
        if (_isRunning)
        {
            yield break;
        }

        Debug.Log($"[TILE EXPERIMENT] Waiting {_experimentStartDelay} seconds...");
        yield return new WaitForSeconds(_experimentStartDelay);
        yield return StartCoroutine(ExperimentRoutine());
    }

    private IEnumerator ExperimentRoutine()
    {
        _isRunning = true;
        Debug.Log("[TILE EXPERIMENT] Started");
        _tracker.StartTracking();
        _movement.StartMovement();
        while (_movement.IsRunning)
        {
            yield return null;
        }
        while (_tracker.HasActiveTiles)
        {
            yield return null;
        }
        _tracker.StopTracking();

        TileExperimentResult result = _tracker.BuildResult();
        Debug.Log("[TILE EXPERIMENT] Finished");
        Debug.Log($"Loaded tiles: {result.TotalLoadedTiles}");
        Debug.Log($"Average full load time: " + $"{result.AverageFullLoadTime:F2} ms");

        _isRunning = false;
    }
}