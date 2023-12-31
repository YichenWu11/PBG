using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;

namespace PBG.Runtime
{
    // 第三人称角色相机
    public class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private ActiveRagdoll m_ActiveRagdoll;

        public Transform LookAtPoint;
        public GameObject Camera;
        public float LookAtSensitivity = 1;
        public float ScrollWhellSensitivity = 1;
        public float CameraRotationSpeed = 5;

        private Vector2 m_CameraRotation;
        private Vector2 m_InputDelta;
        private Vector3 m_SmoothedLookPoint, m_StartDirection;

        private bool improveSteepInclinations = true;
        private float inclinationAngle = 30, inclinationDistance = 1.5f;

        public float MinDistance = 1f;
        public float MaxDistance = 3f;
        public float MarchingDis = 0.05f;
        private float m_Distance;

        public float MinVerticalAngle = -30;
        public float MaxVerticalAngle = 60;
        public LayerMask DontBlockCamera;

        public UIManager uiManager;

        private void Start()
        {
            Camera.GetComponent<Bloom>().enabled = true;
            Camera.GetComponent<Blur>().enabled = false;

            m_SmoothedLookPoint = LookAtPoint.position;
            m_StartDirection = LookAtPoint.forward;
            m_Distance = MinDistance;
        }

        private void Update()
        {
            if (uiManager.IsPause)
                return;

            UpdateCameraInput();
            UpdateCameraTransform();
            AvoidObstacles();
        }

        private void UpdateCameraInput()
        {
            m_CameraRotation.x = Mathf.Repeat(
                m_CameraRotation.x + m_InputDelta.x * LookAtSensitivity,
                360);
            m_CameraRotation.y = Mathf.Clamp(m_CameraRotation.y + m_InputDelta.y * -1 * LookAtSensitivity,
                MinVerticalAngle, MaxVerticalAngle);
        }

        private void UpdateCameraTransform()
        {
            var movedLookPoint = LookAtPoint.position;
            // if (improveSteepInclinations)
            // {
            //     var anglePercent = (m_CameraRotation.y - MinVerticalAngle) / (MaxVerticalAngle - MinVerticalAngle);
            //     var currentDistance = anglePercent * inclinationDistance - inclinationDistance / 2;
            //     movedLookPoint += Quaternion.Euler(inclinationAngle, 0, 0)
            //                       * Vector3.ProjectOnPlane(Camera.transform.forward, Vector3.up).normalized *
            //                       currentDistance;
            // }

            m_SmoothedLookPoint =
                Vector3.Lerp(m_SmoothedLookPoint, movedLookPoint, CameraRotationSpeed * Time.deltaTime);

            Camera.transform.position = m_SmoothedLookPoint - m_StartDirection * m_Distance;
            Camera.transform.RotateAround(m_SmoothedLookPoint, Vector3.right, m_CameraRotation.y);
            Camera.transform.RotateAround(m_SmoothedLookPoint, Vector3.up, m_CameraRotation.x);
            Camera.transform.LookAt(m_SmoothedLookPoint);
        }

        private void AvoidObstacles()
        {
            var cameraRay = new Ray(LookAtPoint.position, Camera.transform.position - LookAtPoint.position);
            var hit = Physics.Raycast(cameraRay, out var hitInfo,
                Vector3.Distance(Camera.transform.position, LookAtPoint.position), ~DontBlockCamera);

            if (hit)
            {
                Camera.transform.position = hitInfo.point + hitInfo.normal * MarchingDis;
                Camera.transform.LookAt(m_SmoothedLookPoint);
            }
        }

        public void LookProcess(Vector2 value)
        {
            m_InputDelta = value / 10;
        }

        public void ScrollWheelProcess(Vector2 value)
        {
            m_Distance =
                Mathf.Clamp(
                    m_Distance + value.y / 1200 * -ScrollWhellSensitivity,
                    MinDistance, MaxDistance);
        }
    }
}