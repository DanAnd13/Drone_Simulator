using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SensorInteraction : MonoBehaviour
{
    [SerializeField] private Sensor _sensor;
    [SerializeField] private XRSimpleInteractable _interactable;

    private void OnEnable()
    {
        _interactable.selectEntered.AddListener(OnSelectEntered);
    }

    private void OnDisable()
    {
        _interactable.selectEntered.RemoveListener(OnSelectEntered);
    }

    private void OnMouseDown()
    {
        _sensor.Interact();
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        _sensor.Interact();
    }
}