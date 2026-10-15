using Mirror;
using Tanks.Core;
using Tanks.Game;
using Tanks.Match;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tanks.UI
{
    public class UIMatchOverScreen : MonoBehaviour
    {
        [SerializeField] private GameObject victoryScreen;
        [SerializeField] private GameObject loseScreen;

        [Header("Actions")]
        [SerializeField] private GameObject actions;
        [SerializeField] private Button playAgainButton;
        [SerializeField] private Button mainMenuButton;
        [Tooltip("Shown to clients instead of Play Again: only the host can restart the match.")]
        [SerializeField] private TMP_Text waitingForHostText;

        private bool isSubscribed;
        private MatchController currentMatch;

        private void Start()
        {
            playAgainButton.onClick.AddListener(OnPlayAgain);
            mainMenuButton.onClick.AddListener(OnMainMenu);

            ResetUI();
        }

        private void Update()
        {
            if (isSubscribed) return;

            if (NetworkSessionManager.Instance && NetworkSessionManager.Match && NetworkSessionManager.Match.IsMatchActive)
            {
                Subscribe();
            } 
        }

        private void Subscribe()
        {
            isSubscribed = true;
            currentMatch = NetworkSessionManager.Match;

            NetworkSessionManager.Match.MatchEnd += OnMatchEnd;
            NetworkSessionManager.Match.MatchStart += OnMatchStart;

            // After a restart the MatchStart RPC can arrive before we resubscribe, so clear the previous result here.
            ResetUI();
        }

        private void Unsubscribe()
        {
            isSubscribed = false;

            currentMatch.MatchEnd -= OnMatchEnd;
            currentMatch.MatchStart -= OnMatchStart;

            currentMatch = null;
        }

        private void OnMatchStart()
        {
            ResetUI();
        }

        private void ResetUI()
        {
            victoryScreen.SetActive(false);
            loseScreen.SetActive(false);
            actions.SetActive(false);
        }

        private void OnMatchEnd(Team winner)
        {
            if (Player.Local.TeamId == winner)
            {
                victoryScreen.SetActive(true);
            }
            else
            {
                loseScreen.SetActive(true);
            }

            bool isHost = NetworkServer.active;
            actions.SetActive(true);
            playAgainButton.gameObject.SetActive(isHost);
            waitingForHostText.gameObject.SetActive(!isHost);

            Unsubscribe();
        }

        private void OnPlayAgain()
        {
            if (NetworkServer.active)
            {
                NetworkSessionManager.Match.SvRestartMatch();
            }
        }

        private void OnMainMenu()
        {
            NetworkSessionManager.Instance.LeaveSession();
        }
    }
}
