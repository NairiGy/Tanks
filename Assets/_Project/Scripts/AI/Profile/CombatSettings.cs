using UnityEngine;

namespace Tanks.AI
{
    [System.Serializable]
    public class CombatSettings
    {
        [Tooltip("Maximum angle (degrees) between the muzzle and the target for the bot to count as aimed.")]
        [SerializeField] private float aimToleranceAngle = 2f;

        [Tooltip("Height (metres) above the enemy's pivot to aim at.")]
        [SerializeField] private float aimHeightOffset = 1.5f;

        [Tooltip("The bot only fires when moving slower than this (m/s).")]
        [SerializeField] private float maxFireSpeed = 1f;

        public float AimToleranceAngle => aimToleranceAngle;
        public float AimHeightOffset => aimHeightOffset;
        public float MaxFireSpeed => maxFireSpeed;
    }
}
