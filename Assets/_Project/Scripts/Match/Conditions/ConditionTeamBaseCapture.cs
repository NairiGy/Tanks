using Tanks.Core;
using UnityEngine;

namespace Tanks.Match
{
    [RequireComponent(typeof(BoxCollider))]
    public class ConditionTeamBaseCapture : MonoBehaviour, IMatchCondition
    {
        [SerializeField] private Team team;
        [SerializeField] private TeamBase teamBase;
        public Team Team => team;
        [SerializeField] private float timeToCapture;
        private float captureTimer;

        public bool IsTriggered => isBaseCaptured;

        private bool isCapturing;
        private int numberOfPlayers = 0;
        private bool isBaseCaptured;

        public float CapturePercentage()
        {
            if (!isCapturing) return 0;

            return captureTimer / timeToCapture;
        }

        private void Start()
        {
            teamBase.EnemyEntered += HandleEnemyEnter;
            teamBase.EnemyExited += HandleEnemyExit;
        }

        private void HandleEnemyEnter()
        {
            numberOfPlayers++;

            isCapturing = true;
        }

        private void HandleEnemyExit()
        {
            numberOfPlayers--;

            if (numberOfPlayers == 0)
            {
                isCapturing = false;
            }
        }

        public void OnServerMatchStart(MatchController controller)
        {
            captureTimer = 0;
            numberOfPlayers = 0;
            isBaseCaptured  = false;
            isCapturing = false;
        }

        public void OnServerMatchEnd(MatchController controller)
        {

        }

        private void Update()
        {
            if (!isCapturing && captureTimer < 0)
            {
                captureTimer = 0;
            }

            if (!isCapturing && captureTimer > 0)
            {
                captureTimer -= Time.deltaTime;
            }

            if (isCapturing && captureTimer < timeToCapture)
            {
                captureTimer += Time.deltaTime;
            }

            if (isCapturing && captureTimer >= timeToCapture)
            {
                isBaseCaptured = true;
            }
        }
    }
}
