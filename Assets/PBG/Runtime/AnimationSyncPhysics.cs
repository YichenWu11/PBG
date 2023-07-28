using System;
using System.Collections;
using System.Collections.Generic;
using PBG.Runtime;
using UnityEngine;

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

        public float MinTargetDirAngle = -30;
        public float MaxTargetDirAngle = 60;

        public float MinLookAngle = -50;
        public float MaxLookAngle = 60;

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
        private float m_TargetDirVerticalPercent;

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
            var lookingBackwards = Vector3.Angle(camAimedDir, m_ActiveRagdoll.AnimatedTorso.forward) > 90;
            if (lookingBackwards)
                camAimedDir = Vector3.Reflect(camAimedDir, m_ActiveRagdoll.AnimatedTorso.forward);
            var camAimedDir2D = Vector3.ProjectOnPlane(camAimedDir, Vector3.up).normalized;

            var directionAngle = Vector3.Angle(camAimedDir, Vector3.up);
            directionAngle -= 90f;
            m_TargetDirVerticalPercent = 1 - Mathf.Clamp01((directionAngle - MinTargetDirAngle) /
                                                           Mathf.Abs(MaxTargetDirAngle - MinTargetDirAngle));

            var lookVerticalAngle = m_TargetDirVerticalPercent * Mathf.Abs(MaxLookAngle - MinLookAngle) + MinLookAngle;
            var lookDir = Quaternion.AngleAxis(-lookVerticalAngle, m_ActiveRagdoll.AnimatedTorso.right) * camAimedDir2D;

            m_IKControl.LookPoint = m_Animator.GetBoneTransform(HumanBodyBones.Head).position + lookDir;

            // Update Arm Targets
            var leftArmPos = m_ActiveRagdoll.PhysicalAnimator.GetBoneTransform(HumanBodyBones.LeftUpperArm).position;
            var rightArmPos = m_ActiveRagdoll.PhysicalAnimator.GetBoneTransform(HumanBodyBones.RightUpperArm).position;
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