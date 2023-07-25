using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputProcess : MonoBehaviour
{
    public Action<Vector2> onMove;
    public Action<Vector2> onLook;
    public Action<Vector2> onScrollWheel;
    public Action<float> onLeftArm;
    public Action<float> onRightArm;
    public Action<bool> onDebug;

    public void OnMove(InputValue value)
    {
        onMove?.Invoke(value.Get<Vector2>());
    }

    public void OnLook(InputValue value)
    {
        onLook?.Invoke(value.Get<Vector2>());
    }

    public void OnScrollWheel(InputValue value)
    {
        onScrollWheel?.Invoke(value.Get<Vector2>());
    }

    public void OnLeftArm(InputValue value)
    {
        onLeftArm?.Invoke(value.Get<float>());
    }

    public void OnRightArm(InputValue value)
    {
        onRightArm?.Invoke(value.Get<float>());
    }

    public void OnDebug(InputValue value)
    {
        onDebug?.Invoke(value.isPressed);
    }
}