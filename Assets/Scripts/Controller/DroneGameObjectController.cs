using UnityEngine;
using UnityEngine.InputSystem;

public class DroneGameObjectController : MonoBehaviour
{
    [SerializeField] private DroneInput _droneInput;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float rotationSpeed = 60f;

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
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        Vector2 moveInput = Vector2.zero;

        if (_droneInput.moveStick != null)
        {
            moveInput = _droneInput.moveStick.action.ReadValue<Vector2>();
        }

        float moveRightValue = -moveInput.x;
        float moveForwardValue = -moveInput.y;

        float moveUpValue = 0f;

        if (_droneInput.moveUp != null && _droneInput.moveUp.action.IsPressed())
        {
            moveUpValue += 1f;
        }

        if (_droneInput.moveDown != null && _droneInput.moveDown.action.IsPressed())
        {
            moveUpValue -= 1f;
        }

        Vector3 movement = transform.forward * moveForwardValue + transform.right * moveRightValue + transform.up * moveUpValue;

        transform.position += movement * moveSpeed * Time.deltaTime;
    }

    private void HandleRotation()
    {
        Vector2 rotateInput = Vector2.zero;

        if (_droneInput.rotateStick != null)
        {
            rotateInput = _droneInput.rotateStick.action.ReadValue<Vector2>();
        }

        float rotateXValue = -rotateInput.y;
        float rotateYValue = rotateInput.x;

        float rotateZValue = 0f;

        if (_droneInput.rotateZRight != null && _droneInput.rotateZRight.action.IsPressed())
        {
            rotateZValue += 1f;
        }

        if (_droneInput.rotateZLeft != null && _droneInput.rotateZLeft.action.IsPressed())
        {
            rotateZValue -= 1f;
        }

        Vector3 rotation = new Vector3(rotateXValue, rotateYValue, rotateZValue);

        transform.Rotate(rotation * rotationSpeed * Time.deltaTime, Space.Self);
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