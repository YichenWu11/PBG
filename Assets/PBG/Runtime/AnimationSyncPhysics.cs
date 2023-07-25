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

        private void OnValidate()
        {
            m_ActiveRagdoll = GetComponent<ActiveRagdoll>();
        }

        private void Start()
        {
            m_Animator = m_ActiveRagdoll.AnimatedAnimator;
        }
    }
}