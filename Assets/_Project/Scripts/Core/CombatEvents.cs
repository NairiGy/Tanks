using System;
using Mirror;

namespace Tanks.Core
{
    public class CombatEvents : NetworkBehaviour
    {
        public static CombatEvents Instance { get; private set; }

        public event Action<Player, Player, ProjectileHitResult> PlayerHitPlayerEvent;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        [Server]
        public void SvRaisePlayerHitPlayerEvent(Player player, Player target, ProjectileHitResult result)
        {
            PlayerHitPlayerEvent?.Invoke(player, target, result);
            RpcPlayerHitPlayerEvent(player, target, result);
        }

        [ClientRpc]
        public void RpcPlayerHitPlayerEvent(Player player, Player target, ProjectileHitResult result)
        {
            PlayerHitPlayerEvent?.Invoke(player, target, result);
        }
    }
}
