using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TokenValidator
{
    public IEnumerator Validate(string token, System.Action<bool> result)
    {
        string url = $"https://api.mapbox.com/styles/v1/mapbox/streets-v11?access_token={token}";

        UnityWebRequest req = UnityWebRequest.Get(url);

        yield return req.SendWebRequest();

        bool ok = req.result == UnityWebRequest.Result.Success;

        result?.Invoke(ok);
    }
}
