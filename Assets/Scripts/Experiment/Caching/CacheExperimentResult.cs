using System.Collections.Generic;

[System.Serializable]
public class CacheExperimentResult
{
    public long Duration;

    public int TotalRequests;

    public int SuccessfulRequests;

    public int FailedRequests;

    public int CacheHits;

    public List<CacheExperimentRecord> Requests = new();
}