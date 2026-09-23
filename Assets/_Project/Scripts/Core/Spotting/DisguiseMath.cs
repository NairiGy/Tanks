using UnityEngine;

namespace Tanks.Core
{
    public static class DisguiseMath
    {
        // The disguise value a vehicle drifts towards: base value, lowered while moving or just fired,
        // raised while sitting in a bush, clamped to 0..1.
        public static float TargetDisguise(
            float baseDisguise,
            bool isMoving,
            float movingPenalty,
            bool isInBush,
            float bushBonus,
            bool hasRecentlyFired,
            float firingPenalty)
        {
            float target = baseDisguise;

            if (isMoving) target -= movingPenalty;
            if (isInBush) target += bushBonus;
            if (hasRecentlyFired) target -= firingPenalty;

            return Mathf.Clamp01(target);
        }
    }
}
