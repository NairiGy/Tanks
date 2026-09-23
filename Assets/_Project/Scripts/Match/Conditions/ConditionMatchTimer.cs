using Mirror;
using UnityEngine;

namespace Tanks.Match
{
    public class ConditionMatchTimer : NetworkBehaviour, IMatchCondition
    {
        [SerializeField] private float matchTime;

        [SyncVar] private float timeLeft;
        public float TimeLeft => timeLeft;

        private bool isTimerEnded;
        public bool IsTriggered => isTimerEnded;

        private void Start()
        {
            if (isServer)
            {
                enabled = false;
            }
        }

        private void Update()
        {
            if (isServer)
            {
                timeLeft -= Time.deltaTime;

                if (timeLeft <= 0)
                {
                    timeLeft = 0;

                    isTimerEnded = true;
                }
            }
        }

        public void OnServerMatchStart(MatchController controller)
        {
            Reset();
        }

        private void Reset()
        {
            enabled = true;
            timeLeft = matchTime;
            isTimerEnded = false;
        }

        public void OnServerMatchEnd(MatchController controller)
        {
            enabled = false;
        }
    }
}
