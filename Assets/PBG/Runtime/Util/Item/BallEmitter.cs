using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBG.Runtime.Util
{
    public class BallEmitter : MonoBehaviour
    {
        private Transform[] m_Transforms;

        private float m_ElapsedTime = 0f;
        [SerializeField] private float m_EmitInterval = 3f;

        [SerializeField] private float m_Radius = 0.1f;
        [SerializeField] private float m_Velocity = 1000f;
        [SerializeField] private float m_Mass = 0.5f;

        [SerializeField] private Material m_BallMaterial;

        private void Start()
        {
            m_Transforms = GetComponentsInChildren<Transform>();
        }

        private void Update()
        {
            if (m_ElapsedTime < m_EmitInterval)
            {
                m_ElapsedTime += Time.deltaTime;
            }
            else
            {
                var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                ball.transform.position = m_Transforms[3].position;
                ball.transform.localScale = Vector3.one * m_Radius;
                ball.GetComponent<Renderer>().sharedMaterial = m_BallMaterial;

                var rb = ball.AddComponent<Rigidbody>();
                rb.velocity = transform.forward * m_Velocity;
                rb.mass = m_Mass;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

                Destroy(ball, m_EmitInterval * 2f);

                m_ElapsedTime = 0f;
            }
        }
    }
}

public class BallEmitter : MonoBehaviour
{
    private Transform[] m_Transforms;

    private float m_ElapsedTime = 0f;
    [SerializeField] private float m_EmitInterval = 3f;

    [SerializeField] private float m_Radius = 0.1f;
    [SerializeField] private float m_Velocity = 1000f;
    [SerializeField] private float m_Mass = 0.5f;

    [SerializeField] private Material m_BallMaterial;

    private void Start()
    {
        m_Transforms = GetComponentsInChildren<Transform>();
    }

    private void Update()
    {
        if (m_ElapsedTime < m_EmitInterval)
        {
            m_ElapsedTime += Time.deltaTime;
        }
        else
        {
            var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ball.transform.position = m_Transforms[3].position;
            ball.transform.localScale = Vector3.one * m_Radius;
            ball.GetComponent<Renderer>().sharedMaterial = m_BallMaterial;

            var rb = ball.AddComponent<Rigidbody>();
            rb.velocity = transform.forward * m_Velocity;
            rb.mass = m_Mass;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            Destroy(ball, m_EmitInterval * 2f);

            m_ElapsedTime = 0f;
        }
    }
}