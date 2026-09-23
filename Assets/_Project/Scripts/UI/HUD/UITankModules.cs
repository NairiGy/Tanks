using Tanks.Core;
using UnityEngine;

namespace Tanks.UI
{
    public class UITankModules : MonoBehaviour
    {
        [SerializeField] private UIRepairIndicator repairIndicatorPrefab;

        private bool initialized;
        private VehicleModule[] vehicleModules;

        private void Update()
        {
            if (initialized) return;

            if (Player.Local && Player.Local.ActiveVehicle)
            {
                vehicleModules = Player.Local.ActiveVehicle.VehicleModules;

                for (int i = 0; i < vehicleModules.Length; i++)
                {
                    vehicleModules[i].Destroyed += ShowUI;
                }

                initialized = true;
            }
        }

        private void OnDestroy()
        {
            if (vehicleModules == null) return;

            for (int i = 0; i < vehicleModules.Length; i++)
            {
                vehicleModules[i].Destroyed -= ShowUI;
            }
        }

        private void ShowUI(Destructible destructible)
        {
            var indicator = Instantiate(repairIndicatorPrefab, transform);
            indicator.Setup(destructible as VehicleModule);
        }
    }
}
