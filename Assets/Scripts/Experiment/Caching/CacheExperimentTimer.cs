using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class CacheExperimentTimer : MonoBehaviour
{
    public static CacheExperimentTimer Instance;
    private Stopwatch _stopwatch = new Stopwatch();

    public bool IsRunning => _stopwatch.IsRunning;

    public long ElapsedMilliseconds => _stopwatch.ElapsedMilliseconds;

    private void Awake()
    {
        Instance = this;
    }

    public void StartTimer()
    {
        _stopwatch.Reset();
        _stopwatch.Start();

        Debug.Log("[TIMER] Started");
    }

    public void StopTimer()
    {
        _stopwatch.Stop();

        Debug.Log($"[TIMER] Stopped ({_stopwatch.ElapsedMilliseconds} ms)");
    }

    public void ResetTimer()
    {
        _stopwatch.Reset();
    }
}