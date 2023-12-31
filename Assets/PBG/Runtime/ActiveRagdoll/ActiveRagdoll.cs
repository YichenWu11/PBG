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
            // AnimatedBones = AnimatedTorso.GetComponentsInChildren<Transform>();
            PhysicalBones = PhysicalTorso.GetComponentsInChildren<Rigidbody>();
            Joints = PhysicalTorso.GetComponentsInChildren<ConfigurableJoint>();

            AnimatedBones = new Transform[Joints.Length + 1];
            AnimatedBones[0] = AnimatedTorso;
            for (var i = 1; i <= Joints.Length; ++i)
                AnimatedBones[i] = FindRecursively(AnimatedTorso, Joints[i - 1].name);

            // for (var i = 0; i < Joints.Length; ++i)
            //     Debug.Log($"{AnimatedBones[i + 1].name}, {Joints[i].name}");

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

            Input = GetComponent<InputProcess>();
        }

        private void Start()
        {
            foreach (var joint in Joints)
            {
                var xDrive = new JointDrive()
                {
                    positionSpring = joint.angularXDrive.positionSpring,
                    positionDamper = joint.angularXDrive.positionDamper * 3f,
                    maximumForce = joint.angularXDrive.maximumForce
                };
                var yzDrive = new JointDrive()
                {
                    positionSpring = joint.angularYZDrive.positionSpring,
                    positionDamper = joint.angularYZDrive.positionDamper * 3f,
                    maximumForce = joint.angularYZDrive.maximumForce
                };
                joint.angularXDrive = xDrive;
                joint.angularYZDrive = yzDrive;
            }
        }

        public void SetAngularDriveScale(float scale)
        {
            for (var i = 0; i < Joints.Length; ++i)
            {
                Joints[i].angularXDrive = JointXAngularDrives[i].Scale(scale);
                Joints[i].angularYZDrive = JointYZAngularDrives[i].Scale(scale);
            }
        }

        public Transform FindRecursively(Transform parent, string fName)
        {
            var result = parent.Find(fName);
            if (result != null) return result;

            for (var i = 0; i < parent.childCount; i++)
            {
                result = FindRecursively(parent.GetChild(i), fName);
                if (result != null) return result;
            }

            return null;
        }
    }
}