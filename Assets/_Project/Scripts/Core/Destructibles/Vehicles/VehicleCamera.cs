using UnityEngine;

namespace Tanks.Core
{
    public class VehicleCamera : MonoBehaviour
    {
        [SerializeField] private Vehicle vehicle;
        [SerializeField] private Vector3 offset;
        [SerializeField] private float distance;
        [SerializeField] private float rotateTargetLerpRate;
        [SerializeField] private float maxLimitY;
        [SerializeField] private float minLimitY;
        [SerializeField] private float sensitivity;
        [SerializeField] private float offsetInterpolationSpeed;

        [SerializeField] private float minDistance;
        [SerializeField] private float distanceLerpRate;
        [SerializeField] private float distanceOffsetFromCollisionHit;

        public bool ShouldRotateTarget;
        [HideInInspector] public Vector2 RotationControl;

        private Vector3 targetOffset;
        private Vector3 defaultOffset;

        private float deltaRotationX;
        private float deltaRotationY;
        private float currentDistance;

        [Header("Zoom Optics")]
        [SerializeField] private GameObject zoomMaskEffect;
        private float defaultFov;
        [SerializeField] private float zoomFov;
        [SerializeField] private float zoomMaxVerticalAngle;

        private new Camera camera;
        public bool IsZoomed;
        private float defaultMaxVerticalAngle;
        private InputSystem_Actions actions;
        private float lastDistance;

        public static VehicleCamera Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        void Start()
        {
            actions = new InputSystem_Actions();
            actions.Enable();
            defaultOffset = offset;
            targetOffset = offset;

            camera = GetComponent<Camera>();
            defaultFov = camera.fieldOfView;
            defaultMaxVerticalAngle = maxLimitY;
        }

        void Update()
        {
            if (!vehicle) return;

            if (actions.Player.Zoom.triggered)
            {
                IsZoomed = !IsZoomed;

                if (IsZoomed)
                {
                    lastDistance = currentDistance;
                    currentDistance = minDistance;
                }
                else
                {
                    currentDistance = lastDistance;

                }
            }
            HandleCameraOffset();
            HandleCameraPositionAndRotation();
            HandleTargetRotation();
        }

        private void HandleCameraOffset()
        {
            if (offset != targetOffset)
            {
                offset = Vector3.MoveTowards(offset, targetOffset, Time.deltaTime * offsetInterpolationSpeed);
            }
        }

        private void HandleCameraPositionAndRotation()
        {
            deltaRotationX += RotationControl.x * sensitivity;
            deltaRotationY += RotationControl.y * -sensitivity;

            deltaRotationX %= 360f;
            deltaRotationY = ClampAngle(deltaRotationY, minLimitY, maxLimitY);

            Quaternion finalRotation = Quaternion.Euler(deltaRotationY, deltaRotationX, 0.0f);
            Vector3 finalPosition = vehicle.transform.position - (finalRotation * Vector3.forward * distance);
            finalPosition = AddLocalOffset(finalPosition);

            // Calculate current distance
            float targetDistance = distance;

            RaycastHit hit;
            Debug.DrawLine(vehicle.transform.position +  new Vector3(0, offset.y, 0), finalPosition, Color.red);
            if (Physics.Linecast(vehicle.transform.position + new Vector3(0, offset.y, 0), finalPosition, out hit))
            {
                if (hit.transform != vehicle && hit.distance < distance)
                {
                    targetDistance = hit.distance - distanceOffsetFromCollisionHit;
                }
            }

            currentDistance = Mathf.MoveTowards(currentDistance, targetDistance, Time.deltaTime * distanceLerpRate);
            currentDistance = Mathf.Clamp(currentDistance, minDistance, distance);

            finalPosition = vehicle.transform.position - (finalRotation * Vector3.forward * currentDistance);

            transform.rotation = finalRotation;
            transform.position = finalPosition;

            transform.position = AddLocalOffset(transform.position);

            // Zoom
            zoomMaskEffect.SetActive(IsZoomed);
            if (IsZoomed)
            {
                transform.position = vehicle.ZoomOpticsPoint.position;
                camera.fieldOfView = zoomFov;
                maxLimitY = zoomMaxVerticalAngle;
            }
            else
            {
                camera.fieldOfView = defaultFov;
                maxLimitY = defaultMaxVerticalAngle;
            }

        }

        private void HandleTargetRotation()
        {
            if (ShouldRotateTarget)
            {
                Quaternion targetRotation =
                    Quaternion.Euler(0, transform.eulerAngles.y, transform.eulerAngles.z);
                vehicle.transform.rotation = Quaternion.RotateTowards(vehicle.transform.rotation, targetRotation, Time.deltaTime * rotateTargetLerpRate);
            }
        }

        private Vector3 AddLocalOffset(Vector3 position)
        {
            Vector3 result = position;
            result += new  Vector3(0, offset.y, 0);
            result += transform.right * offset.x;
            result += transform.forward * offset.z;
            return result;
        }
        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
            {
                angle += 360F;
            }

            if (angle > 360F)
            {
                angle -= 360F;
            }
            return Mathf.Clamp(angle, min, max);
        }

        public void SetTargetOffset(Vector3 newOffset)
        {
            targetOffset = newOffset;
        }

        public void SetDefaultOffset()
        {
            targetOffset = defaultOffset;
        }

        public void SetTarget(Vehicle target)
        {
            vehicle = target;
        }
    }
}
