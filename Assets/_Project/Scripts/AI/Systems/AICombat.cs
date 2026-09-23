using Tanks.Core;
using UnityEngine;

namespace Tanks.AI
{
    // Finds a spotted enemy, aims the turret at it and fires once aligned and slow enough.
    public sealed class AICombat
    {
        private readonly CombatSettings settings;

        public AICombat(CombatSettings settings)
        {
            this.settings = settings;
        }

        public bool TryEngage(Vehicle self, Team team)
        {
            Vehicle enemy = FindEnemy(self, team);
            if (enemy == null) return false;

            Vector3 aimPoint = enemy.transform.position + Vector3.up * settings.AimHeightOffset;
            self.SvSetAimPoint(aimPoint);

            if (!IsAimedAt(self, aimPoint)) return false;

            if (self.LinearVelocity <= settings.MaxFireSpeed)
            {
                self.Fire();
            }

            return true;
        }

        private static Vehicle FindEnemy(Vehicle self, Team team)
        {
            if (PlayerList.Instance == null || TankSpottingInterestManagement.Instance == null) return null;

            Vehicle nearest = null;
            float nearestDistance = float.MaxValue;

            foreach (Player player in PlayerList.Instance.allPlayers)
            {
                if (player == null || player.TeamId == team) continue;

                Vehicle candidate = player.ActiveVehicle;
                if (candidate == null || candidate.HitPoints <= 0) continue;

                if (!TankSpottingInterestManagement.Instance.IsSpottedByTeam(candidate, team)) continue;

                float distance = Vector3.Distance(self.transform.position, candidate.transform.position);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = candidate;
                }
            }

            return nearest;
        }

        private bool IsAimedAt(Vehicle self, Vector3 aimPoint)
        {
            Turret turret = self.Turret;

            if (turret == null || turret.LaunchPoint == null) return false;

            Vector3 toTarget = aimPoint - turret.LaunchPoint.position;

            return Vector3.Angle(turret.LaunchPoint.forward, toTarget) <= settings.AimToleranceAngle;
        }
    }
}
