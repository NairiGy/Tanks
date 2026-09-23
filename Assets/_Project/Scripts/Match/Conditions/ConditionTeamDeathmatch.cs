using System.Collections.Generic;
using Mirror;
using Tanks.Core;
using UnityEngine;

namespace Tanks.Match
{
    public class ConditionTeamDeathmatch : MonoBehaviour, IMatchCondition
    {
        [SerializeField] private float checkInterval = 0.25f;

        private Team victoriousTeam;
        public Team VictoriousTeam => victoriousTeam;
        private bool triggered;

        public bool IsTriggered => triggered;

        private MatchController match;
        private readonly DeathmatchTracker tracker = new DeathmatchTracker();
        private float nextCheckTime;
        private readonly HashSet<Player> counted = new HashSet<Player>();

        public void OnServerMatchStart(MatchController controller)
        {
            match = controller;
            triggered = false;
            victoriousTeam = Team.None;
            tracker.Reset();
        }

        private void Update()
        {
            if (!NetworkServer.active || triggered) return;
            if (match == null || !match.IsMatchActive) return;
            if (Time.time < nextCheckTime) return;

            nextCheckTime = Time.time + checkInterval;

            CountAlive(out int red, out int blue);

            Team? result = tracker.Evaluate(red, blue);
            if (result == null) return;

            triggered = true;
            victoriousTeam = result.Value;
        }

        private void CountAlive(out int red, out int blue)
        {
            red = 0;
            blue = 0;
            counted.Clear();

            if (PlayerList.Instance == null) return;

            foreach (Player player in PlayerList.Instance.allPlayers)
            {
                if (player == null || !counted.Add(player)) continue;

                Vehicle vehicle = player.ActiveVehicle;
                if (vehicle == null || vehicle.HitPoints <= 0) continue;

                if (player.TeamId == Team.Red) red++;
                else if (player.TeamId == Team.Blue) blue++;
            }
        }

        public void OnServerMatchEnd(MatchController controller)
        {
        }
    }
}
