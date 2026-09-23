using UnityEngine;

namespace Tanks.Core
{
    [RequireComponent(typeof(TrackTank))]
    public class TankTurret : Turret
    {
        private TrackTank tank;

        [SerializeField] private Transform tower;
        [SerializeField] private Transform mask;

        [SerializeField] private float horizontalRotationSpeed;
        [SerializeField] private float verticalRotationSpeed;

        [SerializeField] private float maxTopAngle;
        [SerializeField] private float maxBottomAngle;

        [Header("Sfx")]
        [SerializeField] private AudioSource fireSfx;
        [SerializeField] private ParticleSystem fireVfx;
        [SerializeField] private float recoilForce;

        private float maskCurrentAngle;

        private void Start()
        {
            tank = GetComponent<TrackTank>();
            maxTopAngle = -maxTopAngle;

            ControlTurretAim(true);
        }

        protected override void Update()
        {
            base.Update();

            ControlTurretAim(false);
        }

        protected override void OnFire()
        {
            base.OnFire();

            GameObject projectileObj = Instantiate(currentAmmunition.Projectile.Props.ProjectilePrefab);
            projectileObj.transform.position = launchPoint.position;
            projectileObj.transform.forward = launchPoint.forward;

            Projectile projectile = projectileObj.GetComponent<Projectile>();
            projectile.Shooter = GetComponent<Vehicle>()?.OwnerPlayer;
            projectile.Launch();

            FireFx();
        }

        private void ControlTurretAim(bool instant)
        {
            // Tower
            Vector3 lp = tower.InverseTransformPoint(tank.NetAimPoint);
            lp.y = 0;
            Vector3 lpg = tower.TransformPoint(lp);

            Quaternion targetTowerRotation = Quaternion.LookRotation((lpg - tower.position).normalized, tower.up);

            tower.rotation = instant
                ? targetTowerRotation
                : Quaternion.RotateTowards(tower.rotation, targetTowerRotation, horizontalRotationSpeed * Time.deltaTime);

            // Mask
            mask.localRotation = Quaternion.identity;

            lp = mask.InverseTransformPoint(tank.NetAimPoint);
            lp.x = 0;
            lpg = mask.TransformPoint(lp);

            float targetAngle = -Vector3.SignedAngle((lpg - mask.position).normalized, mask.forward, mask.right);
            targetAngle = Mathf.Clamp(targetAngle, maxTopAngle, maxBottomAngle);

            maskCurrentAngle = instant
                ? targetAngle
                : Mathf.MoveTowards(maskCurrentAngle, targetAngle, verticalRotationSpeed * Time.deltaTime);
            mask.localRotation = Quaternion.Euler(maskCurrentAngle, 0, 0);

        }

        public void FireFx()
        {
            fireSfx.Play();
            fireVfx.Play();

            var rb = tank.GetComponent<Rigidbody>();
            rb.AddForceAtPosition(-mask.forward * recoilForce, mask.position, ForceMode.Impulse);
        }
    }
}
