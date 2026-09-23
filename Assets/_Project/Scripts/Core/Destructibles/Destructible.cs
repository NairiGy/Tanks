using System;
using Mirror;
using UnityEngine;

namespace Tanks.Core
{
    public class Destructible : NetworkBehaviour
    {
        public event Action<int> HitPointChange;
        public event Action<Destructible> Destroyed;

        [SerializeField] private int maxHitPoints;
        public int MaxHitPoints => maxHitPoints;

        [SyncVar(hook = nameof(ChangeHitPoints))]
        private int syncCurrentHitpoints;
        public int HitPoints => syncCurrentHitpoints;

        public float HitPointsPercentage()
        {
            var percentage = (float)syncCurrentHitpoints / (float)maxHitPoints * 100f;
            return percentage <= 0f ? 0f : percentage;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            syncCurrentHitpoints = maxHitPoints;
        }

        [Server]
        public void SvTakeDamage(int damage)
        {
            syncCurrentHitpoints -= damage;

            if (syncCurrentHitpoints <= 0)
            {
                RpcDestroy();
            }
        }

        [ClientRpc]
        protected void RpcDestroy()
        {
            OnDestroyed();
        }

        protected virtual void OnDestroyed()
        {
            Destroyed?.Invoke(this);
        }

        private void ChangeHitPoints(int oldValue, int newValue)
        {
            HitPointChange?.Invoke(newValue);
        }
    }
}
