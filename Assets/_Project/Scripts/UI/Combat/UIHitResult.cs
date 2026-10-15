using Tanks.Core;
using UnityEngine;

namespace Tanks.UI
{
    public class UIHitResult : MonoBehaviour
    {
        [SerializeField] private UIHitResultMessage messagePrefab;

        private CombatEvents subscribedEvents;

        // CombatEvents is a networked scene object: in a build it only appears once a session starts,
        // so subscribe when it shows up instead of in Start.
        private void Update()
        {
            if (subscribedEvents != null || CombatEvents.Instance == null) return;

            subscribedEvents = CombatEvents.Instance;
            subscribedEvents.PlayerHitPlayerEvent += ShowHitMessage;
        }

        private void OnDestroy()
        {
            if (subscribedEvents != null)
            {
                subscribedEvents.PlayerHitPlayerEvent -= ShowHitMessage;
            }
        }

        private void ShowHitMessage(Player arg1, Player arg2, ProjectileHitResult arg3)
        {
            if (Player.Local != arg1)  return;
            
            var message = Instantiate(messagePrefab, transform);
            message.SetText(arg3.HitType.ToString());
        }
    }
}
