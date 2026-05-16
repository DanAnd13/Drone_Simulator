using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ErrorDisplay : MonoBehaviour
{
    [Header("Parent for spawned errors")]
    [SerializeField] private Transform _parent;
    [Header("Prefab for error message")]
    [SerializeField] private GameObject _errorPrefab;

    public void ShowError(string errorText)
    {
        if (_parent == null || _errorPrefab == null)
        {
            Debug.LogWarning("ErrorDisplay: Parent or ErrorPrefab not set.");
            return;
        }

        GameObject errorGO = Instantiate(_errorPrefab, _parent);

        errorGO.transform.localPosition = Vector3.zero;

        TextMeshProUGUI tmp = errorGO.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = errorText;
        }
        else
        {
            Debug.LogWarning("ErrorDisplay: TMP component not found on prefab.");
        }
    }
}
