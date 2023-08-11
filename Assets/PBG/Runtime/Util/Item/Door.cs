using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBG.Runtime.Util
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private ButtonTrigger m_ButtonTrigger;
        [SerializeField] private HingeJoint m_LJoint;
        [SerializeField] private HingeJoint m_RJoint;

        public bool DirectOpen = false; // 如果为 true, 只要关联的按钮被按下一次后就永久打开

        private JointMotor m_LOpen;
        private JointMotor m_ROpen;
        private JointMotor m_LClose;
        private JointMotor m_RClose;

        public float OpenSpeed = 50f;

        public bool OpenOrClose = false; // true for open & false for close

        private float m_LTargetPosX = -0.25f;
        private float m_RTargetPosX = 0.25f;

        [SerializeField] private Rigidbody m_Lrb;
        [SerializeField] private Rigidbody m_Rrb;

        private void OnValidate()
        {
            var joints = GetComponentsInChildren<HingeJoint>();
            m_LJoint = joints[0];
            m_RJoint = joints[1];
            var rbs = GetComponentsInChildren<Rigidbody>();
            m_Lrb = rbs[0];
            m_Rrb = rbs[1];
        }

        private void Start()
        {
            m_LJoint.useMotor = true;
            m_RJoint.useMotor = true;
            m_LOpen = new JointMotor()
            {
                targetVelocity = -OpenSpeed,
                force = 10f
            };
            m_ROpen = new JointMotor()
            {
                targetVelocity = OpenSpeed,
                force = 10f
            };
            m_LClose = new JointMotor()
            {
                targetVelocity = OpenSpeed,
                force = 10f
            };
            m_RClose = new JointMotor()
            {
                targetVelocity = -OpenSpeed,
                force = 10f
            };

            m_Lrb.isKinematic = true;
            m_Rrb.isKinematic = true;

            if (m_ButtonTrigger != null)
            {
                if (DirectOpen)
                    m_ButtonTrigger.OnButtonTriggeredEnterOnly += value => OpenCloseProcess(true);
                else
                    m_ButtonTrigger.OnButtonTriggered += OpenCloseProcess;
            }
        }

        private void Update()
        {
            m_LJoint.motor = OpenOrClose ? m_LOpen : m_LClose;
            m_RJoint.motor = OpenOrClose ? m_ROpen : m_RClose;
            if (Mathf.Approximately(m_Lrb.transform.localPosition.x, m_LTargetPosX))
                m_Lrb.isKinematic = true;
            if (Mathf.Approximately(m_Rrb.transform.localPosition.x, m_RTargetPosX))
                m_Rrb.isKinematic = true;
        }

        private void OpenCloseProcess(bool isPressed)
        {
            OpenOrClose = isPressed;
            m_Lrb.isKinematic = false;
            m_Rrb.isKinematic = false;
            m_LTargetPosX = isPressed ? -0.525f : -0.25f;
            m_RTargetPosX = isPressed ? 0.525f : 0.25f;
        }
    }
}