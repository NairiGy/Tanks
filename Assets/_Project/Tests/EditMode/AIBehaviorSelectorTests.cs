using NUnit.Framework;
using Tanks.AI;

namespace Tanks.Tests
{
    public class AIBehaviorSelectorTests
    {
        [TestCase(false, false)]
        [TestCase(false, true)]
        public void ATeamWithoutACapturer_GetsOneFirst(bool hasCapturer, bool hasDefender)
        {
            Assert.AreEqual(AIPlayerBehaviorType.Capturer, AIBehaviorSelector.Choose(hasCapturer, hasDefender));
        }

        [Test]
        public void ATeamWithACapturerButNoDefender_GetsADefender()
        {
            Assert.AreEqual(AIPlayerBehaviorType.Defender, AIBehaviorSelector.Choose(true, false));
        }

        [Test]
        public void OnceBothRolesAreCovered_TheChoiceComesFromTheRandomSource()
        {
            Assert.AreEqual(AIPlayerBehaviorType.Default, AIBehaviorSelector.Choose(true, true, count => 0));
            Assert.AreEqual(AIPlayerBehaviorType.Capturer, AIBehaviorSelector.Choose(true, true, count => 1));
            Assert.AreEqual(AIPlayerBehaviorType.Defender, AIBehaviorSelector.Choose(true, true, count => count - 1));
        }

        [Test]
        public void TheRandomSourceIsAskedForAnIndexAmongAllBehaviors()
        {
            int requested = 0;

            AIBehaviorSelector.Choose(true, true, count =>
            {
                requested = count;
                return 0;
            });

            Assert.AreEqual(System.Enum.GetValues(typeof(AIPlayerBehaviorType)).Length, requested);
        }
    }
}
