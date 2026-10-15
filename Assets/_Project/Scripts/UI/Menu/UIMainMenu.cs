using Mirror;
using Tanks.Core;
using Tanks.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tanks.UI
{
    /// <summary>Start screen: nickname, host a match, join one by address, or quit.</summary>
    public class UIMainMenu : MonoBehaviour
    {
        private const string NicknameKey = "Nickname";
        private const string AddressKey = "ServerAddress";

        [SerializeField] private GameObject root;
        [Tooltip("In-game HUD, hidden while the menu is open. Only rendering is turned off, so its scripts keep running.")]
        [SerializeField] private Canvas hudCanvas;
        [SerializeField] private TMP_InputField nicknameInput;
        [SerializeField] private TMP_InputField addressInput;
        [SerializeField] private Button hostButton;
        [SerializeField] private Button joinButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private TMP_Text statusText;

        private void Start()
        {
            nicknameInput.text = PlayerPrefs.GetString(NicknameKey, "Player");
            addressInput.text = PlayerPrefs.GetString(AddressKey, NetworkSessionManager.Instance.networkAddress);

            hostButton.onClick.AddListener(OnHost);
            joinButton.onClick.AddListener(OnJoin);
            quitButton.onClick.AddListener(Application.Quit);
        }

        private void Update()
        {
            bool inSession = NetworkServer.active || NetworkClient.isConnected;
            root.SetActive(!inSession);
            hudCanvas.enabled = inSession;

            if (inSession) return;

            bool connecting = NetworkClient.active;
            hostButton.interactable = !connecting;
            joinButton.interactable = !connecting;
            statusText.text = connecting ? $"Connecting to {NetworkSessionManager.Instance.networkAddress}..." : string.Empty;
        }

        private void OnHost()
        {
            ApplyNickname();
            NetworkSessionManager.Instance.StartHost();
        }

        private void OnJoin()
        {
            ApplyNickname();

            string address = addressInput.text.Trim();
            if (string.IsNullOrEmpty(address)) address = "localhost";

            PlayerPrefs.SetString(AddressKey, address);
            NetworkSessionManager.Instance.networkAddress = address;
            NetworkSessionManager.Instance.StartClient();
        }

        private void ApplyNickname()
        {
            string nickname = nicknameInput.text.Trim();
            if (string.IsNullOrEmpty(nickname)) nickname = "Player";

            PlayerPrefs.SetString(NicknameKey, nickname);
            Player.LocalNickname = nickname;
        }
    }
}
