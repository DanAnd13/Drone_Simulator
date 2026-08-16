[System.Serializable]
public class CacheExperimentRecord
{
    public long Time;

    public string TileId;

    public bool Success;

    public bool FromCache;

    public int? StatusCode;

    public string Error;
}