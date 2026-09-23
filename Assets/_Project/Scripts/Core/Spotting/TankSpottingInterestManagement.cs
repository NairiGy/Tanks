using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Tanks.Core
{
    public class TankSpottingInterestManagement : InterestManagement
    {
        public static TankSpottingInterestManagement Instance { get; private set; }

        [SerializeField] private float rebuildInterval = 0.5f;
        [SerializeField] private LayerMask visionObstructionMask;

        private double lastRebuildTime;

        protected override void OnEnable()
        {
            base.OnEnable();

            Instance = this;
        }

        private void OnDisable()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        [ServerCallback]
        void LateUpdate()
        {
            if (NetworkTime.localTime >= lastRebuildTime + rebuildInterval)
            {
                RebuildAll();
                lastRebuildTime = NetworkTime.localTime;
            }
        }

        [ServerCallback]
        public override void ResetState()
        {
            lastRebuildTime = 0d;
        }

        public override bool OnCheckObserver(NetworkIdentity identity, NetworkConnectionToClient newObserver)
        {
            Vehicle vehicle = identity.GetComponent<Vehicle>();
            Player ownerPlayer = vehicle != null ? vehicle.OwnerPlayer : null;

            if (vehicle == null || ownerPlayer == null) return true;

            Player observerPlayer = newObserver.identity != null ? newObserver.identity.GetComponent<Player>() : null;
            if (observerPlayer == null) return true;

            if (observerPlayer.TeamId == ownerPlayer.TeamId) return true;

            return IsSpottedByTeam(vehicle, observerPlayer.TeamId);
        }

        public override void OnRebuildObservers(NetworkIdentity identity, HashSet<NetworkConnectionToClient> newObservers)
        {
            Vehicle vehicle = identity.GetComponent<Vehicle>();
            Player ownerPlayer = vehicle != null ? vehicle.OwnerPlayer : null;

            if (vehicle == null || ownerPlayer == null || PlayerList.Instance == null)
            {
                AddAllConnections(newObservers);
                return;
            }

            Team enemyTeam = TeamUtils.GetOpponent(ownerPlayer.TeamId);
            bool spottedByEnemyTeam = IsSpottedByTeam(vehicle, enemyTeam);

            foreach (Player player in PlayerList.Instance.allPlayers)
            {
                if (player == null || player.connectionToClient == null) continue;

                if (player.TeamId == ownerPlayer.TeamId || (player.TeamId == enemyTeam && spottedByEnemyTeam))
                {
                    newObservers.Add(player.connectionToClient);
                }
            }
        }

        public bool IsSpottedByTeam(Vehicle target, Team team)
        {
            if (PlayerList.Instance == null) return true;

            foreach (Player player in PlayerList.Instance.allPlayers)
            {
                if (player == null || player.TeamId != team) continue;

                Vehicle observer = player.ActiveVehicle;
                if (observer == null || observer == target) continue;

                if (CanDetect(observer, target)) return true;
            }

            return false;
        }

        private bool CanDetect(Vehicle observer, Vehicle target)
        {
            Vector3 observerPos = observer.transform.position;
            Vector3 targetPos = target.transform.position;
            float distance = Vector3.Distance(observerPos, targetPos);

            if (distance <= observer.AlwaysVisibleRadius) return true;

            float effectiveRange = SpottingMath.EffectiveDetectionRange(observer.AlwaysVisibleRadius, observer.MaxDetectionRange, target.CurrentDisguise);
            if (distance > effectiveRange) return false;

            return !Physics.Linecast(observerPos, targetPos, visionObstructionMask, QueryTriggerInteraction.Collide);
        }

        private void AddAllConnections(HashSet<NetworkConnectionToClient> newObservers)
        {
            foreach (NetworkConnectionToClient conn in NetworkServer.connections.Values)
            {
                if (conn != null && conn.isAuthenticated && conn.identity != null)
                {
                    newObservers.Add(conn);
                }
            }
        }
    }
}
