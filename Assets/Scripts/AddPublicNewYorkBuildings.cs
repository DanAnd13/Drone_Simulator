
using System.Collections;
using UnityEngine;
using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Layers;
using Esri.Unity;

public class AddGlobalOpen3DBuildings : MonoBehaviour
{
    [SerializeField] private ArcGISMapComponent _mapComponent;

    [SerializeField] private float _opacity = 1.0f;

    private IEnumerator Start()
    {
        if (_mapComponent == null)
        {
            _mapComponent = FindObjectOfType<ArcGISMapComponent>();
        }

        if (_mapComponent == null)
        {
            Debug.LogError("[Global Buildings] ArcGISMapComponent not found.");
            yield break;
        }

        float elapsed = 0f;
        while ((_mapComponent.Map == null || _mapComponent.Map.LoadStatus.ToString() != "Loaded") && elapsed < 60f)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        var map = _mapComponent.Map;
        if (map == null || map.LoadStatus.ToString() != "Loaded")
        {
            Debug.LogError("[Global Buildings] Map did not load.");
            yield break;
        }

        const string source = "https://www.arcgis.com/home/item.html" + "?id=c444b24b184c4523a5dc96248bfea4e1";
        var buildingsLayer = new ArcGIS3DObjectSceneLayer(source, "Open 3D Buildings - Global", _opacity, true, "");
        map.Layers.Add(buildingsLayer);
        Debug.Log("[Global Buildings] Layer added. " + "Waiting for the layer view state.");
    }
}
