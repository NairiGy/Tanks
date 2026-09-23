using Mirror;
using UnityEngine;

namespace Tanks.Core
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private ProjectileProps props;
        [SerializeField] private ProjectileMovement movement;
        [SerializeField] private ProjectileHit hit;

        [SerializeField] private GameObject visualModel;

        [SerializeField] private float delayBeforeDestroy;
        [SerializeField] private float lifeTime;

        public ProjectileProps Props => props;
        public Player Shooter { get; set; }

        // Called once the launch position/direction has been set, right before the projectile starts moving.
        public void Launch()
        {
            movement.Launch();
        }

        private void Start()
        {
            Destroy(gameObject, lifeTime);
        }

        private void Update()
        {
            hit.Check();
            movement.Move();

            if (hit.DidHit) OnHit();

        }

        private void OnHit()
        {
            transform.position = hit.RaycastHit.point;
            ProjectileHitResult hitResult = hit.GetHitResult();

            if (NetworkServer.active)
            {
                if (hitResult.HitType == ProjectileHitType.Penetration || hitResult.HitType == ProjectileHitType.Nopenetration &&
                    props.Type == ProjectileType.HighExplosive)
                {
                    float dmg = Props.GetSpreadDamage();

                    hit.HitDestructible.SvTakeDamage((int)dmg);
                }

                if (hit.HitDestructible != null)
                {
                    Player targetPlayer = hit.HitDestructible.GetComponentInParent<Vehicle>()?.OwnerPlayer;

                    if (targetPlayer != null && Shooter != null)
                    {
                        CombatEvents.Instance.SvRaisePlayerHitPlayerEvent(Shooter, targetPlayer, hitResult);
                    }
                }
            }

            Destroy();
        }
        private void Destroy()
        {
            visualModel.SetActive(false);
            enabled = false;

            Destroy(gameObject, delayBeforeDestroy);
        }
    }
}
