using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBG.Runtime.Util
{
    public class UpDownPlatform : MonoBehaviour
    {
        [SerializeField] private ButtonTrigger m_ButtonTrigger;

        public Vector3 MoveDirection = Vector3.up;
        public float MoveDistance = 5f;
        public float MoveSpeed = 1f;

        public bool UpOrDown = false; // true for up & false for down

        private Vector3 m_StartPosition;
        private float m_CurDistance = 0f;

        private void Start()
        {
            m_StartPosition = transform.position;
        }

        private void Update()
        {
            m_CurDistance =
                Mathf.Clamp(m_CurDistance + (UpOrDown ? 1 : -1) * Time.deltaTime * MoveSpeed, 0f, MoveDistance);
            var newPosition = m_StartPosition + MoveDirection * m_CurDistance;
            transform.position = newPosition;
        }
    }
}