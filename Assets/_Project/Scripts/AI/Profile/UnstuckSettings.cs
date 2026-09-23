using UnityEngine;

namespace Tanks.AI
{
    [System.Serializable]
    public class UnstuckSettings
    {
        [Tooltip("Seconds between progress checks while driving.")]
        [SerializeField] private float checkInterval = 2.5f;

        [Tooltip("Moving less than this many metres over a check interval counts as no progress.")]
        [SerializeField] private float minDistance = 1f;

        [Tooltip("Turning less than this many degrees over a check interval counts as no progress.")]
        [SerializeField] private float minTurnAngle = 15f;

        [Tooltip("Seconds spent reversing once stuck.")]
        [SerializeField] private float reverseTime = 1.5f;

        [Tooltip("Seconds spent turning away after reversing.")]
        [SerializeField] private float turnTime = 1f;

        public float CheckInterval => checkInterval;
        public float MinDistance => minDistance;
        public float MinTurnAngle => minTurnAngle;
        public float ReverseTime => reverseTime;
        public float TurnTime => turnTime;
    }
}
