using UnityEngine;

namespace Tanks.AI
{
    [System.Serializable]
    public class CornerRoamingSettings
    {
        [Tooltip("Minimum number of map corners to visit before heading for the enemy base.")]
        [SerializeField] private int minLegs = 1;

        [Tooltip("Maximum number of map corners to visit before heading for the enemy base.")]
        [SerializeField] private int maxLegs = 3;

        [Tooltip("Corners are taken this many metres inside the edge of the NavMesh bounds.")]
        [SerializeField] private float mapInset = 15f;

        [Tooltip("Maximum distance (metres) to snap a corner onto the NavMesh.")]
        [SerializeField] private float sampleDistance = 60f;

        [Tooltip("Distance (metres) from a corner at which the leg counts as finished.")]
        [SerializeField] private float arriveDistance = 12f;

        [Tooltip("Seconds allowed per leg before moving on.")]
        [SerializeField] private float legTimeout = 45f;

        public int MinLegs => minLegs;
        public int MaxLegs => maxLegs;
        public float MapInset => mapInset;
        public float SampleDistance => sampleDistance;
        public float ArriveDistance => arriveDistance;
        public float LegTimeout => legTimeout;
    }
}
