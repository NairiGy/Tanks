using System;
using Mirror;
using Tanks.Core;
using UnityEngine;

namespace Tanks.Match
{
    public interface IMatchCondition
    {
        bool IsTriggered { get; }
        void OnServerMatchStart(MatchController controller);
        void OnServerMatchEnd(MatchController controller);
    }
    public class MatchController : NetworkBehaviour
    {
        [SyncVar] private Team victoriousTeam;
        public Team VictoriousTeam => victoriousTeam;

        public event Action MatchStart;
        public event Action<Team> MatchEnd;

        private bool hasAutoStarted;

        [SyncVar] private bool isMatchActive;
        public bool IsMatchActive => isMatchActive;

        private IMatchCondition[] matchConditions;

        private void Awake()
        {
            matchConditions = GetComponentsInChildren<IMatchCondition>();
        }

        private void OnDestroy()
        {
            Time.timeScale = 1f;
        }

        private void Update()
        {
            if (isServer)
            {
                if (isMatchActive)
                {
                    foreach (var c in matchConditions)
                    {
                        if (c.IsTriggered)
                        {
                            SvEndMatch();
                            break;
                        }
                    }
                }
                else
                {
                    if (hasAutoStarted) return;

                    hasAutoStarted = true;
                    SvRestartMatch();
                }
            }
        }

        [Server]
        public void SvRestartMatch()
        {
            if (isMatchActive) return;

            isMatchActive = true;
            Time.timeScale = 1f;

            foreach (var p in FindObjectsByType<Player>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (p.ActiveVehicle != null)
                {
                    NetworkServer.UnSpawn(p.ActiveVehicle.gameObject);
                    Destroy(p.ActiveVehicle.gameObject);

                    p.SetActiveVehicle(null);
                }
            }

            foreach (var p in FindObjectsByType<Player>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                p.SvSpawnClientVehicle();
            }

            foreach (var c in matchConditions)
            {
                c.OnServerMatchStart(this);
            }

            RpcMatchStart();
        }

        [Server]
        public void SvEndMatch()
        {
            if (!isMatchActive) return;

            Team winner = Team.None;

            foreach (var c in matchConditions)
            {
                if (winner == Team.None && c.IsTriggered)
                {
                    if (c is ConditionTeamDeathmatch deathmatch)
                    {
                        winner = deathmatch.VictoriousTeam;
                    }
                    else if (c is ConditionTeamBaseCapture capture)
                    {
                        winner = TeamUtils.GetOpponent(capture.Team);
                    }
                }

                c.OnServerMatchEnd(this);
            }

            victoriousTeam = winner;

            isMatchActive = false;

            RpcMatchEnd(victoriousTeam);

            Time.timeScale = 0f;
        }

        [ClientRpc]
        private void RpcMatchStart()
        {
            Time.timeScale = 1f;

            MatchStart?.Invoke();
        }

        [ClientRpc]
        private void RpcMatchEnd(Team winner)
        {
            Player.Local?.HandleMatchEnded();
            MatchEnd?.Invoke(winner);

            Time.timeScale = 0f;
        }
    }
}
