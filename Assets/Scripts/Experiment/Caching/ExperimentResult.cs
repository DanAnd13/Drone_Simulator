using System.Collections.Generic;

[System.Serializable]
public class ExperimentResult
{
    public long Duration;

    public int TotalRequests;

    public int SuccessfulRequests;

    public int FailedRequests;

    public int CacheHits;

    public List<ExperimentRecord> Requests = new();
}