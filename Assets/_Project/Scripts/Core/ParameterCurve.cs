using UnityEngine;

namespace Tanks.Core
{
    [System.Serializable]
    public class ParameterCurve
    {
        [SerializeField] private AnimationCurve curve;
        [SerializeField] private float duration = 1;

        private float expiredTime;

        public float MoveForward(float dt)
        {
            expiredTime += dt;

            return curve.Evaluate(expiredTime / duration);
        }

        public float Restart()
        {
            expiredTime = 0;

            return curve.Evaluate(0);
        }

    }
}
