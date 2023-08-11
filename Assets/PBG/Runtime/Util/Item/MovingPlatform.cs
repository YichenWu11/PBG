using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBG.Runtime.Util
{
    public class MovingPlatform : MonoBehaviour
    {
        public Vector3 MoveDirection = Vector3.right;
        public float MoveRange = 5f;
        public float MoveSpeed = 1f;

        private Vector3 m_StartPosition;
        private Vector3 m_EndPosition;

        private void Start()
        {
            m_StartPosition = transform.position;
            m_EndPosition = m_StartPosition + MoveDirection.normalized * MoveRange;
        }

        private void Update()
        {
            var pingPongValue = Mathf.PingPong(Time.time * MoveSpeed, 1);
            transform.position = Vector3.Lerp(m_StartPosition, m_EndPosition, pingPongValue);
        }
    }
}