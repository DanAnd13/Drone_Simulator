using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkWatcher : MonoBehaviour
{
    public Action OnOnline;

    private bool _wasOffline = false;

    private void Update()
    {
        bool online = Application.internetReachability != NetworkReachability.NotReachable;

        if (!online)
        {
            _wasOffline = true;
            return;
        }

        if (online && _wasOffline)
        {
            _wasOffline = false;

            Debug.Log("[NETWORK] Restored");

            OnOnline?.Invoke();
        }
    }
}
