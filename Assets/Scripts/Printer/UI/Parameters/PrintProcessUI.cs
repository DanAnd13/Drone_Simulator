using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PrintProcessUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI layerText;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Settings")]
    [SerializeField] private int totalLayers = 100;
    [SerializeField] private float updateInterval = 0.5f;

    private int currentLayer = 0;
    private PrintState currentState = PrintState.Printing;

    private void Start()
    {
        StartCoroutine(ProcessRoutine());
    }

    private IEnumerator ProcessRoutine()
    {
        while (currentLayer < totalLayers)
        {
            yield return new WaitForSeconds(updateInterval);

            if (currentState != PrintState.Printing)
                continue;

            currentLayer++;

            UpdateUI();

            //optional
            //SimulateRandomEvents();
        }

        currentState = PrintState.Done;
        UpdateUI();
    }

    private void UpdateUI()
    {
        // --- Layers ---
        layerText.text = $"{currentLayer} / {totalLayers}";

        float progress = (float)currentLayer / totalLayers;
        float percent = progress * 100f;

        // --- Status ---
        statusText.text = $"{currentState}   {percent:0}%";

        ApplyColors(progress);
    }

    private void ApplyColors(float progress)
    {
        if (currentLayer >= totalLayers)
            layerText.color = Color.green;
        else
            layerText.color = Color.yellow;

        switch (currentState)
        {
            case PrintState.Printing:
                statusText.color = Color.yellow;
                break;

            case PrintState.Done:
                statusText.color = Color.green;
                break;

            case PrintState.Error:
            case PrintState.Stopped:
                statusText.color = Color.red;
                break;
        }
    }

    private void SimulateRandomEvents()
    {
        float rand = Random.value;

        if (rand < 0.02f)
        {
            currentState = PrintState.Error;
            UpdateUI();
        }
        else if (rand < 0.05f)
        {
            currentState = PrintState.Stopped;
            UpdateUI();

            Invoke(nameof(ResumePrinting), 2f);
        }
    }

    private void ResumePrinting()
    {
        currentState = PrintState.Printing;
        UpdateUI();
    }
}
