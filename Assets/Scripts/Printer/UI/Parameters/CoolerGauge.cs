using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoolerGauge : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI percentText;

    [SerializeField] private float maxRPM = 5000f;
    [SerializeField] private float animationTime = 0.5f;

    private float currentValue = 0f;

    public float GetMaxRpm()
    {
        return maxRPM;
    }

    public void SetRPM(float rpm)
    {
        float normalized = Mathf.Clamp01(rpm / maxRPM);

        DOTween.To(
            () => currentValue,
            x =>
            {
                currentValue = x;

                fillImage.fillAmount = x;

                float percent = x * 100f;
                percentText.text = $"{percent:0}%";
            },
            normalized,
            animationTime
        );
    }
}
