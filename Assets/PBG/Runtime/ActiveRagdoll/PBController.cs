using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace PBG.Runtime
{
    // Physics Based Controller
    public class PBController : MonoBehaviour
    {
        [Header("-- Components --")] [SerializeField]
        private ActiveRagdoll m_ActiveRagdoll;

        [SerializeField] private AnimationSyncPhysics m_AnimSyncPhysics;
        [SerializeField] private PhysicsSyncAnimation m_PhysicsSyncAnim;
        [SerializeField] private ThirdPersonCamera m_ThirdPersonCamera;
        [SerializeField] private GrabControl m_GrabControl;
        [SerializeField] private WorldManager m_WorldMngr;

        [SerializeField] private GameObject MyGlasses;

        [Header("-- Movement --")] public float MaxSpeed = 2.5f;
        public float MinSpeed = 1.25f;
        public float SpeedTransSpeed = 1f;

        [Header("-- Buff --")] public bool HasBuff = false;
        public bool IsBuffEnabled = false;

        [Header("-- State --")] public bool IsEnd = false;

        private Vector2 m_Movement;
        private float m_Speed = 1f;
        private bool m_MoveEnabled = true;
        private bool m_IsSpeedUp = false;

        private float m_FallingTime = 0f;

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
            MyGlasses.SetActive(false);

            // move
            m_ActiveRagdoll.Input.onMove += MovementProcess;
            m_ActiveRagdoll.Input.onSprint += isSpeedUp => m_IsSpeedUp = isSpeedUp && m_Movement != Vector2.zero;
            m_ActiveRagdoll.Input.onSprint += m_PhysicsSyncAnim.SpeedUpProcess;
            m_ActiveRagdoll.Input.onGroundChanged += OnGroundChangedProcess;
            m_ActiveRagdoll.Input.onMove += m_PhysicsSyncAnim.ShakeProcess;
            m_ActiveRagdoll.Input.onJump += m_PhysicsSyncAnim.JumpProcess;

            // TP Camera
            m_ActiveRagdoll.Input.onLook += m_ThirdPersonCamera.LookProcess;
            m_ActiveRagdoll.Input.onScrollWheel += m_ThirdPersonCamera.ScrollWheelProcess;

            m_ActiveRagdoll.Input.onLeftArm += m_AnimSyncPhysics.LeftArmProcess;
            m_ActiveRagdoll.Input.onLeftArm += m_GrabControl.UseLeftGrab;
            m_ActiveRagdoll.Input.onRightArm += m_AnimSyncPhysics.RightArmProcess;
            m_ActiveRagdoll.Input.onRightArm += m_GrabControl.UseRightGrab;

            // Buff
            m_ActiveRagdoll.Input.onBuff += ProcessBuff;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                m_ActiveRagdoll.Input.IsOnGround = true;
                OnGroundChangedProcess(m_ActiveRagdoll.Input.IsOnGround);
            }

            if (!m_PhysicsSyncAnim.IsOnGround && !m_PhysicsSyncAnim.IsGrabbing)
            {
                m_FallingTime += Time.deltaTime;
                if (m_FallingTime > 2f)
                {
                    m_MoveEnabled = false;
                    m_ActiveRagdoll.SetAngularDriveScale(0.1f);
                    m_ActiveRagdoll.PhysicalTorso.constraints = 0;
                    m_ActiveRagdoll.AnimatedAnimator.Play("InTheAir");
                }
            }

            m_MoveEnabled = m_PhysicsSyncAnim.IsOnGround;

            // 由于目前人物只能沿着 Forward 方向前进，为了防止人物抓住 Kinematic 的墙体后向后走导致身体扭曲，加上以下代码
            if (m_GrabControl.IsGrabKinematic)
                m_Movement.y = Mathf.Abs(m_Movement.y);

            m_PhysicsSyncAnim.IsDragSelfUp = !m_PhysicsSyncAnim.IsOnGround && m_PhysicsSyncAnim.IsGrabbing;

            if (m_Movement == Vector2.zero || !m_MoveEnabled)
            {
                m_ActiveRagdoll.AnimatedAnimator.SetBool("moving", false);
                m_Speed = 0f;
                m_PhysicsSyncAnim.SpeedUpRatio = -1f;
                return;
            }

            m_FallingTime = 0f;

            m_Speed =
                Mathf.Lerp(m_Speed, m_IsSpeedUp ? MaxSpeed : MinSpeed, SpeedTransSpeed * Time.deltaTime);

            // 如果开启了 Buff，速度就变慢
            if (HasBuff && IsBuffEnabled)
                m_Speed = 0.5f;

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

        private void MovementProcess(Vector2 value)
        {
            m_Movement = value;
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
        }

        private void ProcessBuff(bool value)
        {
            if (HasBuff)
            {
                MyGlasses.SetActive(!MyGlasses.activeSelf);
                IsBuffEnabled = !IsBuffEnabled;
                m_WorldMngr.ToggleVolumeColorGrading();
                m_WorldMngr.ToggleVolumeDOF();
                m_WorldMngr.ToggleInvisibleObjectsVis();
            }
        }
    }
}