using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DroneInput : MonoBehaviour
{
    [Header("Movement")]
    public InputActionReference moveStick;
    public InputActionReference moveUp;
    public InputActionReference moveDown;

    [Header("Rotation")]
    public InputActionReference rotateStick;
    public InputActionReference rotateZRight;
    public InputActionReference rotateZLeft;
}
