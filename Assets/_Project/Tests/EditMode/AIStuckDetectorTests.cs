using NUnit.Framework;
using Tanks.AI;
using UnityEngine;

namespace Tanks.Tests
{
    // Uses the default UnstuckSettings: check every 2.5 s, stuck below 1 m / 15 degrees of progress,
    // reverse for 1.5 s, then turn for 1 s.
    public class AIStuckDetectorTests
    {
        private AIStuckDetector detector;

        [SetUp]
        public void SetUp()
        {
            // 0.25 (< 0.5) makes the detector pick a turn direction of -1.
            detector = new AIStuckDetector(new UnstuckSettings(), () => 0.25f);
            detector.Update(Vector3.zero, 0f, 0f, false);
        }

        [Test]
        public void ABotThatIsNotTryingToDrive_IsNeverStuck()
        {
            for (float time = 0f; time < 20f; time += 0.5f)
            {
                Assert.IsFalse(detector.Update(Vector3.zero, 0f, time, false));
            }
        }

        [Test]
        public void ABotThatKeepsMoving_IsNotStuck()
        {
            Assert.IsFalse(detector.Update(new Vector3(0f, 0f, 5f), 0f, 2.5f, true));
        }

        [Test]
        public void ABotThatIsPivotingInPlace_IsNotStuck()
        {
            Assert.IsFalse(detector.Update(Vector3.zero, 30f, 2.5f, true));
        }

        [Test]
        public void NothingIsJudgedBeforeTheCheckIntervalHasPassed()
        {
            Assert.IsFalse(detector.Update(Vector3.zero, 0f, 1f, true));
        }

        [Test]
        public void ABotThatDrivesWithoutMoving_IsDetectedAsStuck()
        {
            Assert.IsTrue(detector.Update(Vector3.zero, 0f, 2.5f, true));
        }

        [Test]
        public void RecoveryReversesFirstAndThenTurns()
        {
            detector.Update(Vector3.zero, 0f, 2.5f, true);

            Assert.IsTrue(detector.IsRecoveringAt(3f));
            Assert.AreEqual(new Vector3(0f, 0f, -1f), detector.GetRecoveryInput(3f));
            Assert.AreEqual(new Vector3(-1f, 0f, 0f), detector.GetRecoveryInput(4.5f));
            Assert.IsFalse(detector.IsRecoveringAt(5f));
        }

        [Test]
        public void Reset_CancelsARecoveryInProgress()
        {
            detector.Update(Vector3.zero, 0f, 2.5f, true);
            detector.Reset();

            Assert.IsFalse(detector.IsRecoveringAt(3f));
        }
    }
}
