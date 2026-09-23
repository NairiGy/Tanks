using NUnit.Framework;
using Tanks.Core;

namespace Tanks.Tests
{
    public class SpottingMathTests
    {
        private const float AlwaysVisible = 20f;
        private const float MaxRange = 150f;
        private const float Tolerance = 0.0001f;

        [Test]
        public void FullyExposedTarget_IsSpottedAtMaxRange()
        {
            Assert.AreEqual(MaxRange, SpottingMath.EffectiveDetectionRange(AlwaysVisible, MaxRange, 0f), Tolerance);
        }

        [Test]
        public void FullyDisguisedTarget_IsOnlySpottedAtPointBlank()
        {
            Assert.AreEqual(AlwaysVisible, SpottingMath.EffectiveDetectionRange(AlwaysVisible, MaxRange, 1f), Tolerance);
        }

        [Test]
        public void HalfDisguisedTarget_IsSpottedHalfWayBetween()
        {
            Assert.AreEqual(85f, SpottingMath.EffectiveDetectionRange(AlwaysVisible, MaxRange, 0.5f), Tolerance);
        }

        [Test]
        public void DisguiseOutsideZeroToOne_IsClampedToTheRange()
        {
            Assert.AreEqual(MaxRange, SpottingMath.EffectiveDetectionRange(AlwaysVisible, MaxRange, -1f), Tolerance);
            Assert.AreEqual(AlwaysVisible, SpottingMath.EffectiveDetectionRange(AlwaysVisible, MaxRange, 2f), Tolerance);
        }

        [Test]
        public void HigherDisguise_NeverIncreasesTheRange()
        {
            float previous = float.MaxValue;

            for (float disguise = 0f; disguise <= 1f; disguise += 0.1f)
            {
                float range = SpottingMath.EffectiveDetectionRange(AlwaysVisible, MaxRange, disguise);

                Assert.LessOrEqual(range, previous);
                previous = range;
            }
        }
    }
}
