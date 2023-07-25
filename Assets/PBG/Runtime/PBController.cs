using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBG.Runtime
{
    // Physics Based Controller
    public class PBController : MonoBehaviour
    {
        [SerializeField] private ActiveRagdoll m_ActiveRagdoll;
        [SerializeField] private AnimationSyncPhysics m_AnimSyncPhysics;
        [SerializeField] private PhysicsSyncAnimation m_PhysicsSyncAnim;

        private Vector2 m_Movement;

        private void OnValidate()
        {
            m_AnimSyncPhysics = GetComponent<AnimationSyncPhysics>();
            m_PhysicsSyncAnim = GetComponent<PhysicsSyncAnimation>();
            m_ActiveRagdoll = GetComponent<ActiveRagdoll>();
        }

        private void Start()
        {
            m_ActiveRagdoll.Input.onMove += MovementProcess;
        }

        private void Update()
        {
            if (m_Movement == Vector2.zero)
            {
                m_ActiveRagdoll.AnimatedAnimator.SetBool("moving", false);
            }
            else
            {
                m_ActiveRagdoll.AnimatedAnimator.SetBool("moving", true);
                m_ActiveRagdoll.AnimatedAnimator.SetFloat("speed", m_Movement.magnitude);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();
        }

        private void MovementProcess(Vector2 movement)
        {
            m_Movement = movement;
        }
    }
}