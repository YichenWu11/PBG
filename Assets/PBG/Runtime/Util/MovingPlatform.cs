using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 MoveDirection = Vector3.forward;
    public float MoveDistance = 5f;
    public float MoveSpeed = 1f;

    private Vector3 StartPosition;

    private void Start()
    {
        StartPosition = transform.position;
    }

    private void Update()
    {
        var oscillation = Mathf.Sin(Time.time * MoveSpeed) * MoveDistance * 0.5f;
        var newPosition = StartPosition + MoveDirection * oscillation;
        transform.position = newPosition;
    }
}