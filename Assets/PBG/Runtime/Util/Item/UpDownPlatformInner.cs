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

    private void OnCollisionEnter(Collision other)
    {
        var joint = other.gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = m_Rb;
    }


    private void OnCollisionStay(Collision other)
    {
    }

    private void OnCollisionExit(Collision other)
    {
        DestroyImmediate(other.gameObject.GetComponent<FixedJoint>());
    }
}