using Tanks.Core;
using UnityEngine;

namespace Tanks.UI
{
    public class UIPlayerList : MonoBehaviour
    {
        [SerializeField] private UIPlayerListItem playerListItemPrefab;
        private PlayerList playerList;
        private bool isSubscribed;

        private void Update()
        {
            if (isSubscribed) return;

            if (!PlayerList.Instance) return;

            playerList = PlayerList.Instance;

            Subscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            if (isSubscribed || playerList == null) return;

            playerList.UserListChanged += UpdateList;
            isSubscribed = true;

            UpdateList();
        }

        private void Unsubscribe()
        {
            playerList.UserListChanged -= UpdateList;
            isSubscribed = false;
            playerList = null;
        }

        private void UpdateList()
        {
            ClearList();

            var userList = playerList.allPlayers;

            foreach (var user in userList)
            {
                if (user == null) continue; // guard against stale refs, see note below
                var player = Instantiate(playerListItemPrefab, transform);
                player.Init(user);
            }
        }

        private void ClearList()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
