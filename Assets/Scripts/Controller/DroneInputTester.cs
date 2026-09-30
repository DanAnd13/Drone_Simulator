using UnityEngine;
using UnityEngine.InputSystem;

public class DroneInputTester : MonoBehaviour
{
    [SerializeField] private DroneInput _droneInput;

    private void OnEnable()
    {
        EnableAction(_droneInput.moveStick);
        EnableAction(_droneInput.moveUp);
        EnableAction(_droneInput.moveDown);

        EnableAction(_droneInput.rotateStick);
        EnableAction(_droneInput.rotateZRight);
        EnableAction(_droneInput.rotateZLeft);
    }

    private void OnDisable()
    {
        DisableAction(_droneInput.moveStick);
        DisableAction(_droneInput.moveUp);
        DisableAction(_droneInput.moveDown);

        DisableAction(_droneInput.rotateStick);
        DisableAction(_droneInput.rotateZRight);
        DisableAction(_droneInput.rotateZLeft);
    }

    private void Update()
    {
        TestStick("Move", _droneInput.moveStick);
        TestButton("MoveUp", _droneInput.moveUp);
        TestButton("MoveDown", _droneInput.moveDown);

        TestStick("Rotate", _droneInput.rotateStick);
        TestButton("RotateZRight", _droneInput.rotateZRight);
        TestButton("RotateZLeft", _droneInput.rotateZLeft);
    }

    private void TestStick(string fieldName, InputActionReference action)
    {
        if (action == null)
        {
            return;
        }

        Vector2 value = action.action.ReadValue<Vector2>();

        if (value.sqrMagnitude > 0.0001f)
        {
            Debug.Log($"{fieldName}: X = {value.x:F2}, Y = {value.y:F2}");
        }
    }

    private void TestButton(string fieldName, InputActionReference action)
    {
        if (action == null)
        {
            return;
        }

        if (action.action.IsPressed())
        {
            Debug.Log($"{fieldName}: 1");
        }
    }

    private void EnableAction(InputActionReference action)
    {
        if (action != null)
        {
            action.action.Enable();
        }
    }

    private void DisableAction(InputActionReference action)
    {
        if (action != null)
        {
            action.action.Disable();
        }
    }
}
