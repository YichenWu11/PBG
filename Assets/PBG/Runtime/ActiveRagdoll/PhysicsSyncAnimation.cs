using System;
using UnityEngine;
using PBG.Runtime.Util;

namespace PBG.Runtime
{
    public class PhysicsSyncAnimation : MonoBehaviour
    {
        [SerializeField] private ActiveRagdoll m_ActiveRagdoll;

        private ConfigurableJoint[] m_Joints;
        private Transform[] m_AnimatedBones;
        private Quaternion[] m_IniJointsRotations;

        [SerializeField] private float m_RotationSpeed = 2f;

        [SerializeField] [Range(0, 60)] private float m_MaxSlopeAngle = 45f;

        [SerializeField] private GrabControl m_GrabControl;

        public bool IsGrabbing => m_GrabControl && m_GrabControl.IsGrabbing;

        public bool WantToJump { get; set; } = false;
        public bool IsJumping { get; set; } = false;
        public bool IsOnGround { get; set; } = true;
        public bool IsDragSelfUp { get; set; } = false;

        public float SpeedUpRatio { get; set; } = 0;

        [SerializeField] private float m_JumpImpluse = 300f;
        [SerializeField] private float m_ShakeImpluse = 30f;
        [SerializeField] private float m_ShakeDir = 1;

        public float MaxSpeedMulti = 2.5f;
        public float MinSpeedMulti = 1.25f;


        public Vector3 AimedDirection
        {
            set
            {
                m_TargetDirection = value;
                m_AimedDirection = Quaternion.LookRotation(value, Vector3.up);
            }
        }

        private Vector3 m_TargetDirection;
        private Quaternion m_AimedDirection;

        private void OnValidate()
        {
            if (m_ActiveRagdoll == null) m_ActiveRagdoll = GetComponent<ActiveRagdoll>();
            if (m_GrabControl == null) m_GrabControl = GetComponent<GrabControl>();
        }

        private void Start()
        {
            m_Joints = m_ActiveRagdoll.Joints;
            m_AnimatedBones = m_ActiveRagdoll.AnimatedBones;

            // 缓存关节初始旋转
            m_IniJointsRotations = new Quaternion[m_Joints.Length];
            for (var i = 0; i < m_Joints.Length; ++i)
                m_IniJointsRotations[i] = m_Joints[i].transform.localRotation;

            // 冻结躯干旋转
            m_ActiveRagdoll.PhysicalTorso.constraints = RigidbodyConstraints.FreezeRotation;
        }

        private void Update()
        {
            // Debug.Log($"{IsGrabbing} {m_GrabControl.IsGrabbing}");
        }

        private void FixedUpdate()
        {
            // SpeedingUp By SpeedUpRatio
            if (IsOnGround)
            {
                var curVel = SpeedUpRatio < -0.5f
                    ? Vector3.zero
                    : (SpeedUpRatio * (MaxSpeedMulti - MinSpeedMulti) + MinSpeedMulti) *
                      m_ActiveRagdoll.PhysicalTorso.transform.forward;
                m_ActiveRagdoll.PhysicalTorso.velocity = curVel;
            }

            // if (IsDragSelfUp)
            // {
            //     var rotationQuaternion = Quaternion.AngleAxis(45, m_ActiveRagdoll.PhysicalTorso.transform.right);
            //     var dragUpDir =
            //         rotationQuaternion * m_ActiveRagdoll.PhysicalTorso.transform.forward;
            //
            //     m_ActiveRagdoll.PhysicalTorso.AddForce(
            //         1600f * m_ShakeDir * dragUpDir,
            //         ForceMode.Force);
            // }

            if (IsOnGround && WantToJump)
            {
                m_ActiveRagdoll.PhysicalTorso.AddForce(Vector3.up * m_JumpImpluse, ForceMode.Impulse);
                WantToJump = false;
            }

            // if (!IsOnGround && IsGrabbing)
            //     m_ActiveRagdoll.PhysicalTorso.AddForce(
            //         m_ShakeImpluse * m_ShakeDir * m_ActiveRagdoll.PhysicalTorso.transform.forward,
            //         ForceMode.Force);

            KeepBalance();
            SyncWithAnimation();

            var forceDir = CalculateSlopeDirection();
            if (forceDir != Vector3.zero && Vector3.Dot(m_TargetDirection, forceDir) > -0.5f)
            {
                // 从 MinAngle 到 MaxAngle 插值
                var forceMagnitude = Mathf.Lerp(4f, 12f, Vector3.Angle(forceDir, Vector3.forward));
                m_ActiveRagdoll.PhysicalTorso.AddForce(forceDir * forceMagnitude, ForceMode.Impulse);
            }
        }

        private void SyncWithAnimation()
        {
            for (var i = 0; i < m_Joints.Length; i++)
                // (i + 1) => skip the first bone (Torso)
                m_Joints[i].SetTargetRotationLocal(
                    m_AnimatedBones[i + 1].localRotation,
                    m_IniJointsRotations[i]);
        }

        private void KeepBalance()
        {
            // 转向到 AimedDirection
            var rotation =
                Quaternion.Lerp(
                    m_ActiveRagdoll.PhysicalTorso.rotation,
                    m_AimedDirection,
                    Time.fixedDeltaTime * m_RotationSpeed);
            m_ActiveRagdoll.PhysicalTorso.MoveRotation(rotation);
        }

        public void JumpProcess(bool value)
        {
            WantToJump = value;
            IsJumping = value;
        }

        public void ShakeProcess(Vector2 value)
        {
            if (IsGrabbing) m_ShakeDir = value.y;
        }

        public void SpeedUpProcess(bool isSpeedUp)
        {
        }

        private Vector3 CalculateSlopeDirection()
        {
            if (Physics.Raycast(
                    m_ActiveRagdoll.PhysicalTorso.position,
                    Vector3.down,
                    out var hitInfo, 0.8f))
            {
                var slopeAngle = Vector3.Angle(hitInfo.normal, Vector3.up);
                if (slopeAngle > 0 && slopeAngle <= m_MaxSlopeAngle)
                {
                    // 计算斜坡的方向
                    var rightDirection = Vector3.Cross(hitInfo.normal, Vector3.up);
                    var slopeDirection = Vector3.Cross(rightDirection, hitInfo.normal);
                    return slopeDirection;
                }
            }

            return Vector3.zero;
        }
    }
}