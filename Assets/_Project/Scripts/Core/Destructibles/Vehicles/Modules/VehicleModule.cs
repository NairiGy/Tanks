using UnityEngine;

namespace Tanks.Core
{
    public enum VehicleModuleState
    {
        Active,
        Disabled
    }
    public class VehicleModule : Destructible
    {
        public VehicleModuleState State;
        [SerializeField] protected float recoveryTime;
        [SerializeField] protected Vehicle vehicle;
        private float recoveryTimer;

        private void Start()
        {
            State = VehicleModuleState.Active;
        }

        private void Update()
        {
            if (State == VehicleModuleState.Active) return;

            if (State == VehicleModuleState.Disabled)
            {
                recoveryTimer += Time.deltaTime;

                if (recoveryTimer >= recoveryTime)
                {
                    OnModuleEnabled();
                }
            }
        }

        public float RepairPercentage()
        {
            return recoveryTimer / recoveryTime * 100f;
        }

        protected override void OnDestroyed()
        {
            base.OnDestroyed();

            OnModuleDisabled();
        }

        protected virtual void OnModuleDisabled()
        {
            State = VehicleModuleState.Disabled;

            recoveryTimer = 0;
        }

        protected virtual void OnModuleEnabled()
        {
            State = VehicleModuleState.Active;
        }
    }
}
