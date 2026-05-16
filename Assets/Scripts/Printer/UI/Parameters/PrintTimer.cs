using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PrintTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private float elapsedTime = 0f;
    private PrintState currentState = PrintState.Printing;

    public void SetState(PrintState state)
    {
        currentState = state;
    }

    private void Update()
    {
        if (currentState == PrintState.Printing)
        {
            elapsedTime += Time.deltaTime;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        int totalSeconds = Mathf.FloorToInt(elapsedTime);

        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;

        timerText.text = $"{hours:00}:{minutes:00}:{seconds:00}";
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        UpdateUI();
    }
}
