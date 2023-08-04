using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBG.Runtime.Util
{
    public class MovingPlatform : MonoBehaviour
    {
        public Vector3 MoveDirection = Vector3.forward;
        public float MoveDistance = 5f;
        public float MoveSpeed = 1f;

        private Vector3 m_StartPosition;

        private void Start()
        {
            m_StartPosition = transform.position;
        }

        private void Update()
        {
            var oscillation = Mathf.Sin(Time.time * MoveSpeed) * MoveDistance * 0.5f;
            var newPosition = m_StartPosition + MoveDirection * oscillation;
            transform.position = newPosition;
        }
    }
}