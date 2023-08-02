using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBG.Runtime
{
    public class IKControl : MonoBehaviour
    {
        private Animator m_Animator;

        public Vector3 LookPoint { get; set; } // 头部 IK 看向的点

        public float IKWeightTransSpeed = 10f;

        // IK Weight 
        public float LookIKWeight { get; set; } = 1f;
        public float LeftArmIKWeight { get; set; } = 0f;
        public float RightArmIKWeight { get; set; } = 0f;

        private float m_LeftArmIKWeight = 0f;
        private float m_RightArmIKWeight = 0f;

        // IK Targets
        private GameObject m_AllTargets;
        public Transform LeftHandIKTarget { get; set; }
        public Transform RightHandIKTarget { get; set; }

        private void Start()
        {
            m_Animator = GetComponent<Animator>(); // animated animator
            m_AllTargets = new GameObject("AllTargets");
            LeftHandIKTarget = new GameObject("LeftHandIKTarget").transform;
            LeftHandIKTarget.transform.parent = m_AllTargets.transform;
            RightHandIKTarget = new GameObject("RightHandIKTarget").transform;
            RightHandIKTarget.transform.parent = m_AllTargets.transform;
        }

        private void Update()
        {
            m_LeftArmIKWeight = Mathf.Lerp(m_LeftArmIKWeight, LeftArmIKWeight, Time.deltaTime * IKWeightTransSpeed);
            m_RightArmIKWeight = Mathf.Lerp(m_RightArmIKWeight, RightArmIKWeight, Time.deltaTime * IKWeightTransSpeed);
        }

        private void OnAnimatorIK(int layerIndex)
        {
            // Look
            m_Animator.SetLookAtWeight(LookIKWeight, LeftArmIKWeight + RightArmIKWeight / 2 * 0.5f, 1, 1, 0);
            m_Animator.SetLookAtPosition(LookPoint);

            // Left Arm
            m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, m_LeftArmIKWeight);
            m_Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, m_LeftArmIKWeight);
            m_Animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandIKTarget.position);
            m_Animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandIKTarget.rotation);

            // Right Arm
            m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, m_RightArmIKWeight);
            m_Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, m_RightArmIKWeight);
            m_Animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandIKTarget.position);
            m_Animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandIKTarget.rotation);
        }
    }
}