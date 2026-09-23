using Tanks.Core;
using UnityEngine;

namespace Tanks.AI
{
    // Hops between nearby bushes, otherwise advances on the enemy base; stops to fire when it can.
    public sealed class DefaultBehavior : AIBehavior
    {
        private readonly float arriveDistance;
        private readonly float bushArriveDistance;
        private readonly Transform enemyBase;
        private readonly BushHider bushHider;

        public DefaultBehavior(AIProfile profile, AIMapInfo map, Team team)
        {
            arriveDistance = profile.Navigation.ArriveDistance;
            bushArriveDistance = profile.BushHiding.ArriveDistance;
            enemyBase = map.GetBase(TeamUtils.GetOpponent(team));
            bushHider = new BushHider(profile.BushHiding, map.BushAreas);
        }

        public override void OnVehicleChanged()
        {
            bushHider.Reset();
        }

        public override void OnNavigationFailed()
        {
            bushHider.OnNavigationFailed();
        }

        public override Vector3 GetDestination(Vehicle vehicle, out float stopDistance)
        {
            if (bushHider.TryGetDestination(vehicle, out Vector3 bushPosition))
            {
                stopDistance = bushArriveDistance;
                return bushPosition;
            }

            stopDistance = arriveDistance;
            return PositionOf(enemyBase, vehicle);
        }
    }
}
