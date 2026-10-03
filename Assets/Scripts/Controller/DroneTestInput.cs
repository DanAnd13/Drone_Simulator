using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DroneTestInput : MonoBehaviour
{
    [Header("Movement")]
    public InputActionReference moveForward;
    public InputActionReference moveBackward;
    public InputActionReference moveLeft;
    public InputActionReference moveRight;
    public InputActionReference moveUp;
    public InputActionReference moveDown;

    [Header("Rotation")]
    public InputActionReference rotateUpX;
    public InputActionReference rotateDownX;
    public InputActionReference rotateRightY;
    public InputActionReference rotateLeftY;
    public InputActionReference rotateZRight;
    public InputActionReference rotateZLeft;
}
