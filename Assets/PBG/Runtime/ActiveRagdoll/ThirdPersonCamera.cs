using UnityEngine;
using UnityEngine.InputSystem;

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
        private float m_Distance;

        public float MinVerticalAngle = -30;
        public float MaxVerticalAngle = 60;
        public LayerMask DontBlockCamera;
        public float CameraRepositionOffset = 0.15f;

        [SerializeField] private Material m_BlockCameraMat;
        [SerializeField] private GameObject m_lastBlockObj;
        [SerializeField] private Material m_lastBlockObjMat;

        private void OnValidate()
        {
            if (m_ActiveRagdoll == null)
                m_ActiveRagdoll = GetComponent<ActiveRagdoll>();
            if (LookAtPoint == null)
                LookAtPoint = m_ActiveRagdoll.PhysicalAnimator.GetBoneTransform(HumanBodyBones.Head);
        }

        private void Start()
        {
            Camera = new GameObject("ThirdPersonCamera", typeof(Camera));
            Camera.transform.parent = transform;

            m_SmoothedLookPoint = LookAtPoint.position;
            m_StartDirection = LookAtPoint.forward;
            m_Distance = MinDistance;
        }

        private void Update()
        {
            UpdateCameraInput();
            UpdateCameraTransform();
            // AvoidObstacles();
            MakeObstaclesTransparent();
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
            // Improve steep inclinations
            var movedLookPoint = LookAtPoint.position;
            if (improveSteepInclinations)
            {
                var anglePercent = (m_CameraRotation.y - MinVerticalAngle) / (MaxVerticalAngle - MinVerticalAngle);
                var currentDistance = anglePercent * inclinationDistance - inclinationDistance / 2;
                movedLookPoint += Quaternion.Euler(inclinationAngle, 0, 0)
                                  * Vector3.ProjectOnPlane(Camera.transform.forward, Vector3.up).normalized *
                                  currentDistance;
            }

            // Smooth
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
                Camera.transform.position = hitInfo.point + hitInfo.normal * CameraRepositionOffset;
                Camera.transform.LookAt(m_SmoothedLookPoint);
            }
        }

        private void MakeObstaclesTransparent()
        {
            var cameraRay = new Ray(LookAtPoint.position, Camera.transform.position - LookAtPoint.position);
            var hit = Physics.Raycast(cameraRay, out var hitInfo,
                Vector3.Distance(Camera.transform.position, LookAtPoint.position), ~DontBlockCamera);

            // if (hit)
            // {
            //     if (m_lastBlockObj != null)
            //         if (m_lastBlockObj != hitInfo.transform.gameObject)
            //         {
            //             var lastRenderer = m_lastBlockObj.GetComponent<Renderer>();
            //             lastRenderer.material = m_lastBlockObjMat;
            //         }
            //
            //     var curRenderer = hitInfo.transform.GetComponent<Renderer>();
            //     m_lastBlockObj = hitInfo.transform.gameObject;
            //     m_lastBlockObjMat = curRenderer.material;
            //     curRenderer.material = m_BlockCameraMat;
            // }
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