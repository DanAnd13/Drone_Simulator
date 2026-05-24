using Mapbox.Unity.MeshGeneration.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapboxCacheSimulator : MonoBehaviour
{
    [SerializeField] private Transform _viewer;

    private CachePolicy _policy = new CachePolicy();

    private void Update()
    {
        SimulateCache();
    }

    private void SimulateCache()
    {
        foreach (var tile in TileRegistry.Instance.GetAll())
        {
            if (tile == null)
            {
                continue;
            }

            Vector3 pos = tile.transform.position;

            TileState state = _policy.Evaluate(_viewer.position, pos);

            ApplyState(tile, state);
        }
    }

    private void ApplyState(UnityTile tile, TileState state)
    {
        switch (state)
        {
            case TileState.Active:
                tile.gameObject.SetActive(true);
                break;

            case TileState.Buffer:
                tile.gameObject.SetActive(true);
                break;

            case TileState.Cached:
                tile.gameObject.SetActive(false);
                break;
        }
    }
}
