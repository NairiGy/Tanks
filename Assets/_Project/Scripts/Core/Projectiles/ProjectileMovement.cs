using UnityEngine;

namespace Tanks.Core
{
    [RequireComponent(typeof(Projectile))]
    public class ProjectileMovement : MonoBehaviour
    {
        private Projectile projectile;
        private Vector3 step;
        [SerializeField] private float spreadAngle = 5f;
        private void Awake()
        {
            projectile = GetComponent<Projectile>();
        }

        // Called once the launch position/direction has been set, right before the projectile starts moving.
        public void Launch()
        {
            ApplySpread();
        }

        private void ApplySpread()
        {
            if (spreadAngle <= 0f)
                return;

            // Random point inside a cone around the current forward direction
            float randomAngle = UnityEngine.Random.Range(0f, spreadAngle);
            float randomRotation = UnityEngine.Random.Range(0f, 360f);

            Quaternion spreadRotation = Quaternion.AngleAxis(randomRotation, transform.forward)
                                        * Quaternion.AngleAxis(randomAngle, Vector3.Cross(transform.forward, Vector3.up).normalized);

            transform.forward = spreadRotation * transform.forward;
        }

        public void Move()
        {
            transform.forward = Vector3.Lerp(transform.forward, -Vector3.up, Mathf.Clamp01(projectile.Props.Mass * Time.deltaTime)).normalized;

            step = transform.forward * projectile.Props.Velocity * Time.deltaTime;

            transform.position += step;
        }
    }
}
