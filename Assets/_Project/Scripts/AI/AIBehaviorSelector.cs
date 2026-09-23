using System;
using Random = UnityEngine.Random;

namespace Tanks.AI
{
    public static class AIBehaviorSelector
    {
        // Every team gets a Capturer and a Defender first; after that the choice is random.
        public static AIPlayerBehaviorType Choose(bool teamHasCapturer, bool teamHasDefender, Func<int, int> nextIndex = null)
        {
            if (!teamHasCapturer) return AIPlayerBehaviorType.Capturer;
            if (!teamHasDefender) return AIPlayerBehaviorType.Defender;

            var behaviors = (AIPlayerBehaviorType[])Enum.GetValues(typeof(AIPlayerBehaviorType));
            int index = nextIndex != null ? nextIndex(behaviors.Length) : Random.Range(0, behaviors.Length);

            return behaviors[index];
        }
    }
}
