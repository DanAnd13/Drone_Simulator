using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Data;
using UnityEngine;

public class MapboxTileHook : MonoBehaviour
{
    [SerializeField] private AbstractMap _map;
    [SerializeField] private TileRegistry _tileRegistry;

    private void OnEnable()
    {
        _map.OnTileFinished += OnTile;
    }

    private void OnDisable()
    {
        _map.OnTileFinished -= OnTile;
    }

    private void OnTile(UnityTile tile)
    {
        _tileRegistry.Register(tile);
    }
}
