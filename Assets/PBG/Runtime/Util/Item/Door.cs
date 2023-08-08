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

        private JointLimits m_LOpen;
        private JointLimits m_ROpen;
        private JointLimits m_Close;

        public bool OpenOrClose = false; // true for open & false for close

        private void OnValidate()
        {
            var joints = GetComponentsInChildren<HingeJoint>();
            m_LJoint = joints[0];
            m_RJoint = joints[1];
        }

        private void Start()
        {
            m_LOpen = new JointLimits()
            {
                min = -90f,
                max = 0f
            };
            m_ROpen = new JointLimits()
            {
                min = 0f,
                max = 90f
            };
            m_Close = new JointLimits()
            {
                min = 0f,
                max = 0f
            };
            if (m_ButtonTrigger != null)
                m_ButtonTrigger.OnButtonTriggered += isPressed => OpenOrClose = isPressed;
        }

        private void Update()
        {
            m_LJoint.limits = OpenOrClose ? m_LOpen : m_Close;
            m_RJoint.limits = OpenOrClose ? m_ROpen : m_Close;
        }
    }
}