using System;
using Tanks.Core;
using UnityEngine;

namespace Tanks.Match
{
    [RequireComponent(typeof(BoxCollider))]
    public class TeamBase : MonoBehaviour
    {
        [SerializeField] private Team team;
        public Team Team => team;

        public event Action EnemyEntered;
        public event Action EnemyExited;

        private void OnTriggerEnter(Collider other)
        {
            var vehicle = other.transform.root.GetComponent<Vehicle>();
        
            if (vehicle == null) return;
            if (vehicle.OwnerPlayer == null) return;
            if (vehicle.OwnerPlayer.TeamId == team) return;

            EnemyEntered?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            var vehicle = other.transform.root.GetComponent<Vehicle>();
        
            if (vehicle == null) return;
            if (vehicle.OwnerPlayer == null) return;
            if (vehicle.OwnerPlayer.TeamId == team) return;

            EnemyExited?.Invoke();
        }
    }
}
