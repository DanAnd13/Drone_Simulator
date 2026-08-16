using System.Collections.Generic;
using UnityEngine;

public class CacheExperimentLogger : MonoBehaviour
{
    public static CacheExperimentLogger Instance;
    private readonly List<CacheExperimentRecord> _records = new();

    private void Awake()
    {
        Instance = this;
    }

    public void LogTileRequest(long time, string tileId, bool success, bool fromCache, int? statusCode, string error)
    {
        _records.Add(new CacheExperimentRecord
        {
            Time = time,
            TileId = tileId,
            Success = success,
            FromCache = fromCache,
            StatusCode = statusCode,
            Error = error
        });
    }

    public IReadOnlyList<CacheExperimentRecord> Records => _records;

    public void Clear()
    {
        _records.Clear();
    }
}