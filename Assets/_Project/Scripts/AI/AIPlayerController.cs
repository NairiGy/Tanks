using Tanks.Core;
using Tanks.Game;
using UnityEngine;

namespace Tanks.AI
{
    // Drives a bot's vehicle on the server; wires together the behavior, combat, navigation and unstuck systems.
    [RequireComponent(typeof(AIPlayer))]
    public class AIPlayerController : MonoBehaviour
    {
        [SerializeField] private AIProfile profile;
        [SerializeField] private bool randomizeBehavior = true;
        [SerializeField] private AIPlayerBehaviorType behaviorType;

        private AIPlayer aiPlayer;
        private Vehicle currentVehicle;

        private AIBehavior behavior;
        private AINavigator navigator;
        private AICombat combat;
        private AIStuckDetector stuckDetector;

        public AIPlayerBehaviorType BehaviorType => behaviorType;
        public bool HasBehavior => behavior != null;

        private void Awake()
        {
            aiPlayer = GetComponent<AIPlayer>();

            if (profile == null)
            {
                Debug.LogError("[AI] AIPlayerController has no AIProfile assigned.", this);
                enabled = false;
                return;
            }

            navigator = new AINavigator(profile.Navigation);
            combat = new AICombat(profile.Combat);
            stuckDetector = new AIStuckDetector(profile.Unstuck);
        }

        private void Start()
        {
            if (!aiPlayer.isServer) return;

            if (randomizeBehavior)
            {
                behaviorType = ChooseBehavior();
            }

            behavior = CreateBehavior(behaviorType, AIMapInfo.Get());

            aiPlayer.SvSetNickname($"Bot_{behaviorType}_{aiPlayer.netId}");
        }

        private void Update()
        {
            if (!aiPlayer.isServer) return;
            if (behavior == null) return;
            if (aiPlayer.ActiveVehicle == null) return;
            if (!NetworkSessionManager.Match.IsMatchActive) return;

            EnsureVehicle();

            bool engaged = combat.TryEngage(currentVehicle, aiPlayer.TeamId);
            bool isDriving = false;

            if (stuckDetector.IsRecovering)
            {
                stuckDetector.ApplyRecoveryInput(currentVehicle);
            }
            else if (engaged && behavior.HoldsPositionToFire)
            {
                currentVehicle.SetInputControl(Vector3.zero);
            }
            else
            {
                Vector3 destination = behavior.GetDestination(currentVehicle, out float stopDistance);
                navigator.DriveTowards(currentVehicle, destination, stopDistance);
                isDriving = navigator.IsDrivingForward;

                if (!navigator.HasPath)
                {
                    behavior.OnNavigationFailed();
                }
            }

            if (stuckDetector.Update(currentVehicle, isDriving))
            {
                navigator.InvalidatePath();
            }
        }

        private void EnsureVehicle()
        {
            if (currentVehicle == aiPlayer.ActiveVehicle) return;

            currentVehicle = aiPlayer.ActiveVehicle;

            navigator.InvalidatePath();
            stuckDetector.Reset();
            behavior.OnVehicleChanged();
        }

        private AIPlayerBehaviorType ChooseBehavior()
        {
            bool teamHasCapturer = false;
            bool teamHasDefender = false;

            foreach (AIPlayerController other in FindObjectsByType<AIPlayerController>(FindObjectsSortMode.None))
            {
                if (other == this || !other.HasBehavior || other.aiPlayer.TeamId != aiPlayer.TeamId) continue;

                if (other.BehaviorType == AIPlayerBehaviorType.Capturer) teamHasCapturer = true;
                else if (other.BehaviorType == AIPlayerBehaviorType.Defender) teamHasDefender = true;
            }

            return AIBehaviorSelector.Choose(teamHasCapturer, teamHasDefender);
        }

        private AIBehavior CreateBehavior(AIPlayerBehaviorType type, AIMapInfo map)
        {
            switch (type)
            {
                case AIPlayerBehaviorType.Capturer:
                    return new CapturerBehavior(profile, map, aiPlayer.TeamId);
                case AIPlayerBehaviorType.Defender:
                    return new DefenderBehavior(profile, map, aiPlayer.TeamId);
                default:
                    return new DefaultBehavior(profile, map, aiPlayer.TeamId);
            }
        }
    }
}
