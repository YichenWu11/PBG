using System;
using System.Collections;
using System.Collections.Generic;
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

            if (TryGetComponent(out InputProcess input))
                Input = input;
#if UNITY_EDITOR
            else
                Debug.Log("Active RagDoll GameObject Missing InputProcess Comp.");
#endif
        }
    }
}