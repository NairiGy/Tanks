using Tanks.Core;
using UnityEngine;

namespace Tanks.UI
{
    public class UIHitResult : MonoBehaviour
    {
        [SerializeField] private UIHitResultMessage messagePrefab;
        void Start()
        {
            CombatEvents.Instance.PlayerHitPlayerEvent += ShowHitMessage;
        }

        private void ShowHitMessage(Player arg1, Player arg2, ProjectileHitResult arg3)
        {
            if (Player.Local != arg1)  return;
            
            var message = Instantiate(messagePrefab, transform);
            message.SetText(arg3.HitType.ToString());
        }
    }
}
