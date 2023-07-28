using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBG.Runtime.Util
{
    public static class JointDriveExtensions
    {
        public static JointDrive Scale(this JointDrive drive, float scale)
        {
            return new JointDrive
            {
                positionSpring = drive.positionSpring * scale,
                positionDamper = drive.positionDamper * scale,
                maximumForce = drive.maximumForce * scale
            };
        }
    }

    [Serializable]
    public struct JointMotionsConfig
    {
        public ConfigurableJointMotion angularXMotion, angularYMotion, angularZMotion;
        public float angularXLimit, angularYLimit, angularZLimit;

        public void ApplyTo(ref ConfigurableJoint joint)
        {
            joint.angularXMotion = angularXMotion;
            joint.angularYMotion = angularYMotion;
            joint.angularZMotion = angularZMotion;

            var softJointLimit = new SoftJointLimit();

            softJointLimit.limit = angularXLimit / 2;
            joint.highAngularXLimit = softJointLimit;

            softJointLimit.limit = -softJointLimit.limit;
            joint.lowAngularXLimit = softJointLimit;

            softJointLimit.limit = angularYLimit;
            joint.angularYLimit = softJointLimit;

            softJointLimit.limit = angularZLimit;
            joint.angularZLimit = softJointLimit;
        }
    }

    public static class ConfigurableJointExtensions
    {
        public static void SetTargetRotationLocal(this ConfigurableJoint joint, Quaternion targetLocalRotation,
            Quaternion startLocalRotation)
        {
            if (joint.configuredInWorldSpace)
                Debug.LogError(
                    "SetTargetRotationLocal should not be used with joints that are configured in world space. For world space joints, use SetTargetRotation.",
                    joint);
            SetTargetRotationInternal(joint, targetLocalRotation, startLocalRotation, Space.Self);
        }

        public static void SetTargetRotation(this ConfigurableJoint joint, Quaternion targetWorldRotation,
            Quaternion startWorldRotation)
        {
            if (!joint.configuredInWorldSpace)
                Debug.LogError(
                    "SetTargetRotation must be used with joints that are configured in world space. For local space joints, use SetTargetRotationLocal.",
                    joint);
            SetTargetRotationInternal(joint, targetWorldRotation, startWorldRotation, Space.World);
        }

        private static void SetTargetRotationInternal(ConfigurableJoint joint, Quaternion targetRotation,
            Quaternion startRotation, Space space)
        {
            // Calculate the rotation expressed by the joint's axis and secondary axis
            var right = joint.axis;
            var forward = Vector3.Cross(joint.axis, joint.secondaryAxis).normalized;
            var up = Vector3.Cross(forward, right).normalized;
            var worldToJointSpace = Quaternion.LookRotation(forward, up);

            // Transform into world space
            var resultRotation = Quaternion.Inverse(worldToJointSpace);

            // Counter-rotate and apply the new local rotation.
            // Joint space is the inverse of world space, so we need to invert our value
            if (space == Space.World)
                resultRotation *= startRotation * Quaternion.Inverse(targetRotation);
            else
                resultRotation *= Quaternion.Inverse(targetRotation) * startRotation;

            // Transform back into joint space
            resultRotation *= worldToJointSpace;

            // Set target rotation to our newly calculated rotation
            joint.targetRotation = resultRotation;
        }
    }
}