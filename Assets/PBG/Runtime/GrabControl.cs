using System;
using System.Collections;
using System.Collections.Generic;
using PBG.Runtime;
using PBG.Runtime.Util;
using UnityEngine;

public class GrabControl : MonoBehaviour
{
    public ActiveRagdoll ActiveRagdoll;

    public JointMotionsConfig DefaultMotionsCfg;

    public float leftArmWeightThreshold = 0.5f, rightArmWeightThreshold = 0.5f;

    private Grabber m_LeftGrab, m_RightGrab;

    private void Awake()
    {
        if (ActiveRagdoll == null) ActiveRagdoll = GetComponent<ActiveRagdoll>();
    }


    private void Start()
    {
        var leftHand = ActiveRagdoll.PhysicalAnimator.GetBoneTransform(HumanBodyBones.LeftHand).gameObject;
        var rightHand = ActiveRagdoll.PhysicalAnimator.GetBoneTransform(HumanBodyBones.RightHand).gameObject;

        (m_LeftGrab = leftHand.AddComponent<Grabber>()).GrabCtrl = this;
        (m_RightGrab = rightHand.AddComponent<Grabber>()).GrabCtrl = this;
    }


    public void UseLeftGrab(float weight)
    {
        m_LeftGrab.enabled = weight > leftArmWeightThreshold;
    }

    public void UseRightGrab(float weight)
    {
        m_RightGrab.enabled = weight > rightArmWeightThreshold;
    }
}