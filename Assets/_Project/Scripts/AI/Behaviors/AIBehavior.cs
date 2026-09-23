using Tanks.Core;
using UnityEngine;

namespace Tanks.AI
{
    // A bot role: decides where the bot wants to go and whether it stops to fight.
    // Aiming, driving and unsticking are handled elsewhere, so a behavior stays small.
    public abstract class AIBehavior
    {
        public virtual bool HoldsPositionToFire => true;

        public virtual void OnVehicleChanged()
        {
        }

        public virtual void OnNavigationFailed()
        {
        }

        public abstract Vector3 GetDestination(Vehicle vehicle, out float stopDistance);

        protected static Vector3 PositionOf(Transform target, Vehicle fallback)
        {
            return target != null ? target.position : fallback.transform.position;
        }
    }
}
