using Mapbox.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppBootstrap : MonoBehaviour
{
    [SerializeField] private MapboxRuntimeController _mapController;
    [SerializeField] private NetworkWatcher _networkWatcher;
    [SerializeField] private RecoveryManager _recoveryManager;

    private IEnumerator Start()
    {
        yield return Initialize();

        _networkWatcher.OnOnline += _recoveryManager.OnNetworkRestored;
    }

    private IEnumerator Initialize()
    {
        string token = TokenStorage.LoadToken();

        if (string.IsNullOrEmpty(token))
        {
            token = MapboxAccess.Instance.Configuration.AccessToken;
        }

        yield return _mapController.RestoreMapbox(token);
    }
}
