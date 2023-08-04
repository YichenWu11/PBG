using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBG.Runtime.Util
{
//If we have a stiff rope, such as a metal wire, then we need a simplified solution
//this is also an accurate solution because a metal wire is not swinging as much as a rope made of a lighter material
    public class RopeController : MonoBehaviour
    {
        public Transform ConnectedTo;
        public Transform HangingFrom;

        private LineRenderer m_LineRenderer;

        public List<Vector3> AllRopeSections = new();

        private float m_RopeLength = 1f;
        [SerializeField] private float m_MinRopeLength = 1f;
        [SerializeField] private float m_MaxRopeLength = 20f;

        // Mass of what the rope is carrying
        private float m_LoadMass = 10000f;

        // How fast we can add more/less rope
        private float m_WinchSpeed = 2f;

        //The joint we use to approximate the rope
        private SpringJoint m_SpringJoint;

        private void Start()
        {
            m_SpringJoint = ConnectedTo.GetComponent<SpringJoint>();

            m_LineRenderer = GetComponent<LineRenderer>();

            UpdateSpring();

            HangingFrom.GetComponent<Rigidbody>().mass = m_LoadMass;
        }

        private void Update()
        {
            UpdateWinch();
            DisplayRope();
        }

        private void UpdateSpring()
        {
            var density = 7750f;
            var radius = 0.02f;
            var volume = Mathf.PI * radius * radius * m_RopeLength;
            var ropeMass = volume * density;

            // Add what the rope is carrying
            ropeMass += m_LoadMass;

            // The spring constant (has to recalculate if the rope length is changing)
            //
            // The force from the rope F = rope_mass * g, which is how much the top rope segment will carry
            var ropeForce = ropeMass * 9.81f;

            // Use the spring equation to calculate F = k * x should balance this force, 
            // where x is how much the top rope segment should stretch, such as 0.01m

            // Is about 146000
            var kRope = ropeForce / 0.01f;

            // Add the value to the spring
            // m_SpringJoint.spring = kRope * 1.0f;
            // m_SpringJoint.damper = kRope * 0.8f;

            // Update length of the rope
            m_SpringJoint.maxDistance = m_RopeLength;
        }

        private void DisplayRope()
        {
            // This is not the actual width, but the width use so we can see the rope
            var ropeWidth = 0.2f;

            m_LineRenderer.startWidth = ropeWidth;
            m_LineRenderer.endWidth = ropeWidth;

            // Update the list with rope sections by approximating the rope with a bezier curve
            // A Bezier curve needs 4 control points
            var D = ConnectedTo.position;
            var A = HangingFrom.position;

            // Upper control point
            // To get a little curve at the top than at the bottom
            var B = A + ConnectedTo.up * (-(A - D).magnitude * 0.1f);
            // B = A;

            // Lower control point
            var C = D + HangingFrom.up * ((A - D).magnitude * 0.5f);

            // Get the positions
            BezierCurve.GetBezierCurve(A, B, C, D, AllRopeSections);


            // An array with all rope section positions
            var positions = new Vector3[AllRopeSections.Count];

            for (var i = 0; i < AllRopeSections.Count; i++) positions[i] = AllRopeSections[i];

            // Just add a line between the start and end position for testing purposes
            // Vector3[] positions = new Vector3[2];

            // positions[0] = ConnectedTo.position;
            // positions[1] = HangingFrom.position;


            // Add the positions to the line renderer
            m_LineRenderer.positionCount = positions.Length;

            m_LineRenderer.SetPositions(positions);
        }

        private void UpdateWinch()
        {
            var hasChangedRope = false;

            // More rope
            if (Input.GetKey(KeyCode.O) && m_RopeLength < m_MaxRopeLength)
            {
                m_RopeLength += m_WinchSpeed * Time.deltaTime;

                hasChangedRope = true;
            }
            else if (Input.GetKey(KeyCode.I) && m_RopeLength > m_MinRopeLength)
            {
                m_RopeLength -= m_WinchSpeed * Time.deltaTime;

                hasChangedRope = true;
            }


            if (hasChangedRope)
            {
                m_RopeLength = Mathf.Clamp(m_RopeLength, m_MinRopeLength, m_MaxRopeLength);

                // Need to recalculate the k-value because it depends on the length of the rope
                UpdateSpring();
            }
        }
    }
}