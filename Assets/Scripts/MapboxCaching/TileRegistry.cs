using Mapbox.Unity.MeshGeneration.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRegistry : MonoBehaviour
{
    public static TileRegistry Instance;

    private HashSet<UnityTile> _tiles = new();

    private void Awake()
    {
        Instance = this;
    }

    public void Register(UnityTile tile)
    {
        if (tile == null) 
        { 
            return; 
        }

        _tiles.Add(tile);
    }

    public IEnumerable<UnityTile> GetAll()
    {
        return _tiles;
    }
}
