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

        private void OnValidate()
        {
            m_ActiveRagdoll = GetComponent<ActiveRagdoll>();
        }

        private void Start()
        {
            m_Joints = m_ActiveRagdoll.Joints;
            m_AnimatedBones = m_ActiveRagdoll.AnimatedBones;

            // 缓存关节初始旋转
            m_IniJointsRotations = new Quaternion[m_Joints.Length];
            for (var i = 0; i < m_Joints.Length; i++)
                m_IniJointsRotations[i] = m_Joints[i].transform.localRotation;

            // 冻结躯干骨旋转
            m_ActiveRagdoll.PhysicalTorso.constraints = RigidbodyConstraints.FreezeRotation;
        }

        private void Update()
        {
            SyncWithAnimation();
            KeepBalance();
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
        }
    }
}