using UnityEngine;

namespace Tanks.Core
{
    public class TankTrackModule : VehicleModule
    {
        [SerializeField] private TrackSide side;

        protected override void OnModuleDisabled()
        {
            base.OnModuleDisabled();

            if (vehicle is TrackTank trackTank)
            {
                trackTank.SetTrackDisabled(side, true);
            }
        }

        protected override void OnModuleEnabled()
        {
            base.OnModuleEnabled();

            if (vehicle is TrackTank trackTank)
            {
                trackTank.SetTrackDisabled(side, false);
            }
        }
    }
}
