using System;
using Mirror;
using UnityEngine;

namespace Tanks.Core
{
    public class Player : NetworkBehaviour
    {
        private const int MaxNicknameLength = 24;

        public static int TeamIdCounter;

        public static Player Local
        {
            get
            {
                var networkIdentity = NetworkClient.localPlayer;
                return networkIdentity ? networkIdentity.GetComponent<Player>() : null;
            }
        }

        public virtual bool IsBot => false;

        [SerializeField] private Vehicle vehiclePrefab;
        [SerializeField] private VehicleInputControl vehicleInputControl;
        [SyncVar(hook = nameof(OnActiveVehicleChanged))]
        private Vehicle activeVehicle;

        public Vehicle ActiveVehicle => activeVehicle;

        [Header("Player")]
        [SyncVar(hook = nameof(OnNicknameChanged))]
        private string nickname;
        [SyncVar(hook = nameof(OnNumberOfKillsChanged))]
        private int numberOfKills;

        public string Nickname => nickname;
        public int NumberOfKills => numberOfKills;

        public event Action<int> KillScored;
        public event Action<string> NicknameChanged;
        public event Action<Vehicle> ActiveVehicleChanged;

        [SyncVar] [SerializeField] private Team teamId;
        public Team TeamId => teamId;

        public override void OnStartServer()
        {
            base.OnStartServer();
            teamId = TeamIdCounter % 2 == 0 ? Team.Red : Team.Blue;
            TeamIdCounter++;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (!isOwned) return;
            
            CmdSetNickname(NetworkManager.singleton.GetComponent<NetworkManagerHUD>().Nickname);
            CmdSpawnVehicle();
        }

        private void Update()
        {
            if (!isLocalPlayer) return;
            
            if (ActiveVehicle != null)
            {
                ActiveVehicle.SetVisibility(!VehicleCamera.Instance.IsZoomed);
            }
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            if (NetworkServer.active && PlayerList.Instance != null)
            {
                PlayerList.Instance.SvRemoveCurrentUser(this);
            }
        }

        [Server]
        public void SetActiveVehicle(Vehicle vehicle)
        {
            activeVehicle = vehicle;
        }

        private void OnActiveVehicleChanged(Vehicle oldVal, Vehicle newVal)
        {
            ActiveVehicleChanged?.Invoke(ActiveVehicle);

            if (!isOwned) return;
            if (ActiveVehicle != null && VehicleCamera.Instance != null)
                VehicleCamera.Instance.SetTarget(ActiveVehicle);
            vehicleInputControl.enabled = ActiveVehicle != null;
        }

        [Command]
        public void CmdSetNickname(string newNickname)
        {
            SvSetNickname(newNickname);
        }

        [Server]
        public void SvSetNickname(string newNickname)
        {
            newNickname = (newNickname ?? string.Empty).Trim();

            if (newNickname.Length > MaxNicknameLength)
            {
                newNickname = newNickname.Substring(0, MaxNicknameLength);
            }

            nickname = newNickname;
            gameObject.name = "Player_" + nickname;
        }

        private void OnNicknameChanged(string oldNickname, string newNickname)
        {
            gameObject.name = "Player_" + newNickname;
            NicknameChanged?.Invoke(newNickname);
        }

        private void OnNumberOfKillsChanged(int oldNumberOfKills, int newNumberOfKills)
        {
            numberOfKills = newNumberOfKills;

            KillScored?.Invoke(newNumberOfKills);
        }

        public void HandleMatchEnded()
        {
            if (ActiveVehicle == null) return;

            ActiveVehicle.SetInputControl(Vector3.zero);
            vehicleInputControl.enabled = false;
        }

        [Command]
        private void CmdSpawnVehicle()
        {
            SvSpawnClientVehicle();
        }

        [Server]
        public void SvSpawnClientVehicle()
        {
            if (ActiveVehicle != null) return;

            if (!(NetworkManager.singleton is ISpawnPointProvider spawnPoints))
            {
                Debug.LogError("[Player] The NetworkManager does not implement ISpawnPointProvider.", this);
                return;
            }

            (Vector3 position, Quaternion rotation) rnd = spawnPoints.GetSpawnPoint(teamId);

            GameObject go = Instantiate(vehiclePrefab.gameObject, rnd.position, rnd.rotation);

            NetworkServer.Spawn(go, connectionToClient);

            SetActiveVehicle(go.GetComponent<Vehicle>());
            ActiveVehicle.SetOwner(netIdentity);

            PlayerList.Instance.SvAddCurrentUser(this);
        }
    }
}
