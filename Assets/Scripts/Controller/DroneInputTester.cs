using UnityEngine;
using UnityEngine.InputSystem;

public class DroneInputTester : MonoBehaviour
{
    [Header("Drone Input Actions")]
    [SerializeField] private InputActionReference moveStick;
    [SerializeField] private InputActionReference moveUp;
    [SerializeField] private InputActionReference moveDown;

    [SerializeField] private InputActionReference rotateStick;
    [SerializeField] private InputActionReference rotateZRight;
    [SerializeField] private InputActionReference rotateZLeft;

    private void OnEnable()
    {
        EnableAction(moveStick);
        EnableAction(moveUp);
        EnableAction(moveDown);

        EnableAction(rotateStick);
        EnableAction(rotateZRight);
        EnableAction(rotateZLeft);
    }

    private void OnDisable()
    {
        DisableAction(moveStick);
        DisableAction(moveUp);
        DisableAction(moveDown);

        DisableAction(rotateStick);
        DisableAction(rotateZRight);
        DisableAction(rotateZLeft);
    }

    private void Update()
    {
        TestStick("Move", moveStick);
        TestButton("MoveUp", moveUp);
        TestButton("MoveDown", moveDown);

        TestStick("Rotate", rotateStick);
        TestButton("RotateZRight", rotateZRight);
        TestButton("RotateZLeft", rotateZLeft);
    }

    private void TestStick(string fieldName, InputActionReference action)
    {
        if (action == null)
            return;

        Vector2 value = action.action.ReadValue<Vector2>();

        if (value.sqrMagnitude > 0.0001f)
        {
            Debug.Log(
                $"{fieldName}: X = {value.x:F2}, Y = {value.y:F2}"
            );
        }
    }

    private void TestButton(string fieldName, InputActionReference action)
    {
        if (action == null)
            return;

        if (action.action.IsPressed())
        {
            Debug.Log($"{fieldName}: 1");
        }
    }

    private void EnableAction(InputActionReference action)
    {
        if (action != null)
            action.action.Enable();
    }

    private void DisableAction(InputActionReference action)
    {
        if (action != null)
            action.action.Disable();
    }
}
