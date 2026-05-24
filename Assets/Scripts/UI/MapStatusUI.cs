using Mapbox.Unity.Map;
using Mapbox.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapStatusUI : MonoBehaviour
{
    [Header("Map")]
    [SerializeField] private AbstractMap map;
    [SerializeField] private MapboxCityService _cityService;
    [SerializeField] private Transform trackingTarget;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI coordsText;
    [SerializeField] private TextMeshProUGUI cityText;
    [SerializeField] private TextMeshProUGUI networkText;
    [SerializeField] private TextMeshProUGUI tokenText;

    private float timer;

    private void OnEnable()
    {
        TokenState.OnChanged += OnTokenChanged;
        _cityService.OnCityChanged += HandleCity;
    }

    private void OnDisable()
    {
        TokenState.OnChanged -= OnTokenChanged;
        _cityService.OnCityChanged -= HandleCity;
    }

    private void Update()
    {
        UpdateCoordinates();
        UpdateCity();
        UpdateNetwork();
        UpdateColors();
    }

    private void UpdateCoordinates()
    {
        if (map == null || trackingTarget == null) return;

        Vector2d geo = map.WorldToGeoPosition(trackingTarget.position);

        coordsText.text = $"Lat: {geo.x:F6} \n Lon: {geo.y:F6}";
    }

    private void HandleCity(string city)
    {
        cityText.text = city;
    }

    private void UpdateCity()
    {
        timer += Time.deltaTime;

        if (timer < 2f) return; // не спамимо API
        timer = 0f;

        if (map == null) return;

        Vector2d coords = map.CenterLatitudeLongitude;

        _cityService.RequestCity(coords);
    }

    private void UpdateNetwork()
    {
        bool online =
            Application.internetReachability != NetworkReachability.NotReachable;

        networkText.text = online ? "ONLINE" : "OFFLINE";
    }

    private void OnTokenChanged(bool valid)
    {
        tokenText.text = valid ? "TOKEN VALID" : "TOKEN INVALID";
        tokenText.color = valid ? Color.green : Color.red;
    }

    private void UpdateColors()
    {
        bool online = Application.internetReachability != NetworkReachability.NotReachable;

        networkText.color = online ? Color.green : Color.red;
    }
}
