using UnityEngine;

namespace Tanks.Core
{
    [RequireComponent(typeof(Player))]
    public class VehicleInputControl : MonoBehaviour
    {
        private const float AimDistance = 1000;

        /// <summary>True while a menu is open: the tank stops and the camera ignores the mouse.</summary>
        public static bool IsBlocked;

        [SerializeField] private Vector3 cameraOffset;

        private Player player;
        private VehicleCamera vehicleCamera;
        private InputSystem_Actions actions;

        private Vehicle ActiveVehicle => player.ActiveVehicle;

        private void Awake()
        {
            player = GetComponent<Player>();
            vehicleCamera = Camera.main.GetComponent<VehicleCamera>();
            actions = new InputSystem_Actions();
        }

        private void Start()
        {
            if (vehicleCamera != null)
            {
                vehicleCamera.ShouldRotateTarget = false;
            }
        }

        // The component is switched off at match end and back on at restart,
        // so the actions must follow every enable, not just the first one.
        private void OnEnable()
        {
            actions.Enable();
        }

        private void OnDisable()
        {
            actions.Disable();
        }

        private void OnDestroy()
        {
            actions.Dispose();
        }

        private void Update()
        {
            if (player == null) return;
            if (player.ActiveVehicle == null) return;
            if (!player.isOwned || !player.isLocalPlayer) return;

            if (IsBlocked)
            {
                ActiveVehicle.SetInputControl(Vector3.zero);
                vehicleCamera.RotationControl = Vector2.zero;
                return;
            }

            Vector2 move = actions.Player.Move.ReadValue<Vector2>();
            ActiveVehicle.SetInputControl(new Vector3(move.x, actions.Player.Jump.ReadValue<float>(), move.y));

            vehicleCamera.RotationControl = actions.Player.Look.ReadValue<Vector2>();

            if (actions.Player.Attack.triggered)
            {
                ActiveVehicle.Fire();
            }

            ActiveVehicle.NetAimPoint = TraceAimPointWithoutPlayerVehicle(
                VehicleCamera.Instance.transform.position,
                VehicleCamera.Instance.transform.forward);

            if (actions.Player.ChooseAmmo1.triggered) ActiveVehicle.Turret.ChooseAmmunition(0);
            if (actions.Player.ChooseAmmo2.triggered) ActiveVehicle.Turret.ChooseAmmunition(1);
        }

        public void AssignCamera(VehicleCamera camera)
        {
            vehicleCamera = camera;
            vehicleCamera.ShouldRotateTarget = false;
            vehicleCamera.SetTargetOffset(cameraOffset);
            vehicleCamera.SetTarget(ActiveVehicle);
        }

        public static Vector3 TraceAimPointWithoutPlayerVehicle(Vector3 start, Vector3 direction)
        {
            Ray ray = new Ray(start, direction);

            RaycastHit[] hits = Physics.RaycastAll(ray, AimDistance);

            var playerBody = Player.Local.ActiveVehicle.GetComponent<Rigidbody>();

            foreach (var hit in hits)
            {
                if (hit.rigidbody == playerBody) continue;

                return hit.point;
            }

            return ray.GetPoint(AimDistance);
        }
    }
}
