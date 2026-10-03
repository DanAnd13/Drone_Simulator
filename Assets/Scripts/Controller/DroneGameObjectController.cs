using UnityEngine;
using UnityEngine.InputSystem;

public class DroneGameObjectController : MonoBehaviour
{
    [SerializeField] private DroneInput _droneInput;
    [SerializeField] private DroneTestInput _testInput;
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private float _rotationSpeed = 60f;

    private void OnEnable()
    {
        //VR controller
        //EnableAction(_droneInput.moveStick);
        //EnableAction(_droneInput.moveUp);
        //EnableAction(_droneInput.moveDown);

        //EnableAction(_droneInput.rotateStick);
        //EnableAction(_droneInput.rotateZRight);
        //EnableAction(_droneInput.rotateZLeft);

        //Test keyboard controller
        EnableAction(_testInput.moveBackward);
        EnableAction(_testInput.moveForward);
        EnableAction(_testInput.moveRight);
        EnableAction(_testInput.moveLeft);
        EnableAction(_testInput.moveUp);
        EnableAction(_testInput.moveDown);

        EnableAction(_testInput.rotateUpX);
        EnableAction(_testInput.rotateDownX);
        EnableAction(_testInput.rotateRightY);
        EnableAction(_testInput.rotateLeftY);
        EnableAction(_testInput.rotateZRight);
        EnableAction(_testInput.rotateZLeft);
    }

    private void OnDisable()
    {
        //VR controller
        //DisableAction(_droneInput.moveStick);
        //DisableAction(_droneInput.moveUp);
        //DisableAction(_droneInput.moveDown);

        //DisableAction(_droneInput.rotateStick);
        //DisableAction(_droneInput.rotateZRight);
        //DisableAction(_droneInput.rotateZLeft);

        //Test keyboard controller
        DisableAction(_testInput.moveBackward);
        DisableAction(_testInput.moveForward);
        DisableAction(_testInput.moveRight);
        DisableAction(_testInput.moveLeft);
        DisableAction(_testInput.moveUp);
        DisableAction(_testInput.moveDown);

        DisableAction(_testInput.rotateUpX);
        DisableAction(_testInput.rotateDownX);
        DisableAction(_testInput.rotateRightY);
        DisableAction(_testInput.rotateLeftY);
        DisableAction(_testInput.rotateZRight);
        DisableAction(_testInput.rotateZLeft);
    }

    private void Update()
    {
        //Test keyboard controller
        HandleTestMovement();
        HandleTestRotation();

        //VR controller
        //HandleMovement();
        //HandleRotation();
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
        transform.position += movement * _moveSpeed * Time.deltaTime;
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
        transform.Rotate(rotation * _rotationSpeed * Time.deltaTime, Space.Self);
    }

    private void HandleTestMovement()
    {
        float forward = 0f;
        float right = 0f;
        float up = 0f;

        if (IsPressed(_testInput.moveForward))
        {
            forward += 1f;
        }

        if (IsPressed(_testInput.moveBackward))
        {
            forward -= 1f;
        }

        if (IsPressed(_testInput.moveRight))
        {
            right += 1f;
        }

        if (IsPressed(_testInput.moveLeft))
        {
            right -= 1f;
        }

        if (IsPressed(_testInput.moveUp))
        {
            up += 1f;
        }

        if (IsPressed(_testInput.moveDown))
        {
            up -= 1f;
        }
        
        Vector3 movement = transform.forward * forward + transform.right * right + transform.up * up;
        transform.position += movement * _moveSpeed * Time.deltaTime;
    }

    private void HandleTestRotation()
    {
        float rotateX = 0f;
        float rotateY = 0f;
        float rotateZ = 0f;

        if (IsPressed(_testInput.rotateUpX))
        {
            rotateX += 1f;
        }

        if (IsPressed(_testInput.rotateDownX))
        {
            rotateX -= 1f;
        }

        if (IsPressed(_testInput.rotateRightY))
        {
            rotateY += 1f;
        }

        if (IsPressed(_testInput.rotateLeftY))
        {
            rotateY -= 1f;
        }

        if (IsPressed(_testInput.rotateZRight))
        {
            rotateZ += 1f;
        }

        if (IsPressed(_testInput.rotateZLeft))
        {
            rotateZ -= 1f;
        }

        Vector3 rotation = new Vector3(rotateX, rotateY, rotateZ);
        transform.Rotate(rotation * _rotationSpeed * Time.deltaTime, Space.Self);
    }

    private bool IsPressed(InputActionReference action)
    {
        return action != null && action.action.IsPressed();
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