using System.Linq;
using UnityEngine;

public class CacheExperimentResultBuilder : MonoBehaviour
{
    [SerializeField] private CacheExperimentLogger _logger;
    [SerializeField] private CacheExperimentTimer _timer;

    public CacheExperimentResult Build()
    {
        var result = new CacheExperimentResult();

        result.Duration = _timer.ElapsedMilliseconds;

        result.Requests = _logger.Records.ToList();

        result.TotalRequests = result.Requests.Count;

        result.SuccessfulRequests = result.Requests.Count(r => r.Success);

        result.FailedRequests = result.Requests.Count(r => !r.Success);

        result.CacheHits = result.Requests.Count(r => r.FromCache);

        return result;
    }
}