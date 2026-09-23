using System;
using Mirror;

namespace Tanks.Core
{
    public class PlayerList : NetworkBehaviour
    {
        public static PlayerList Instance { get; private set; }

        public event Action UserListChanged;

        public readonly SyncList<Player> allPlayers = new SyncList<Player>();

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

        public override void OnStartClient()
        {
            allPlayers.Callback += OnPlayerListChanged;
        }

        public override void OnStopClient()
        {
            allPlayers.Callback -= OnPlayerListChanged;
        }

        private void OnPlayerListChanged(SyncList<Player>.Operation operation, int index, Player oldPlayer, Player newPlayer)
        {
            UserListChanged?.Invoke();
        }

        [Server]
        public void SvAddCurrentUser(Player player)
        {
            if (allPlayers.Contains(player)) return;

            allPlayers.Add(player);
        }

        [Server]
        public void SvRemoveCurrentUser(Player player)
        {
            allPlayers.Remove(player);
        }
    }
}
