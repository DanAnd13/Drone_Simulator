using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TemperatureGauge : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI temperatureText;

    [SerializeField] private float minTemp = 0;
    [SerializeField] private float maxTemp = 300;

    [SerializeField] private Gradient temperatureGradient;

    [SerializeField] private float animationTime = 0.5f;

    private float currentValue;

    public void SetTemperature(float temperature)
    {
        float normalized = Mathf.InverseLerp(minTemp, maxTemp, temperature);

        DOTween.To(
            () => currentValue,
            x =>
            {
                currentValue = x;

                fillImage.fillAmount = x;

                fillImage.color = temperatureGradient.Evaluate(x);

                float realTemp = Mathf.Lerp(minTemp, maxTemp, x);
                temperatureText.text = $"{realTemp:0} °C";
            },
            normalized,
            animationTime
        );
    }
}
