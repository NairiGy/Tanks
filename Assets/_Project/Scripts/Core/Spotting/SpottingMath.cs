using UnityEngine;

namespace Tanks.Core
{
    public static class SpottingMath
    {
        // Disguise 0 is fully exposed (max range); disguise 1 can only be spotted at point-blank range.
        public static float EffectiveDetectionRange(float alwaysVisibleRadius, float maxDetectionRange, float targetDisguise)
        {
            return Mathf.Lerp(alwaysVisibleRadius, maxDetectionRange, 1f - targetDisguise);
        }
    }
}
