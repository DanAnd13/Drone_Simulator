using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileExperimentTracker : MonoBehaviour
{
    public static TileExperimentTracker Instance { get; private set; }

    private readonly Dictionary<string, long> _loadingTiles = new();
    private readonly HashSet<string> _loadedTileIds = new();
    private readonly List<TileExperimentRecord> _records = new();

    private float _experimentStartRealtime;
    private bool _isRunning;

    public IReadOnlyList<TileExperimentRecord> Records => _records;
    public int LoadedTilesCount => _records.Count;
    public bool IsRunning => _isRunning;
    public bool HasActiveTiles => _loadingTiles.Count > 0;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void StartTracking()
    {
        _loadingTiles.Clear();
        _loadedTileIds.Clear();
        _records.Clear();

        _experimentStartRealtime = Time.realtimeSinceStartup;
        _isRunning = true;

        Debug.Log("[TILE EXPERIMENT] Tracking started");
    }

    public void StopTracking()
    {
        _isRunning = false;

        Debug.Log($"[TILE EXPERIMENT] Tracking stopped. " + $"Fully loaded tiles: {LoadedTilesCount}");
    }

    public void RegisterTileLoading(string tileId)
    {
        if (!_isRunning)
        {
            return;
        }

        if (string.IsNullOrEmpty(tileId))
        {
            return;
        }

        if (_loadedTileIds.Contains(tileId))
        {
            return;
        }

        if (_loadingTiles.ContainsKey(tileId))
        {
            return;
        }

        long currentTime = GetElapsedMilliseconds();
        _loadingTiles.Add(tileId, currentTime);
        Debug.Log($"[TILE EXPERIMENT] Tile loading: {tileId} | " + $"time: {currentTime} ms");
    }

    public void RegisterTileFullyLoaded(string tileId)
    {
        if (!_isRunning)
        {
            return;
        }

        if (string.IsNullOrEmpty(tileId))
        {
            return;
        }

        if (!_loadedTileIds.Add(tileId))
        {
            return;
        }
        long currentTime = GetElapsedMilliseconds();
        if (!_loadingTiles.TryGetValue(tileId, out long loadingTime))
        {
            Debug.LogWarning($"[TILE EXPERIMENT] Tile loaded without " + $"registered Loading state: {tileId}");
            return;
        }

        long fullLoadDuration = currentTime - loadingTime;
        TileExperimentRecord record = new TileExperimentRecord
        {
            TileId = tileId,
            LoadingTime = loadingTime,
            FullLoadTime = currentTime,
            FullLoadDuration = fullLoadDuration
        };
        _records.Add(record);
        _loadingTiles.Remove(tileId);

        Debug.Log($"[TILE EXPERIMENT] Tile fully loaded: {tileId} | " + $"loading: {loadingTime} ms | " + $"loaded: {currentTime} ms | " + $"duration: {fullLoadDuration} ms");
    }

    public TileExperimentResult BuildResult()
    {
        TileExperimentResult result = new TileExperimentResult
        {
            Records = _records.ToList(),
            TotalLoadedTiles = _records.Count
        };

        if (_records.Count > 0)
        {
            result.AverageFullLoadTime = _records.Average(record => record.FullLoadDuration);
        }

        return result;
    }

    private long GetElapsedMilliseconds()
    {
        return (long)((Time.realtimeSinceStartup - _experimentStartRealtime) * 1000f);
    }
}