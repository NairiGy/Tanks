using System.IO;
using Tanks.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tanks.UI
{
    public class UIPlayerListItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text nickname;
        [SerializeField] private TMP_Text numberOfKills;
        [SerializeField] private TMP_Text hp;
        [SerializeField] private Image image;

        [SerializeField] private Color ownColor;
        [SerializeField] private Color teammateColor;
        [SerializeField] private Color enemyColor;

        private Player player;
        private Vehicle vehicle;

        public void Init(Player player)
        {
            this.player = player;

            player.KillScored += HandleKillScored;
            player.NicknameChanged += HandleNicknameChanged;
            player.ActiveVehicleChanged += HandleActiveVehicleChanged;

            UpdateUI();
            SetColor();

            HandleActiveVehicleChanged(player.ActiveVehicle); // covers case where vehicle already exists at Init time
        }

        private void OnDestroy()
        {
            if (player != null)
            {
                player.KillScored -= HandleKillScored;
                player.NicknameChanged -= HandleNicknameChanged;
                player.ActiveVehicleChanged -= HandleActiveVehicleChanged;
            }
            if (vehicle != null)
                vehicle.HitPointChange -= HandleHitpointChanged;
        }

        private void UpdateUI()
        {
            nickname.text = player.Nickname;
            numberOfKills.text = player.NumberOfKills.ToString();
        }

        private void SetColor()
        {
            if (Player.Local == null) return;

            if (player.netId == Player.Local.netId)
            {
                image.color = ownColor;
            }
            else if (player.TeamId == Player.Local.TeamId)
            {
                image.color = teammateColor;
            } else if (player.TeamId != Player.Local.TeamId)
            {
                image.color = enemyColor;
            }
            else
            {
                throw new InvalidDataException();
            }

        }

        private void HandleActiveVehicleChanged(Vehicle newVehicle)
        {
            if (vehicle != null)
                vehicle.HitPointChange -= HandleHitpointChanged;

            vehicle = newVehicle;

            if (vehicle != null)
            {
                vehicle.HitPointChange += HandleHitpointChanged;
                HandleHitpointChanged(0);
            }
            else
            {
                hp.text = "";
            }
        }

        private void HandleHitpointChanged(int obj)
        {
            if (player.ActiveVehicle != null)
            {
                hp.text = $"{player.ActiveVehicle.HitPointsPercentage()} %";
            }
        }

        private void HandleKillScored(int newNumberOfKills)
        {
            numberOfKills.text = newNumberOfKills.ToString();
        }

        private void HandleNicknameChanged(string newNickname)
        {
            nickname.text = newNickname;
        }
    }
}
