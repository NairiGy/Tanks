using Tanks.Core;
using TMPro;
using UnityEngine;

namespace Tanks.UI
{
    public class UIRepairIndicator : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        private VehicleModule vehicleModule;

        private void Update()
        {
            text.text = $"{vehicleModule.gameObject.name} : {(int)vehicleModule.RepairPercentage()} %";

            if (vehicleModule.RepairPercentage() >= 100f)
            {
                Destroy(gameObject);
            }
        }

        public void Setup(VehicleModule module)
        {
            vehicleModule = module;
        }
    }
}
