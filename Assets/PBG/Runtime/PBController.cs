using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBG.Runtime
{
    // Physics Based Controller
    public class PBController : MonoBehaviour
    {
        [SerializeField] private ActiveRagdoll m_ActiveRagdoll;
        [SerializeField] private AnimationSyncPhysics m_AnimSyncPhysics;
        [SerializeField] private PhysicsSyncAnimation m_PhysicsSyncAnim;
        [SerializeField] private ThirdPersonCamera m_ThirdPersonCamera;
        [SerializeField] private GrabControl m_GrabControl;

        public float MaxSpeed = 2.5f;
        public float MinSpeed = 1.25f;
        public float SpeedTransSpeed = 1f;

        private Vector2 m_Movement;
        private float m_Speed = 1f;
        private bool m_MoveEnabled = true;
        private bool m_IsSpeedUp = false;

        private void OnValidate()
        {
            if (m_AnimSyncPhysics == null) m_AnimSyncPhysics = GetComponent<AnimationSyncPhysics>();
            if (m_PhysicsSyncAnim == null) m_PhysicsSyncAnim = GetComponent<PhysicsSyncAnimation>();
            if (m_ActiveRagdoll == null) m_ActiveRagdoll = GetComponent<ActiveRagdoll>();
            if (m_ThirdPersonCamera == null) m_ThirdPersonCamera = GetComponent<ThirdPersonCamera>();
            if (m_GrabControl == null) m_GrabControl = GetComponent<GrabControl>();
        }

        private void Start()
        {
            // move
            m_ActiveRagdoll.Input.onMove += movement => m_Movement = movement;
            m_ActiveRagdoll.Input.onSprint += isSpeedUp => m_IsSpeedUp = isSpeedUp && m_Movement != Vector2.zero;
            m_ActiveRagdoll.Input.onGroundChanged += OnGroundChangedProcess;
            m_ActiveRagdoll.Input.onJump += isJumping => m_PhysicsSyncAnim.IsJumping = isJumping;

            // TP Camera
            m_ActiveRagdoll.Input.onLook += m_ThirdPersonCamera.LookProcess;
            m_ActiveRagdoll.Input.onScrollWheel += m_ThirdPersonCamera.ScrollWheelProcess;

            m_ActiveRagdoll.Input.onLeftArm += m_AnimSyncPhysics.LeftArmProcess;
            m_ActiveRagdoll.Input.onLeftArm += m_GrabControl.UseLeftGrab;
            m_ActiveRagdoll.Input.onRightArm += m_AnimSyncPhysics.RightArmProcess;
            m_ActiveRagdoll.Input.onRightArm += m_GrabControl.UseRightGrab;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                m_ActiveRagdoll.Input.IsOnGround = true;
                OnGroundChangedProcess(m_ActiveRagdoll.Input.IsOnGround);
            }

            if (Input.GetKeyDown(KeyCode.LeftAlt))
                Cursor.visible = !Cursor.visible;
            // Quit Game
            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();

            if (m_Movement == Vector2.zero || !m_MoveEnabled)
            {
                m_ActiveRagdoll.AnimatedAnimator.SetBool("moving", false);
                m_Speed = 0f;
                m_PhysicsSyncAnim.SpeedUpRatio = -1f;
                if (!m_PhysicsSyncAnim.IsGrabbing)
                    return;
            }

            m_Speed =
                Mathf.Lerp(m_Speed, m_IsSpeedUp ? MaxSpeed : MinSpeed, SpeedTransSpeed * Time.deltaTime);
            m_PhysicsSyncAnim.SpeedUpRatio = (m_Speed - MinSpeed) / (MaxSpeed - MinSpeed);

            m_ActiveRagdoll.AnimatedAnimator.SetBool("moving", true);
            m_ActiveRagdoll.AnimatedAnimator.SetFloat("speed", m_Movement.magnitude * m_Speed);

            // 前进方向 (相机 forward 在 XZ 平面上的投影)
            var aimedDir =
                Vector3.ProjectOnPlane(m_ThirdPersonCamera.Camera.transform.forward, Vector3.up).normalized;
            var angleOffset = Vector2.SignedAngle(m_Movement, Vector2.up);
            // 根据 movement(输入) 和 aimedDirection(相机朝向) 的目标方向
            var targetDir = Quaternion.AngleAxis(angleOffset, Vector3.up) * aimedDir;
            m_AnimSyncPhysics.AimedDirection = targetDir;
            m_PhysicsSyncAnim.AimedDirection = targetDir;
        }

        private void OnGroundChangedProcess(bool isOnGround)
        {
            m_PhysicsSyncAnim.IsOnGround = isOnGround;
            if (isOnGround)
            {
                m_MoveEnabled = true;
                m_ActiveRagdoll.SetAngularDriveScale(1.0f);
                m_ActiveRagdoll.PhysicalTorso.constraints = RigidbodyConstraints.FreezeRotation;
                m_ActiveRagdoll.AnimatedAnimator.Play("Idle");
            }
            else
            {
                if (!m_PhysicsSyncAnim.IsJumping)
                {
                    m_MoveEnabled = false;
                    m_ActiveRagdoll.SetAngularDriveScale(0.1f);
                    m_ActiveRagdoll.PhysicalTorso.constraints = 0;
                    m_ActiveRagdoll.AnimatedAnimator.Play("InTheAir");
                }
            }
        }
    }
}