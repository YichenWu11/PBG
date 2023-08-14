using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBG.Runtime.Util
{
    public class Grabber : MonoBehaviour
    {
        public GrabControl GrabCtrl { get; set; }

        private Rigidbody m_LastCollision;
        private ConfigurableJoint m_Joint;
        private Grabable m_Grabbed;
        public Rigidbody GrabbedRb;

        public bool IsGrabbing => m_Joint != null;

        public void Start()
        {
            enabled = false;
        }

        private void Grab(Rigidbody whatToGrab)
        {
            if (!enabled)
            {
                m_LastCollision = whatToGrab;
                return;
            }

            if (m_Joint != null)
                return;

            if (whatToGrab.transform.IsChildOf(GrabCtrl.ActiveRagdoll.transform))
                return;

            var layerMask0 = LayerMask.NameToLayer("FrozenGrab");
            var layerMask1 = LayerMask.NameToLayer("InnerTrigger");
            if (whatToGrab.gameObject.layer == layerMask0 || whatToGrab.gameObject.layer == layerMask1)
                return;

            // Debug.Log(
            //     $"{whatToGrab.gameObject.layer & layerMask}, {LayerMask.LayerToName(whatToGrab.gameObject.layer)}");

            m_Joint = gameObject.AddComponent<ConfigurableJoint>();
            m_Joint.connectedBody = whatToGrab;
            m_Joint.xMotion = ConfigurableJointMotion.Locked;
            m_Joint.yMotion = ConfigurableJointMotion.Locked;
            m_Joint.zMotion = ConfigurableJointMotion.Locked;

            if (whatToGrab.TryGetComponent(out m_Grabbed))
                m_Grabbed.JointMotionsCfg.ApplyTo(ref m_Joint);
            else
                GrabCtrl.DefaultMotionsCfg.ApplyTo(ref m_Joint);

            GrabbedRb = whatToGrab;
        }

        private void UnGrip()
        {
            if (m_Joint == null)
                return;

            Destroy(m_Joint);
            m_Joint = null;
            m_Grabbed = null;
            GrabbedRb = null;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.rigidbody != null)
                Grab(collision.rigidbody);
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.rigidbody == m_LastCollision)
                m_LastCollision = null;
        }

        private void OnEnable()
        {
            if (m_LastCollision != null)
                Grab(m_LastCollision);
        }

        private void OnDisable()
        {
            UnGrip();
        }
    }
}