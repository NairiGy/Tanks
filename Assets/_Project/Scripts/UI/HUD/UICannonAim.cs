using Tanks.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Tanks.UI
{
    public class UICannonAim : MonoBehaviour
    {
        [SerializeField] private Image aim;
        private Vector3 aimPosition;

        private void Update()
        {
            if (Player.Local == null) return;
            if (Player.Local.ActiveVehicle == null) return;

            Vehicle v = Player.Local.ActiveVehicle;

            aimPosition = VehicleInputControl.TraceAimPointWithoutPlayerVehicle(v.Turret.LaunchPoint.position, v.Turret.LaunchPoint.forward);

            Vector3 aimDirection = Camera.main.WorldToScreenPoint(aimPosition);

            if (aimDirection.z > 0)
            {
                aim.transform.position = aimDirection;
            }
        }
    }
}
