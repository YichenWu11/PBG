using System;
using System.Collections;
using PBG.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputProcess : MonoBehaviour
{
    [SerializeField] private ActiveRagdoll m_ActiveRagdoll;

    public bool VibrateEnabled = false;

    public Action<Vector2> onMove;
    public Action<Vector2> onLook;
    public Action<Vector2> onScrollWheel;
    public Action<bool> onSprint;
    public Action<float> onLeftArm;
    public Action<float> onRightArm;
    public Action<bool> onJump;
    public Action<bool> onDebug;
    public Action<bool> onBuff;

    public Action<bool> onGroundChanged;

    private Rigidbody m_LeftFoot;
    private Rigidbody m_RightFoot;

    public UIManager uiManager;
    public WorldManager worldManager;

    public bool IsOnGround { get; set; }

    [SerializeField] private float m_OnGroundDetectionDistance = 0.1f;
    [SerializeField] private float m_MaxSlopeAngle = 45f;

    private void OnValidate()
    {
        if (m_ActiveRagdoll == null) m_ActiveRagdoll = GetComponent<ActiveRagdoll>();
    }

    private void Start()
    {
        m_LeftFoot =
            m_ActiveRagdoll.PhysicalAnimator.GetBoneTransform(HumanBodyBones.LeftLowerLeg).GetComponent<Rigidbody>();
        m_RightFoot =
            m_ActiveRagdoll.PhysicalAnimator.GetBoneTransform(HumanBodyBones.RightLowerLeg).GetComponent<Rigidbody>();
    }

    private void Update()
    {
        UpdateOnGround();
    }

    public void OnMove(InputValue value)
    {
        onMove?.Invoke(value.Get<Vector2>());
    }

    public void OnLook(InputValue value)
    {
        if (uiManager.IsPause)
            return;
        onLook?.Invoke(value.Get<Vector2>());
    }

    public void OnScrollWheel(InputValue value)
    {
        if (uiManager.IsPause)
            return;
        onScrollWheel?.Invoke(value.Get<Vector2>());
    }

    public void OnSprint(InputValue value)
    {
        onSprint?.Invoke(value.isPressed);
    }

    public void OnBuff(InputValue value)
    {
        if (uiManager.IsPause)
            return;
        if (value.Get<float>() == 1.0f)
            onBuff?.Invoke(value.isPressed);
    }

    public void OnLeftArm(InputValue value)
    {
        if (uiManager.IsPause)
            return;
        // 手柄震动
        if (VibrateEnabled && value.Get<float>() > 0.5f)
            GamepadVibrate(0f, 8f, 0.3f);
        onLeftArm?.Invoke(value.Get<float>());
    }

    public void OnRightArm(InputValue value)
    {
        if (uiManager.IsPause)
            return;
        // 手柄震动
        if (VibrateEnabled && value.Get<float>() > 0.5f)
            GamepadVibrate(0f, 8f, 0.3f);
        onRightArm?.Invoke(value.Get<float>());
    }

    public void OnJump(InputValue value)
    {
        if (uiManager.IsPause)
            return;
        onJump?.Invoke(value.isPressed);
    }

    public void OnDebug(InputValue value)
    {
        onDebug?.Invoke(value.isPressed);
    }

    public void OnCheat(InputValue value)
    {
        worldManager.Rebirth();
    }

    private void UpdateOnGround()
    {
        var lastIsOnFloor = IsOnGround;

        IsOnGround = CheckRigidbodyOnGround(m_LeftFoot, out var foo)
                     || CheckRigidbodyOnGround(m_RightFoot, out foo);

        if (IsOnGround != lastIsOnFloor)
            onGroundChanged?.Invoke(IsOnGround);
    }

    private bool CheckRigidbodyOnGround(Rigidbody rb, out Vector3 normal)
    {
        // Raycast
        var ray = new Ray(rb.position, Vector3.down);
        var onFloor = Physics.Raycast(ray, out var info, m_OnGroundDetectionDistance, ~(1 << rb.gameObject.layer));

        // Additional checks
        onFloor = onFloor && Vector3.Angle(info.normal, Vector3.up) <= m_MaxSlopeAngle;

        normal = info.normal;
        return onFloor;
    }


    public void GamepadVibrate(float low, float high, float time)
    {
        StartCoroutine(IEGamepadVibrate(low, high, time));
    }

    private IEnumerator IEGamepadVibrate(float low, float high, float time)
    {
        //防止因未连接手柄造成的 DebugError
        if (Gamepad.current == null)
            yield break;

        //设置手柄的 震动速度 以及 恢复震动 , 计时到达之后暂停震动
        Gamepad.current.SetMotorSpeeds(low, high);
        Gamepad.current.ResumeHaptics();
        var endTime = Time.time + time;

        while (Time.time < endTime)
        {
            Gamepad.current.ResumeHaptics();
            yield return null;
        }

        if (Gamepad.current == null)
            yield break;

        Gamepad.current.PauseHaptics();
    }
}