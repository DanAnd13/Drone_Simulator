using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenState : MonoBehaviour
{
    public static bool IsValid { get; private set; }

    public static event System.Action<bool> OnChanged;

    public static void SetValid(bool value)
    {
        if (IsValid == value) return;

        IsValid = value;
        OnChanged?.Invoke(value);
    }
}
