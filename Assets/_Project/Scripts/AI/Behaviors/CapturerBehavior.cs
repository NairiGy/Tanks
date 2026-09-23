using Tanks.Core;
using UnityEngine;

namespace Tanks.AI
{
    // Takes a few detours through the map corners, then captures the enemy base without stopping to fight.
    public sealed class CapturerBehavior : AIBehavior
    {
        private readonly float arriveDistance;
        private readonly float cornerArriveDistance;
        private readonly Transform enemyBase;
        private readonly CornerRoamer cornerRoamer;

        public override bool HoldsPositionToFire => false;

        public CapturerBehavior(AIProfile profile, AIMapInfo map, Team team)
        {
            arriveDistance = profile.Navigation.ArriveDistance;
            cornerArriveDistance = profile.CornerRoaming.ArriveDistance;
            enemyBase = map.GetBase(TeamUtils.GetOpponent(team));
            cornerRoamer = new CornerRoamer(profile.CornerRoaming, map);
        }

        public override void OnVehicleChanged()
        {
            cornerRoamer.Reset();
        }

        public override void OnNavigationFailed()
        {
            cornerRoamer.OnNavigationFailed();
        }

        public override Vector3 GetDestination(Vehicle vehicle, out float stopDistance)
        {
            if (cornerRoamer.TryGetDestination(vehicle, out Vector3 cornerPosition))
            {
                stopDistance = cornerArriveDistance;
                return cornerPosition;
            }

            stopDistance = arriveDistance;
            return PositionOf(enemyBase, vehicle);
        }
    }
}
