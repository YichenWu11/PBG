using System;
using System.Collections;
using System.Collections.Generic;
using PBG.Runtime.Util;
using UnityEngine;

namespace PBG.Runtime
{
    // All Data For An ActiveRagDoll
    public class ActiveRagdoll : MonoBehaviour
    {
        public InputProcess Input { get; private set; }

        // Animators
        public Animator AnimatedAnimator;
        public Animator PhysicalAnimator;

        // Physical Torso & Animated Torso
        public Transform AnimatedTorso;
        public Rigidbody PhysicalTorso;

        // Joints and Bones
        public Transform[] AnimatedBones;
        public Rigidbody[] PhysicalBones;
        public ConfigurableJoint[] Joints;
        public JointDrive[] JointXAngularDrives;
        public JointDrive[] JointYZAngularDrives;

        private void OnValidate()
        {
            var animators = GetComponentsInChildren<Animator>();
            if (AnimatedAnimator == null)
                AnimatedAnimator = animators[0];
            if (PhysicalAnimator == null)
                PhysicalAnimator = animators[1];

            if (AnimatedTorso == null)
                AnimatedTorso = AnimatedAnimator.GetBoneTransform(HumanBodyBones.Hips);
            if (PhysicalTorso == null)
                PhysicalTorso = PhysicalAnimator.GetBoneTransform(HumanBodyBones.Hips).GetComponent<Rigidbody>();
        }

        private void Awake()
        {
            AnimatedBones = AnimatedTorso.GetComponentsInChildren<Transform>();
            PhysicalBones = PhysicalTorso.GetComponentsInChildren<Rigidbody>();
            Joints = PhysicalTorso.GetComponentsInChildren<ConfigurableJoint>();
            // 缓存 AngularDrives
            JointXAngularDrives = new JointDrive[Joints.Length];
            JointYZAngularDrives = new JointDrive[Joints.Length];
            for (var i = 0; i < Joints.Length; ++i)
            {
                var xDrive = new JointDrive
                {
                    positionSpring = Joints[i].angularXDrive.positionSpring,
                    positionDamper = Joints[i].angularXDrive.positionDamper,
                    maximumForce = Joints[i].angularXDrive.maximumForce
                };
                var yzDrive = new JointDrive
                {
                    positionSpring = Joints[i].angularYZDrive.positionSpring,
                    positionDamper = Joints[i].angularYZDrive.positionDamper,
                    maximumForce = Joints[i].angularYZDrive.maximumForce
                };
                JointXAngularDrives[i] = xDrive;
                JointYZAngularDrives[i] = yzDrive;
            }

            if (TryGetComponent(out InputProcess input))
                Input = input;
#if UNITY_EDITOR
            else
                Debug.Log("Active RagDoll GameObject Missing InputProcess Comp.");
#endif
        }

        public void SetAngularDriveScale(float scale)
        {
            for (var i = 0; i < Joints.Length; ++i)
            {
                Joints[i].angularXDrive = JointXAngularDrives[i].Scale(scale);
                Joints[i].angularYZDrive = JointYZAngularDrives[i].Scale(scale);
            }
        }
    }
}