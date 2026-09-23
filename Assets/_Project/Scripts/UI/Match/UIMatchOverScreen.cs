using Tanks.Core;
using Tanks.Game;
using Tanks.Match;
using UnityEngine;

namespace Tanks.UI
{
    public class UIMatchOverScreen : MonoBehaviour
    {
        [SerializeField] private GameObject victoryScreen;
        [SerializeField] private GameObject loseScreen;
        private bool isSubscribed;
        private MatchController currentMatch;

        private void Start()
        {
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

            Unsubscribe();
        }
    }
}
