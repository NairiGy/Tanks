using Mirror;
using Tanks.Core;
using Tanks.Match;
using UnityEngine;

namespace Tanks.Game
{
    public class NetworkSessionManager : NetworkManager, ISpawnPointProvider
    {
        [SerializeField] private SphereArea[] spawnZonesRed;
        [SerializeField] private SphereArea[] spawnZonesBlue;

        public (Vector3 position, Quaternion rotation) RandomSpawnPointRed => spawnZonesRed[Random.Range(0, spawnZonesRed.Length)].RandomInsidePose;
        public (Vector3 position, Quaternion rotation) RandomSpawnPointBlue => spawnZonesBlue[Random.Range(0, spawnZonesBlue.Length)].RandomInsidePose;
        public static NetworkSessionManager Instance => singleton as NetworkSessionManager;

        [SerializeField] private MatchController matchController;
        public static MatchController Match => Instance.matchController;

        public bool IsServer => mode is NetworkManagerMode.Host or NetworkManagerMode.ServerOnly;
        public bool IsClient => mode is NetworkManagerMode.Host or NetworkManagerMode.ClientOnly;

        [Header("Spawning")]
        [SerializeField] private float spawnClearance = 8f;
        [SerializeField] private int spawnAttempts = 12;

        [Header("Bots")]
        [SerializeField] private GameObject aiPlayerPrefab;
        [SerializeField] private int botCount;

        public override void OnStartServer()
        {
            base.OnStartServer();
            Player.TeamIdCounter = 0;

            SpawnBots();
        }

        private void SpawnBots()
        {
            if (aiPlayerPrefab == null) return;

            for (int i = 0; i < botCount; i++)
            {
                GameObject bot = Instantiate(aiPlayerPrefab);
                NetworkServer.Spawn(bot);
            }
        }

        public (Vector3 position, Quaternion rotation) GetSpawnPoint(Team team)
        {
            Vehicle[] vehicles = FindObjectsByType<Vehicle>(FindObjectsSortMode.None);

            (Vector3 position, Quaternion rotation) best = default;
            float bestClearance = -1f;

            for (int i = 0; i < spawnAttempts; i++)
            {
                var candidate = team == Team.Red ? RandomSpawnPointRed : RandomSpawnPointBlue;

                float nearest = float.MaxValue;

                foreach (Vehicle vehicle in vehicles)
                {
                    Vector3 flat = vehicle.transform.position - candidate.position;
                    flat.y = 0f;
                    nearest = Mathf.Min(nearest, flat.magnitude);
                }

                if (nearest >= spawnClearance) return candidate;

                if (nearest > bestClearance)
                {
                    bestClearance = nearest;
                    best = candidate;
                }
            }

            return best;
        }
    }
}
