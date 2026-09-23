using NUnit.Framework;
using Tanks.Core;
using Tanks.Match;

namespace Tanks.Tests
{
    public class DeathmatchTrackerTests
    {
        private DeathmatchTracker tracker;

        [SetUp]
        public void SetUp()
        {
            tracker = new DeathmatchTracker();
        }

        [Test]
        public void WhileBothTeamsHaveTanks_TheMatchIsUndecided()
        {
            Assert.IsNull(tracker.Evaluate(3, 3));
            Assert.IsNull(tracker.Evaluate(1, 2));
        }

        [Test]
        public void ATeamThatNeverSpawned_IsNotConsideredEliminated()
        {
            Assert.IsNull(tracker.Evaluate(0, 2));
        }

        [Test]
        public void WhenRedIsWipedOut_BlueWins()
        {
            tracker.Evaluate(2, 2);

            Assert.AreEqual(Team.Blue, tracker.Evaluate(0, 1));
        }

        [Test]
        public void WhenBlueIsWipedOut_RedWins()
        {
            tracker.Evaluate(2, 2);

            Assert.AreEqual(Team.Red, tracker.Evaluate(1, 0));
        }

        [Test]
        public void WhenBothAreWipedOutTogether_ItIsADraw()
        {
            tracker.Evaluate(1, 1);

            Assert.AreEqual(Team.None, tracker.Evaluate(0, 0));
        }

        [Test]
        public void Reset_ForgetsWhichTeamsHaveBeenSeen()
        {
            tracker.Evaluate(1, 1);
            tracker.Reset();

            Assert.IsNull(tracker.Evaluate(0, 1));
        }
    }
}
