using System;
using System.Collections;
using System.Collections.Generic;
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


        [SerializeField] [Tooltip("Rotation Speed by Angle.")]
        private float m_RotationSpeed = 5f;

        [SerializeField] [Range(0, 60)] private float m_MaxSlopeAngle = 45f;

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
        }

        private void Start()
        {
            m_Joints = m_ActiveRagdoll.Joints;
            m_AnimatedBones = m_ActiveRagdoll.AnimatedBones;

            // 缓存关节初始旋转
            m_IniJointsRotations = new Quaternion[m_Joints.Length];
            for (var i = 0; i < m_Joints.Length; i++)
                m_IniJointsRotations[i] = m_Joints[i].transform.localRotation;

            // 冻结躯干旋转
            m_ActiveRagdoll.PhysicalTorso.constraints = RigidbodyConstraints.FreezeRotation;
        }

        private void FixedUpdate()
        {
            KeepBalance();
            SyncWithAnimation();

            var forceDir = CheckSlopeDirection();
            if (forceDir != Vector3.zero && Vector3.Dot(m_TargetDirection, forceDir) > 0f)
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

        private Vector3 CheckSlopeDirection()
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