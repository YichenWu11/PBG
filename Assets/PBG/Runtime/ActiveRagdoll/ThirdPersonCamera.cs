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
            var bloom = Camera.AddComponent<Bloom>();
            bloom.useKarisAverage = true;
            bloom.luminanceThreshole = 2.0f;
            bloom.bloomIntensity = 0.05f;

            m_SmoothedLookPoint = LookAtPoint.position;
            m_StartDirection = LookAtPoint.forward;
            m_Distance = MinDistance;
        }

        private void Update()
        {
            UpdateCameraInput();
            UpdateCameraTransform();
            AvoidObstacles();
            // MakeObstaclesTransparent();
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

// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEditor;
//
// namespace PBG.Runtime
// {
//     public enum OrbitPosition
//     {
//         Top,
//         Middle,
//         Bottom
//     }
//
//     #region Helper Classes
//
//     [Serializable]
//     public class OrbitRing
//     {
//         public float radius = 3f;
//         public float height = 2.5f;
//         public Color color = Color.red;
//
//         public OrbitRing(float radius, float height, Color color)
//         {
//             this.radius = radius;
//             this.height = height;
//             this.color = color;
//         }
//
//         public OrbitRing(float radius, float height)
//         {
//             this.radius = radius;
//             this.height = height;
//             color = Color.green;
//         }
//
//         public float GetBorderDistanceToReference()
//         {
//             return Mathf.Sqrt(radius * radius + height * height);
//         }
//     }
//
//     [Serializable]
//     public class ZoomOutOnMotionEffect
//     {
//         public bool enabled = true;
//         public float startSpeed = 10f;
//         public float capSpeed = 15f;
//         public float startDistanceRatio = 0f;
//         public float capDistanceRatio = 0.3f;
//
//         public float GetDistanceIncreaseForSpeed(float speed)
//         {
//             if (enabled && speed > startSpeed)
//             {
//                 var speedRatio = Mathf.InverseLerp(startSpeed, capSpeed, speed);
//                 var distanceIncrease = 1 + Mathf.Lerp(startDistanceRatio, capDistanceRatio, speedRatio);
//                 return distanceIncrease;
//             }
//             else
//             {
//                 return 1f;
//             }
//         }
//     }
//
//     [Serializable]
//     public class MotionShakeEffect
//     {
//         public bool enabled = true;
//         public float startSpeed = 10f;
//         public float capSpeed = 15f;
//
//         [Header("Vertical")] public float verticalStartIntensity = 0.02f;
//         public float verticalCapIntensity = 0.05f;
//         public float verticalSpeed = 15f;
//         [Range(0, 2)] public float verticalPhase = 0.5f;
//
//         [Header("Horizontal")] public float horizontalStartIntensity = 0.03f;
//         public float horizontalCapIntensity = 0.07f;
//         public float horizontalSpeed = 7.5f;
//         [Range(0, 2)] public float horizontalPhase = 0f;
//
//         private float phase = 0f;
//         private bool running = false;
//
//         public Vector3 Update(float speed, float deltaTime)
//         {
//             if (speed >= startSpeed)
//             {
//                 if (!running) Start();
//                 phase += deltaTime;
//
//                 var speedRatio = Mathf.InverseLerp(startSpeed, capSpeed, speed);
//                 var verticalIntensity = Mathf.Lerp(verticalStartIntensity, verticalCapIntensity, speedRatio);
//                 var horizontalIntensity = Mathf.Lerp(horizontalStartIntensity, horizontalCapIntensity, speedRatio);
//
//                 var horizontal = Mathf.Sin(Time.time * horizontalSpeed + horizontalPhase * Mathf.PI) *
//                                  horizontalIntensity;
//                 var vertical = Mathf.Sin(Time.time * verticalSpeed + verticalPhase * Mathf.PI) * verticalIntensity;
//                 return new Vector3(horizontal, vertical, 0f);
//             }
//             else
//             {
//                 Stop();
//                 return Vector3.zero;
//             }
//         }
//
//         private void Start()
//         {
//             phase = 0f;
//             running = true;
//         }
//
//         private void Stop()
//         {
//             running = false;
//         }
//     }
//
//     #endregion
//
//     [ExecuteInEditMode]
//     public class ThirdPersonCamera : MonoBehaviour
//     {
//         #region Inspector Settings
//
//         [Header("Editor Settings")] [SerializeField]
//         private bool showGizmos = true;
//
//         [SerializeField] private bool editorPreview = true;
//
//         [Header("Targets")] [SerializeField] private GameObject follow = null;
//         [SerializeField] private GameObject lookAt = null;
//
//         [Header("Orbits")] [SerializeField] private OrbitRing topRing = new(2f, 1.4f, Color.red);
//         [SerializeField] private OrbitRing middleRing = new(5f, 3f, Color.red);
//         [SerializeField] private OrbitRing bottomRing = new(1f, -1f, Color.red);
//
//         [Header("Positioning")] [SerializeField]
//         private bool lockHeight = false;
//
//         [SerializeField] private float fixedHeight = .5f;
//
//         [SerializeField] private bool lockTranslation = false;
//
//         [SerializeField] [Range(0f, 360f)] private float fixedTranslation = 0f;
//
//         [SerializeField] private bool avoidClipping = true;
//
//         [SerializeField] private LayerMask avoidClippingLayerMask = 0;
//
//         [SerializeField] private float clipDistance = 5f;
//
//         [SerializeField] private float clippingOffset = 0f;
//
//         [SerializeField] [Range(-180, 180)] private float horizontalTilt = 0f;
//         [SerializeField] private float horizontalOffset = 0f;
//         [SerializeField] [Range(-180, 180)] private float verticalTilt = 0f;
//         [SerializeField] private float verticalOffset = 0f;
//         [SerializeField] private bool useTargetNormal = true;
//
//         [Header("Controls")] [SerializeField] private bool captureCursor = false;
//
//         [Header("X axis")] [SerializeField] private string horizontalAxis = "Mouse X";
//         [SerializeField] private float horizontalSensitivity = 1f;
//         [SerializeField] private bool invertX = false;
//         [Header("Y axis")] [SerializeField] private string verticalAxis = "Mouse Y";
//         [SerializeField] private float verticalSensitivity = 0.8f;
//         [SerializeField] private bool invertY = true;
//
//         [Header("Effects")] [SerializeField] private ZoomOutOnMotionEffect zoomOutOnMotion = new();
//         [SerializeField] private MotionShakeEffect motionShake = new();
//
//         #endregion
//
//         #region Private Variables
//
//         private float cameraTranslation = 0f;
//         private float verticalMultiplier = 10f;
//         private float referenceHeight = 0f;
//         private float referenceDistance;
//         private float noClippingHeight;
//         private float noClippingDistance;
//         private OrbitRing cameraRing = null;
//         private Vector3 up;
//         private Vector3 right;
//         private Vector3 forward;
//
//         #endregion
//
//         // ===================== Lifecycle ===================== //
//
//         #region Lifecycle Methods
//
//         private void Start()
//         {
//             referenceHeight = middleRing.height;
//         }
//
//         private void Update()
//         {
//             if ((Application.isPlaying || editorPreview) && Time.timeScale > 0)
//             {
//                 if (captureCursor && Application.isPlaying) Cursor.lockState = CursorLockMode.Locked;
//                 SetNormalVectors();
//                 SetPosition();
//                 SetRotation();
//                 ApplyEffects();
//             }
//         }
//
//         private void OnDrawGizmos()
//         {
//             if (follow != null && showGizmos)
//             {
//                 DrawRing(topRing);
//                 DrawRing(middleRing);
//                 DrawRing(bottomRing);
//             }
//         }
//
//         #endregion
//
//         // ===================== Update steps ===================== //
//
//         #region Update Steps
//
//         private void SetNormalVectors()
//         {
//             up = useTargetNormal ? follow.transform.up : Vector3.up;
//             right = Vector3.Cross(up, Vector3.right);
//             forward = Vector3.Cross(up, right);
//         }
//
//         private void SetPosition()
//         {
//             ReadInputs();
//             referenceDistance = 0f;
//
//             cameraRing = GetCameraRing();
//
//             referenceHeight = cameraRing.height;
//             var distance = cameraRing.GetBorderDistanceToReference();
//             referenceDistance = Mathf.Sqrt(distance * distance - referenceHeight * referenceHeight);
//             referenceDistance = ApplyZoomOutOnMotion(referenceDistance);
//             if (avoidClipping) CorrectClipping(Mathf.Min(distance, clipDistance));
//
//             var heightVector = up * (avoidClipping ? noClippingHeight : referenceHeight);
//             var distanceVector = -forward * (avoidClipping ? noClippingDistance : referenceDistance);
//
//             transform.position = follow.transform.position + heightVector + distanceVector;
//             transform.RotateAround(follow.transform.position, up, cameraTranslation);
//         }
//
//         private void SetRotation()
//         {
//             LookAt(up, lookAt.transform);
//
//             var verticalAngles = forward * verticalTilt;
//             var horizontalAngles = up * horizontalTilt;
//
//             var eulerRotation = verticalAngles + horizontalAngles;
//             transform.Rotate(eulerRotation.x, eulerRotation.y, eulerRotation.z);
//             ApplyPositionOffset();
//         }
//
//         private void ApplyEffects()
//         {
//             if (motionShake.enabled) ApplyMotionShake();
//         }
//
//         #endregion
//
//         // ===================== Input ===================== //
//
//         #region Input Methods
//
//         private void ReadInputs()
//         {
//             if (lockHeight)
//                 referenceHeight = fixedHeight;
//             else if (Application.isPlaying)
//                 referenceHeight += Input.GetAxis(verticalAxis) * verticalSensitivity * (invertY ? -1 : 1);
//
//             if (lockTranslation)
//             {
//                 cameraTranslation = fixedTranslation;
//             }
//             else if (Application.isPlaying)
//             {
//                 cameraTranslation += Input.GetAxis(horizontalAxis) * verticalMultiplier * horizontalSensitivity *
//                                      (invertX ? -1 : 1);
//                 if (cameraTranslation > 360f)
//                     cameraTranslation -= 360f;
//                 else if (cameraTranslation < 0f) cameraTranslation += 360f;
//             }
//         }
//
//         #endregion
//
//         // ===================== Positioning ===================== //
//
//         #region Positioning Methods
//
//         private OrbitRing GetCameraRing()
//         {
//             if (referenceHeight >= topRing.height)
//             {
//                 return new OrbitRing(topRing.radius, topRing.height);
//             }
//             else if (referenceHeight >= middleRing.height)
//             {
//                 var radius = EaseLerpRingRadius(middleRing, topRing);
//                 return new OrbitRing(radius, referenceHeight);
//             }
//             else if (referenceHeight >= bottomRing.height)
//             {
//                 var radius = EaseLerpRingRadius(bottomRing, middleRing);
//                 return new OrbitRing(radius, referenceHeight);
//             }
//             else
//             {
//                 return new OrbitRing(bottomRing.radius, bottomRing.height);
//             }
//         }
//
//         private void CorrectClipping(float raycastDistance)
//         {
//             var ray = new Ray(follow.transform.position, (transform.position - follow.transform.position).normalized);
//             RaycastHit hit;
//
//             if (Physics.Raycast(ray, out hit, raycastDistance, avoidClippingLayerMask, QueryTriggerInteraction.Ignore))
//             {
//                 var safeDistance = hit.distance - clippingOffset;
//                 var sinAngl = referenceHeight / raycastDistance;
//                 var cosAngl = referenceDistance / raycastDistance;
//
//                 noClippingHeight = safeDistance * sinAngl;
//                 noClippingDistance = safeDistance * cosAngl;
//             }
//             else
//             {
//                 noClippingHeight = referenceHeight;
//                 noClippingDistance = referenceDistance;
//             }
//         }
//
//         private void ApplyPositionOffset()
//         {
//             transform.position =
//                 transform.position + transform.right * horizontalOffset + transform.up * verticalOffset;
//         }
//
//         #endregion
//
//         // ===================== Rotation ===================== //
//
//         #region Rotation Methods
//
//         private void LookAt(Vector3 normal, Transform lookAt)
//         {
//             var targetDirection = (lookAt.position - transform.position).normalized;
//             transform.localRotation = Quaternion.LookRotation(targetDirection, normal);
//         }
//
//         #endregion
//
//         // ===================== Effects ===================== //
//
//         #region Effects Methods
//
//         private float ApplyZoomOutOnMotion(float distance)
//         {
//             var rb = follow.GetComponent<Rigidbody>();
//             if (rb != null)
//             {
//                 var speed = follow.GetComponent<Rigidbody>().velocity.magnitude;
//                 var distanceIncrease = zoomOutOnMotion.GetDistanceIncreaseForSpeed(speed);
//                 return distanceIncrease * distance;
//             }
//             else
//             {
//                 return distance;
//             }
//         }
//
//         private void ApplyMotionShake()
//         {
//             var rb = follow.GetComponent<Rigidbody>();
//             if (rb == null) return;
//             var speed = follow.GetComponent<Rigidbody>().velocity.magnitude;
//             var shake = motionShake.Update(speed, Time.deltaTime);
//             var relativeShake = transform.right * shake.x + transform.up * shake.y;
//             transform.position += relativeShake;
//         }
//
//         #endregion
//
//         // ===================== Utils ===================== //
//
//         #region Utils Methods
//
//         private float EaseLerpRingRadius(OrbitRing r1, OrbitRing r2)
//         {
//             var lerpState = Mathf.InverseLerp(r1.height, r2.height, referenceHeight);
//             if (r1.radius > r2.radius)
//                 lerpState = lerpState * lerpState;
//             else
//                 lerpState = Mathf.Sqrt(lerpState);
//             var radius = Mathf.Lerp(r1.radius, r2.radius, lerpState);
//             return radius;
//         }
//
//         private void DrawRing(OrbitRing ring)
//         {
// #if UNITY_EDITOR
//             Handles.color = ring.color;
//             var position = follow.transform.position + up * ring.height;
//             Handles.DrawWireDisc(position, up, ring.radius);
// #endif
//         }
//
//         #endregion
//
//         // ===================== Setters ===================== //
//
//         #region Setters Methods
//
//         public void SetFollow(GameObject follow)
//         {
//             this.follow = follow;
//         }
//
//         public void SetLookAt(GameObject lookAt)
//         {
//             this.lookAt = lookAt;
//         }
//
//         public void SetOrbitRing(OrbitPosition position, OrbitRing orbit)
//         {
//             if (position == OrbitPosition.Top)
//                 topRing = orbit;
//             else if (position == OrbitPosition.Middle)
//                 middleRing = orbit;
//             else if (position == OrbitPosition.Bottom) bottomRing = orbit;
//         }
//
//         public void SetLockHeight(bool lockHeight)
//         {
//             this.lockHeight = lockHeight;
//         }
//
//         public void SetLockTranslation(bool lockTranslation)
//         {
//             this.lockTranslation = lockTranslation;
//         }
//
//         public void SetAvoidClipping(bool avoidClipping)
//         {
//             this.avoidClipping = avoidClipping;
//         }
//
//         public void SetClipDistance(float clipDistance)
//         {
//             this.clipDistance = clipDistance;
//         }
//
//         public void SetClippingOffset(float clippingOffset)
//         {
//             this.clippingOffset = clippingOffset;
//         }
//
//         public void SetHorizontalTilt(float horizontalTilt)
//         {
//             this.horizontalTilt = horizontalTilt;
//         }
//
//         public void SetHorizontalOffset(float horizontalOffset)
//         {
//             this.horizontalOffset = horizontalOffset;
//         }
//
//         public void SetVerticalTilt(float verticalTilt)
//         {
//             this.verticalTilt = verticalTilt;
//         }
//
//         public void SetVerticalOffset(float verticalOffset)
//         {
//             this.verticalOffset = verticalOffset;
//         }
//
//         public void SetUseTargetNormal(bool useTargetNormal)
//         {
//             this.useTargetNormal = useTargetNormal;
//         }
//
//         public void SetCaptureCursor(bool captureCursor)
//         {
//             this.captureCursor = captureCursor;
//         }
//
//         public void SetHorizontalAxis(string horizontalAxis)
//         {
//             this.horizontalAxis = horizontalAxis;
//         }
//
//         public void SetHorizontalSensitivity(float horizontalSensitivity)
//         {
//             this.horizontalSensitivity = horizontalSensitivity;
//         }
//
//         public void SetInvertX(bool invertX)
//         {
//             this.invertX = invertX;
//         }
//
//         public void SetVerticalAxis(string verticalAxis)
//         {
//             this.verticalAxis = verticalAxis;
//         }
//
//         public void SetVerticalSensitivity(float verticalSensitivity)
//         {
//             this.verticalSensitivity = verticalSensitivity;
//         }
//
//         public void SetInvertY(bool invertY)
//         {
//             this.invertY = invertY;
//         }
//
//         public void SetZoomOutOnMotion(ZoomOutOnMotionEffect zoomOutOnMotion)
//         {
//             this.zoomOutOnMotion = zoomOutOnMotion;
//         }
//
//         public void SetMotionShake(MotionShakeEffect motionShake)
//         {
//             this.motionShake = motionShake;
//         }
//
//         #endregion
//     }
// }