using System;
using Mirror;
using UnityEngine;

namespace Tanks.Core
{
    public class Turret : NetworkBehaviour
    {
        [SerializeField] private Ammunition[]  ammunitions;
        public Ammunition[] Ammunitions => ammunitions;

        [SyncVar]
        [SerializeField] protected Ammunition currentAmmunition;
        [SerializeField] protected Transform launchPoint;
        public Transform LaunchPoint => launchPoint;
        public event Action AmmoCountChanged;

        [SerializeField] private float fireRate;

        public event Action<uint> AmmunitionsChanged;
        private float fireTimer;
        public float FireTimerNormalize => fireTimer / fireRate;

        public override void OnStartServer()
        {
            base.OnStartServer();

            if (currentAmmunition == null && ammunitions.Length > 0)
            {
                currentAmmunition = ammunitions[0];
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            ChooseAmmunition(0);
        }

        protected virtual void Update()
        {
            if (fireTimer > 0)
            {
                fireTimer -= Time.deltaTime;
            }
        }

        public void ChooseAmmunition(uint ind)
        {
            if (!isOwned) return; // only the owning client should be able to request this
            CmdChooseAmmunition(ind);
        }

        [Command]
        private void CmdChooseAmmunition(uint ind)
        {
            if (ind >= ammunitions.Length || ammunitions[ind] == null) return;

            currentAmmunition = ammunitions[ind];
            RpcChooseAmmunition(ind);
        }

        [ClientRpc]
        public void RpcChooseAmmunition(uint ind)
        {
            currentAmmunition = ammunitions[ind];
            AmmunitionsChanged?.Invoke(ind);
        }

        [Server]
        protected virtual bool SvDrawAmmo(uint count)
        {
            if (!currentAmmunition.TryDrawAmmo(count)) return false;
            AmmoCountChanged?.Invoke();
            RpcDrawAmmo(count);

            return true;
        }

        [ClientRpc]
        public void RpcDrawAmmo(uint count)
        {
            if (isServer) return;
            currentAmmunition.TryDrawAmmo(count);
            AmmoCountChanged?.Invoke();
        }

        protected virtual void OnFire() {}

        public void Fire()
        {
            if (isOwned)
            {
                if (isClient)
                {
                    CmdFire();
                }

                return;
            }

            if (isServer && GetComponent<Vehicle>() is { IsBotControlled: true })
            {
                SvFire();
            }
        }

        [Command]
        private void CmdFire()
        {
            SvFire();
        }

        [Server]
        private void SvFire()
        {
            Vehicle vehicle = GetComponent<Vehicle>();
            if (vehicle != null && vehicle.HitPoints <= 0) return;

            if (fireTimer > 0) return;

            if (SvDrawAmmo(1) == false) return;

            OnFire();

            fireTimer = fireRate;

            GetComponent<Vehicle>()?.NotifyFired();

            RpcFire();
        }

        [ClientRpc]
        private void RpcFire()
        {
            if (isServer) return;

            fireTimer = fireRate;

            OnFire();
        }
    }
}
