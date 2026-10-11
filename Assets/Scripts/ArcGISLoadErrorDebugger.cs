
using System;
using System.Collections;
using UnityEngine;
using Esri.ArcGISMapsSDK.Components;

public class ArcGISLoadErrorDebugger : MonoBehaviour
{
    private IEnumerator Start()
    {
        var component = FindObjectOfType<ArcGISMapComponent>();

        if (component == null)
        {
            Debug.LogError("[ArcGIS Debug] ArcGISMapComponent not found.");
            yield break;
        }

        var map = component.Map;

        if (map == null)
        {
            Debug.LogError("[ArcGIS Debug] ArcGISMap is null.");
            yield break;
        }

        Debug.Log(
            $"[ArcGIS Debug] Basemap={component.Basemap}; " +
            $"BasemapType={component.BasemapType}; " +
            $"AuthenticationType={component.BasemapAuthenticationType}; " +
            $"MapType={component.MapType}");

        string previousStatus = map.LoadStatus.ToString();

        Debug.Log($"[ArcGIS Debug] Initial status: {previousStatus}");
        Debug.Log(
            $"[ArcGIS Debug] Initial error: " +
            $"{map.LoadError?.ToString() ?? "null"}");

        for (int elapsed = 1; elapsed <= 60; elapsed++)
        {
            yield return new WaitForSecondsRealtime(1f);

            string status = map.LoadStatus.ToString();

            if (status != previousStatus)
            {
                Debug.Log(
                    $"[ArcGIS Debug] Status changed: " +
                    $"{previousStatus} -> {status}");

                previousStatus = status;
            }

            if (elapsed % 5 == 0)
            {
                Debug.Log(
                    $"[ArcGIS Debug] {elapsed}s; " +
                    $"status={status}; " +
                    $"error={map.LoadError?.ToString() ?? "null"}");
            }

            if (status == "Loaded")
            {
                Debug.Log("[ArcGIS Debug] Map loaded successfully.");

                if (map.LoadError != null)
                    Debug.LogException(map.LoadError);

                yield break;
            }

            if (status == "FailedToLoad" || status == "Unknown")
            {
                Debug.LogError(
                    $"[ArcGIS Debug] Map loading failed. " +
                    $"Status={status}");

                if (map.LoadError != null)
                {
                    Debug.LogException(map.LoadError);
                }
                else
                {
                    Debug.LogError(
                        "[ArcGIS Debug] LoadError is null " +
                        "even though loading failed.");
                }

                yield break;
            }
        }

        Debug.LogWarning(
            $"[ArcGIS Debug] Timeout after 60 seconds. " +
            $"Final status={map.LoadStatus}; " +
            $"error={map.LoadError?.ToString() ?? "null"}");

        if (map.LoadError != null)
            Debug.LogException(map.LoadError);
    }
}