using Tanks.Core;
using UnityEngine;

namespace Tanks.AI
{
    // Holds the team's own base and stops to fire at anything it spots.
    public sealed class DefenderBehavior : AIBehavior
    {
        private readonly float arriveDistance;
        private readonly Transform ownBase;

        public DefenderBehavior(AIProfile profile, AIMapInfo map, Team team)
        {
            arriveDistance = profile.Navigation.ArriveDistance;
            ownBase = map.GetBase(team);
        }

        public override Vector3 GetDestination(Vehicle vehicle, out float stopDistance)
        {
            stopDistance = arriveDistance;
            return PositionOf(ownBase, vehicle);
        }
    }
}
