using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CachePolicy
{
    public float _activeRadius = 150f;
    public float _bufferRadius = 300f;

    public TileState Evaluate(Vector3 viewerPos, Vector3 tilePos)
    {
        float dist = Vector3.Distance(viewerPos, tilePos);

        if (dist <= _activeRadius)
        {
            return TileState.Active;
        }

        if (dist <= _bufferRadius)
        {
            return TileState.Buffer;
        }

        return TileState.Cached;
    }
}
