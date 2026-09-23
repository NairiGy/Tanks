using UnityEngine;

namespace Tanks.AI
{
    [System.Serializable]
    public class BushHidingSettings
    {
        [Tooltip("Bushes further than this (metres) are ignored.")]
        [SerializeField] private float searchRadius = 40f;

        [Tooltip("Seconds between searches for a nearby bush.")]
        [SerializeField] private float searchInterval = 0.5f;

        [Tooltip("Distance (metres) from the bush centre at which the bot stops.")]
        [SerializeField] private float arriveDistance = 1.5f;

        [Tooltip("Seconds the bot stays hidden once inside a bush.")]
        [SerializeField] private float hideDuration = 10f;

        [Tooltip("Seconds allowed to reach a bush before giving up on it.")]
        [SerializeField] private float approachTimeout = 20f;

        [Tooltip("Seconds before a bush the bot has used can be chosen again.")]
        [SerializeField] private float cooldown = 30f;

        public float SearchRadius => searchRadius;
        public float SearchInterval => searchInterval;
        public float ArriveDistance => arriveDistance;
        public float HideDuration => hideDuration;
        public float ApproachTimeout => approachTimeout;
        public float Cooldown => cooldown;
    }
}
