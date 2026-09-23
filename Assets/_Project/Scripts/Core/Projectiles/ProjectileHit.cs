using UnityEngine;

namespace Tanks.Core
{
    public enum ProjectileHitType
    {
        Penetration,
        Nopenetration,
        Ricochet,
        Environment
    }

    public class ProjectileHitResult
    {
        public ProjectileHitType HitType;
        public Vector3 HitPosition;
    }

    [RequireComponent(typeof(Projectile))]
    public class ProjectileHit : MonoBehaviour
    {
        private Projectile projectile;
        private RaycastHit raycastHit;
        public RaycastHit RaycastHit => raycastHit;
        private Destructible hitDestructible;
        public Destructible HitDestructible => hitDestructible;
        private const float RayAdvance = 1.1f;
        private static readonly RaycastHit[] HitBuffer = new RaycastHit[16];
        private bool didHit;
        public bool DidHit => didHit;
        private Armor hitArmor;

        private void Awake()
        {
            projectile = GetComponent<Projectile>();
        }

        public void Check()
        {
            if (didHit) return;

            float distance = projectile.Props.Velocity * Time.deltaTime * RayAdvance;
            int count = Physics.RaycastNonAlloc(transform.position, transform.forward, HitBuffer, distance);

            int nearest = -1;
            float nearestDistance = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                if (HitBuffer[i].distance >= nearestDistance) continue;
                if (HitBuffer[i].collider.GetComponent<Bush>() != null) continue;

                nearest = i;
                nearestDistance = HitBuffer[i].distance;
            }

            if (nearest < 0) return;

            raycastHit = HitBuffer[nearest];

            var destructible = raycastHit.transform.root.GetComponent<Destructible>();

            if (destructible)
            {
                hitDestructible = destructible;
            }

            Armor armor = raycastHit.collider.GetComponent<Armor>();

            if (armor)
            {
                hitArmor = armor;

                if (armor.ProtectedDestructible != null)
                {
                    hitDestructible = armor.ProtectedDestructible;
                }
            }

            didHit = true;
        }

        public ProjectileHitResult GetHitResult()
        {
            ProjectileHitResult result = new ProjectileHitResult();

            if (hitArmor == null)
            {
                result.HitType = ProjectileHitType.Environment;
                result.HitPosition = raycastHit.point;

                return result;
            }

            float normalization = projectile.Props.NormalizationAngle;

            if (projectile.Props.Caliber > hitArmor.Thickness * 2)
            {
                normalization = (projectile.Props.NormalizationAngle * 1.4f * projectile.Props.Caliber) / hitArmor.Thickness;
            }

            float angle = Mathf.Abs(Vector3.SignedAngle(-projectile.transform.forward, raycastHit.normal, projectile.transform.right)) - normalization;

            float reducedArmor = hitArmor.Thickness / Mathf.Cos(angle *  Mathf.Deg2Rad);
            float projectilePenetration = projectile.Props.GetSpreadArmorPenetration();

            if (angle > projectile.Props.RicochetAngle && projectile.Props.Caliber < hitArmor.Thickness * 3)
            {
                result.HitType = ProjectileHitType.Ricochet;
            }

            else if (projectilePenetration >= reducedArmor)
            {
                result.HitType = ProjectileHitType.Penetration;
            }
            else
            {
                result.HitType = ProjectileHitType.Nopenetration;
            }

            result.HitPosition = raycastHit.point;
            return result;
        }
    }
}
