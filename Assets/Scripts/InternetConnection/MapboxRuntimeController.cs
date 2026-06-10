using Mapbox.Map;
using Mapbox.Unity;
using Mapbox.Unity.Map;
using Mapbox.Unity.Map.Interfaces;
using Mapbox.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapboxRuntimeController : MonoBehaviour
{
    [SerializeField] private AbstractMap _map;

    private TokenValidator _validator;

    private void Awake()
    {
        _validator = new TokenValidator();
    }

    public IEnumerator RestoreMapbox(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            Debug.LogError("[MAPBOX] Empty token");
            yield break;
        }

        yield return _validator.Validate(token, result =>
        {
            TokenState.SetValid(result);
        });

        if (!TokenState.IsValid)
        {
            Debug.LogError("[MAPBOX] Token invalid");
            yield break;
        }

        TokenStorage.SaveToken(token);

        MapboxAccess.Instance.Configuration.AccessToken = token;

        Debug.Log("[MAPBOX] Restored");
    }

    public IEnumerator SoftRefresh()
    {
        string token = TokenStorage.LoadToken();

        if (string.IsNullOrEmpty(token))
        {
            token = MapboxAccess.Instance.Configuration.AccessToken;
        }

        yield return _validator.Validate(token, result =>
        {
            TokenState.SetValid(result);
        });

        if (!TokenState.IsValid)
        {
            Debug.LogError("[MAPBOX] Token invalid");
            yield break;
        }

        TokenStorage.SaveToken(token);

        Vector2d center = _map.CenterLatitudeLongitude;

        _map.UpdateMap(center);

        yield return null;
    }
}
