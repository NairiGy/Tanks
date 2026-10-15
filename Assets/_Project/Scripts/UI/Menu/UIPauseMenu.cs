using Mirror;
using Tanks.Core;
using Tanks.Game;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Tanks.UI
{
    /// <summary>In-match menu on Esc. The match keeps running for everyone else; only local input is blocked.</summary>
    public class UIPauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button leaveButton;

        private void Start()
        {
            resumeButton.onClick.AddListener(() => SetOpen(false));
            leaveButton.onClick.AddListener(() => NetworkSessionManager.Instance.LeaveSession());

            SetOpen(false);
        }

        private void Update()
        {
            if (!NetworkClient.isConnected)
            {
                if (root.activeSelf) SetOpen(false);
                return;
            }

            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                SetOpen(!root.activeSelf);
            }
        }

        private void OnDestroy()
        {
            VehicleInputControl.IsBlocked = false;
        }

        private void SetOpen(bool open)
        {
            root.SetActive(open);
            VehicleInputControl.IsBlocked = open;
        }
    }
}
