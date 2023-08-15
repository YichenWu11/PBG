using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class UpDownPlatformInner : MonoBehaviour
{
    private Rigidbody m_Rb;

    private void Start()
    {
        m_Rb = GetComponent<Rigidbody>();
    }


    private void OnCollisionStay(Collision other)
    {
        other.rigidbody.velocity = m_Rb.velocity;
    }
}