using System;
using System.Collections;
using System.Collections.Generic;
using PBG.Runtime;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace PBG.Runtime
{
    public class AnimationSyncPhysics : MonoBehaviour
    {
        private Animator m_Animator;
        [SerializeField] private ActiveRagdoll m_ActiveRagdoll;
        [SerializeField] private bool m_SyncPhysicsBody = true;
        [SerializeField] private bool m_IKEnabled = true;

        private ThirdPersonCamera m_ThirdPersonCamera;
        private IKControl m_IKControl;

        public Vector3 AimedDirection
        {
            set
            {
                m_TargetDirection2D.x = value.x;
                m_TargetDirection2D.y = value.z;
                m_TargetDirection = value;
                m_AimedDirection = Quaternion.LookRotation(value, Vector3.up);
            }
        }

        private Vector2 m_TargetDirection2D = new();
        private Vector3 m_TargetDirection;
        private Quaternion m_AimedDirection;

        private void OnValidate()
        {
            if (m_ActiveRagdoll == null) m_ActiveRagdoll = GetComponent<ActiveRagdoll>();
        }

        private void Awake()
        {
            m_IKControl = m_ActiveRagdoll.AnimatedAnimator.gameObject.AddComponent<IKControl>();
        }

        private void Start()
        {
            m_Animator = m_ActiveRagdoll.AnimatedAnimator;
            m_ThirdPersonCamera = GetComponent<ThirdPersonCamera>();
        }

        private void Update()
        {
            SyncWithPhysics();
        }

        private void FixedUpdate()
        {
            UpdateIK();
        }

        private void SyncWithPhysics()
        {
            if (!m_SyncPhysicsBody) return;
            m_ActiveRagdoll.AnimatedAnimator.transform.position =
                m_ActiveRagdoll.PhysicalTorso.position +
                (m_ActiveRagdoll.AnimatedAnimator.transform.position -
                 m_ActiveRagdoll.AnimatedTorso.position);
            m_ActiveRagdoll.AnimatedAnimator.transform.rotation = m_ActiveRagdoll.PhysicalTorso.rotation;
        }

        private void UpdateIK()
        {
            if (!m_IKEnabled)
            {
                m_IKControl.LookIKWeight = 0;
                m_IKControl.LeftArmIKWeight = 0;
                m_IKControl.RightArmIKWeight = 0;
                return;
            }

            // UpdateIKLookPoint
            var camAimedDir = m_ThirdPersonCamera.Camera.transform.forward;
            var angleOffset = Vector2.SignedAngle(m_TargetDirection2D, Vector2.up);
            // 把 camAimedDir 转到相对于角色前进方向上来
            var rotateQuaternion = Quaternion.AngleAxis(-angleOffset, Vector3.up);
            var finalDir = rotateQuaternion * camAimedDir;

            m_IKControl.LookPoint = m_Animator.GetBoneTransform(HumanBodyBones.Head).position + finalDir;

            var leftArmPos = m_Animator.GetBoneTransform(HumanBodyBones.LeftUpperArm).position;
            var rightArmPos = m_Animator.GetBoneTransform(HumanBodyBones.RightUpperArm).position;
            Debug.Log(m_TargetDirection);
            m_IKControl.LeftArmIKTarget.position = leftArmPos + m_TargetDirection;
            m_IKControl.RightArmIKTarget.position = rightArmPos + m_TargetDirection;
        }

        public void LeftArmProcess(float value)
        {
            m_IKControl.LeftArmIKWeight = value;
        }

        public void RightArmProcess(float value)
        {
            m_IKControl.RightArmIKWeight = value;
        }
    }
}