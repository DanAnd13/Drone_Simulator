using System.Linq;
using UnityEngine;

public class ExperimentResultBuilder : MonoBehaviour
{
    [SerializeField] private ExperimentLogger _logger;
    [SerializeField] private ExperimentTimer _timer;

    public ExperimentResult Build()
    {
        var result = new ExperimentResult();

        result.Duration = _timer.ElapsedMilliseconds;

        result.Requests = _logger.Records.ToList();

        result.TotalRequests = result.Requests.Count;

        result.SuccessfulRequests = result.Requests.Count(r => r.Success);

        result.FailedRequests = result.Requests.Count(r => !r.Success);

        result.CacheHits = result.Requests.Count(r => r.FromCache);

        return result;
    }
}