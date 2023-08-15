using System;
using System.Collections;
using System.Collections.Generic;
using PBG.Runtime;
using UnityEngine;

public class PickableObject : MonoBehaviour
{
    public PBController Controller;

    public Vector3 MoveDirection = Vector3.up;
    public float MoveRange = 5f;
    public float MoveSpeed = 1f;

    private Vector3 m_StartPosition;
    private Vector3 m_EndPosition;

    public Action<bool> OnPickedTriggered;

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

    private void OnTriggerEnter(Collider other)
    {
        Controller.HasBuff = true;
        OnPickedTriggered?.Invoke(true);
        Destroy(gameObject);
    }
}