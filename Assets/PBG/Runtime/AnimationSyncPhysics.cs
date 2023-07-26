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

        public Vector3 AimedDirection
        {
            set => m_AimedDirection = Quaternion.LookRotation(value, Vector3.up);
        }

        private Quaternion m_AimedDirection;

        private void OnValidate()
        {
            if (m_ActiveRagdoll == null) m_ActiveRagdoll = GetComponent<ActiveRagdoll>();
        }

        private void Start()
        {
            m_Animator = m_ActiveRagdoll.AnimatedAnimator;
        }
    }
}