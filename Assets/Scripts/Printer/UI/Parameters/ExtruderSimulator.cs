using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExtruderSimulator : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI xText;
    [SerializeField] private TextMeshProUGUI yText;
    [SerializeField] private TextMeshProUGUI zText;
    [SerializeField] private TextMeshProUGUI positionText;
    [SerializeField] private TextMeshProUGUI feedrateText;

    [Header("Settings")]
    [SerializeField] private float moveDuration = 1.5f;
    [SerializeField] private float maxFeedrate = 60f;

    private Vector3 currentPosition;
    private float currentFeedrate;

    private void Start()
    {
        SetColors();
        StartSimulation();
    }

    private void SetColors()
    {
        xText.color = Color.red;
        yText.color = Color.green;
        zText.color = Color.blue;
    }

    private void StartSimulation()
    {
        GenerateNextPoint();
    }

    private void GenerateNextPoint()
    {
        Vector3 targetPosition = new Vector3(
            Random.Range(0f, 200f),
            Random.Range(0f, 200f),
            Random.Range(0f, 200f)
        );

        float targetFeedrate = Random.Range(10f, maxFeedrate);

        // рух позиції
        DOTween.To(() => currentPosition, x =>
        {
            currentPosition = x;
            UpdateUI();
        }, targetPosition, moveDuration)
        .SetEase(Ease.Linear)
        .OnComplete(GenerateNextPoint);

        // рух швидкості
        DOTween.To(() => currentFeedrate, x =>
        {
            currentFeedrate = x;
            UpdateUI();
        }, targetFeedrate, moveDuration);
    }

    private void UpdateUI()
    {
        xText.text = $"{currentPosition.x:0.0}";
        yText.text = $"{currentPosition.y:0.0}";
        zText.text = $"{currentPosition.z:0.0}";

        float positionMM = currentPosition.magnitude;

        positionText.text = $"{positionMM:0.0}";
        feedrateText.text = $"{currentFeedrate:0.0}";
    }
}
