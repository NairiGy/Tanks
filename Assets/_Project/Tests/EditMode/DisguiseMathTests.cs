using NUnit.Framework;
using Tanks.Core;

namespace Tanks.Tests
{
    public class DisguiseMathTests
    {
        private const float Base = 0.6f;
        private const float MovingPenalty = 0.3f;
        private const float BushBonus = 0.3f;
        private const float FiringPenalty = 0.5f;
        private const float Tolerance = 0.0001f;

        private static float Target(bool moving = false, bool inBush = false, bool fired = false)
        {
            return DisguiseMath.TargetDisguise(Base, moving, MovingPenalty, inBush, BushBonus, fired, FiringPenalty);
        }

        [Test]
        public void StandingStill_UsesTheBaseDisguise()
        {
            Assert.AreEqual(0.6f, Target(), Tolerance);
        }

        [Test]
        public void Moving_LowersDisguise()
        {
            Assert.AreEqual(0.3f, Target(moving: true), Tolerance);
        }

        [Test]
        public void FiringRecently_LowersDisguise()
        {
            Assert.AreEqual(0.1f, Target(fired: true), Tolerance);
        }

        [Test]
        public void SittingInABush_RaisesDisguise()
        {
            Assert.AreEqual(0.9f, Target(inBush: true), Tolerance);
        }

        [Test]
        public void ABushOffsetsMovement()
        {
            Assert.AreEqual(0.6f, Target(moving: true, inBush: true), Tolerance);
        }

        [Test]
        public void ResultNeverGoesBelowZero()
        {
            Assert.AreEqual(0f, Target(moving: true, fired: true), Tolerance);
        }

        [Test]
        public void ResultNeverGoesAboveOne()
        {
            float target = DisguiseMath.TargetDisguise(0.9f, false, MovingPenalty, true, BushBonus, false, FiringPenalty);

            Assert.AreEqual(1f, target, Tolerance);
        }
    }
}
