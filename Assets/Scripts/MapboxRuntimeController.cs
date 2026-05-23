using Mapbox.Unity;
using Mapbox.Unity.Map;
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

        bool isValid = false;

        yield return _validator.Validate(token, result =>
        {
            isValid = result;
        });

        if (!isValid)
        {
            Debug.LogError("[MAPBOX] Token invalid");
            yield break;
        }

        TokenStorage.SaveToken(token);

        MapboxAccess.Instance.Configuration.AccessToken = token;

        yield return ResetAndReinitialize();

        Debug.Log("[MAPBOX] Restored");
    }

    private IEnumerator ResetAndReinitialize()
    {
        Debug.Log("[MAPBOX] HARD REFRESH (reinitialize map)");

        yield return null;

        Vector2d center = _map.CenterLatitudeLongitude;
        float zoom = _map.Zoom;

        //map.Initialize(center, (int)zoom);
        //StartCoroutine(ForceMaterialRefresh());
        yield return null;
    }

    private IEnumerator ForceMaterialRefresh()
    {
        yield return null;

        var renderers = FindObjectsOfType<MeshRenderer>();

        foreach (var r in renderers)
        {
            foreach (var mat in r.materials)
            {
                if (mat != null && mat.shader != null)
                {
                    mat.shader = Shader.Find(mat.shader.name);
                }
            }
        }

        yield return null;
    }
}
