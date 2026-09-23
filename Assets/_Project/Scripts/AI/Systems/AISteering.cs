using UnityEngine;

namespace Tanks.AI
{
    public static class AISteering
    {
        // signedAngle is positive when the waypoint is to the right; returns a steering value in -1..1.
        public static float Compute(float signedAngle, float deadZoneAngle, float angleRange)
        {
            if (Mathf.Abs(signedAngle) < deadZoneAngle) return 0f;

            return Mathf.Clamp(signedAngle / angleRange, -1f, 1f);
        }
    }
}
