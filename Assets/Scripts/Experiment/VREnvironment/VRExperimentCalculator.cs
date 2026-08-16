using System.Collections.Generic;
using UnityEngine;

public class VRExperimentCalculator : MonoBehaviour
{
    private int _hits;
    private int _misses;

    private readonly List<float> _reactionTimes = new();

    public int Hits => _hits;
    public int Misses => _misses;

    public float AverageReactionTime
    {
        get
        {
            if (_reactionTimes.Count == 0)
            {
                return 0f;
            }

            float sum = 0f;

            foreach (float time in _reactionTimes)
            {
                sum += time;
            }

            return sum / _reactionTimes.Count;
        }
    }

    public void RegisterSensor(Sensor sensor)
    {
        if (sensor == null)
        {
            return;
        }

        sensor.OnHit += RegisterHit;
        sensor.OnMiss += RegisterMiss;
    }

    public void UnregisterSensor(Sensor sensor)
    {
        if (sensor == null)
        {
            return;
        }

        sensor.OnHit -= RegisterHit;
        sensor.OnMiss -= RegisterMiss;
    }

    private void RegisterHit(float activationTime, float interactionTime)
    {
        _hits++;

        float reactionTime = interactionTime - activationTime;

        _reactionTimes.Add(reactionTime);
    }

    private void RegisterMiss()
    {
        _misses++;
    }

    public void Reset()
    {
        _hits = 0;
        _misses = 0;
        _reactionTimes.Clear();
    }
}
