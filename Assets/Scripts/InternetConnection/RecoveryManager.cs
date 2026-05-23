using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;

public class RecoveryManager : MonoBehaviour
{
    [SerializeField] private MapboxRuntimeController _mapController;

    private bool _isRecovering = false;

    public void OnNetworkRestored()
    {
        if (_isRecovering)
            return;

        StartCoroutine(RecoveryFlow());
    }

    private IEnumerator RecoveryFlow()
    {
        _isRecovering = true;

        string token = TokenStorage.LoadToken();

        MapboxAccess.Instance.Configuration.AccessToken = token;

        yield return _mapController.RestoreMapbox(token);

        _isRecovering = false;
    }
}
