using Mapbox.Json.Linq;
using Mapbox.Utils;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class MapboxCityService : MonoBehaviour
{
    public string LastCity { get; private set; }

    public event Action<string> OnCityChanged;

    private bool isRunning;

    public void RequestCity(Vector2d coords)
    {
        if (isRunning) return;
        StartCoroutine(ReverseGeocode(coords));
    }

    private IEnumerator ReverseGeocode(Vector2d coords)
    {
        isRunning = true;

        string token = TokenStorage.LoadToken();

        string url =
            $"https://api.mapbox.com/geocoding/v5/mapbox.places/" +
            $"{coords.y},{coords.x}.json?access_token={token}";

        UnityWebRequest req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            string jsonText = req.downloadHandler.text;

            JObject json = JObject.Parse(jsonText);

            string city = "Unknown";

            var features = json["features"];

            if (features != null && features.HasValues)
            {
                city = features[0]["place_name"]?.ToString();
            }

            if (city != LastCity)
            {
                LastCity = city;
                OnCityChanged?.Invoke(city);
            }
        }
        else
        {
            Debug.LogError("[GEOCODER] Failed: " + req.error);
        }

        isRunning = false;
    }
}