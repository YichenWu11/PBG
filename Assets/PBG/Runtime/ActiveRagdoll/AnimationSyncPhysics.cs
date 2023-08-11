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

        private Transform m_AnimatedTorso;
        private Transform m_Chest;

        public float MinTargetDirAngle = -30;
        public float MaxTargetDirAngle = 60;

        public float MinLookAngle = -50;
        public float MaxLookAngle = 60;

        public float MinArmAngle = -70;
        public float MaxArmAngle = 100;

        public float ArmsHorizontalSeparation = 0.75f;

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
            m_AnimatedTorso = m_ActiveRagdoll.AnimatedTorso;
            m_Chest = m_Animator.GetBoneTransform(HumanBodyBones.Spine);
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
                 m_AnimatedTorso.position);
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
            var lookingBackwards = Vector3.Angle(camAimedDir, m_AnimatedTorso.forward) > 90;
            if (lookingBackwards)
                camAimedDir = Vector3.Reflect(camAimedDir, m_AnimatedTorso.forward);
            var camAimedDir2D = Vector3.ProjectOnPlane(camAimedDir, Vector3.up).normalized;

            var directionAngle = Vector3.Angle(camAimedDir, Vector3.up) - 90f;
            m_TargetDirVerticalPercent = 1 - Mathf.Clamp01((directionAngle - MinTargetDirAngle) /
                                                           Mathf.Abs(MaxTargetDirAngle - MinTargetDirAngle));

            var lookVerticalAngle = m_TargetDirVerticalPercent * Mathf.Abs(MaxLookAngle - MinLookAngle) + MinLookAngle;
            var lookDir = Quaternion.AngleAxis(-lookVerticalAngle, m_AnimatedTorso.right) * camAimedDir2D;

            m_IKControl.LookPoint = m_Animator.GetBoneTransform(HumanBodyBones.Head).position + lookDir;

            // Update Arm Targets
            var armsVerticalAngle = m_TargetDirVerticalPercent * Mathf.Abs(MaxArmAngle - MinArmAngle) + MinArmAngle;
            var armsDir = Quaternion.AngleAxis(-armsVerticalAngle, m_AnimatedTorso.right) * camAimedDir2D;
            var currentArmsDistance = m_TargetDirVerticalPercent / 2 + 0.5f;

            var armsMiddleTarget = m_Chest.position + armsDir * currentArmsDistance;
            var upRef = Vector3.Cross(armsDir, m_AnimatedTorso.right).normalized;
            var armsHorizontalVec = Vector3.Cross(armsDir, upRef).normalized;

            m_IKControl.LeftHandIKTarget.position =
                armsMiddleTarget + armsHorizontalVec * ArmsHorizontalSeparation / 2;
            m_IKControl.RightHandIKTarget.position =
                armsMiddleTarget - armsHorizontalVec * ArmsHorizontalSeparation / 2;

            var armsUpVec = Vector3.Cross(armsDir, m_AnimatedTorso.right).normalized;
            m_IKControl.LeftHandHint.position =
                armsMiddleTarget + armsHorizontalVec * ArmsHorizontalSeparation - armsUpVec;
            m_IKControl.RightHandHint.position =
                armsMiddleTarget - armsHorizontalVec * ArmsHorizontalSeparation - armsUpVec;
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