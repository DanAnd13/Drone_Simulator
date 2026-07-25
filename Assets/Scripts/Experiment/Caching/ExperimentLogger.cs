using System.Collections.Generic;
using UnityEngine;

public class ExperimentLogger : MonoBehaviour
{
    public static ExperimentLogger Instance;
    private readonly List<ExperimentRecord> _records = new();

    private void Awake()
    {
        Instance = this;
    }

    public void LogTileRequest(long time, string tileId, bool success, bool fromCache, int? statusCode, string error)
    {
        _records.Add(new ExperimentRecord
        {
            Time = time,
            TileId = tileId,
            Success = success,
            FromCache = fromCache,
            StatusCode = statusCode,
            Error = error
        });
    }

    public IReadOnlyList<ExperimentRecord> Records => _records;

    public void Clear()
    {
        _records.Clear();
    }
}