using Mapbox.Unity.Map;
using Mapbox.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMapDriver : MonoBehaviour
{
    [SerializeField] private AbstractMap _map;

    private Transform _drone;

    void Update()
    {
        Vector3 pos = _drone.position;

        Vector2d latLon = _map.WorldToGeoPosition(pos);

        _map.UpdateMap(latLon);
    }
}
