using UnityEngine;

namespace Tanks.AI
{
    [System.Serializable]
    public class NavigationSettings
    {
        [Tooltip("Angle (degrees) to the next waypoint at which steering reaches full lock.")]
        [SerializeField] private float steerAngleRange = 45f;

        [Tooltip("Heading error (degrees) below which the bot drives straight.")]
        [SerializeField] private float steerDeadZoneAngle = 10f;

        [Tooltip("Distance (metres) from the destination at which the bot stops.")]
        [SerializeField] private float arriveDistance = 5f;

        [Tooltip("A path corner closer than this counts as reached; the bot steers to the next one.")]
        [SerializeField] private float cornerReachDistance = 4f;

        [Tooltip("Maximum distance (metres) to snap the vehicle and destination onto the NavMesh.")]
        [SerializeField] private float navMeshSampleDistance = 20f;

        [Tooltip("Seconds between path recalculations.")]
        [SerializeField] private float repathInterval = 0.5f;

        public float SteerAngleRange => steerAngleRange;
        public float SteerDeadZoneAngle => steerDeadZoneAngle;
        public float ArriveDistance => arriveDistance;
        public float CornerReachDistance => cornerReachDistance;
        public float NavMeshSampleDistance => navMeshSampleDistance;
        public float RepathInterval => repathInterval;
    }
}
