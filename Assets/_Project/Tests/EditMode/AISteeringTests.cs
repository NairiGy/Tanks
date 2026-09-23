using NUnit.Framework;
using Tanks.AI;

namespace Tanks.Tests
{
    public class AISteeringTests
    {
        private const float DeadZone = 10f;
        private const float Range = 45f;
        private const float Tolerance = 0.0001f;

        [TestCase(0f)]
        [TestCase(5f)]
        [TestCase(-9.9f)]
        public void SmallHeadingErrors_DriveStraight(float angle)
        {
            Assert.AreEqual(0f, AISteering.Compute(angle, DeadZone, Range), Tolerance);
        }

        [Test]
        public void SteeringIsProportionalToTheError()
        {
            Assert.AreEqual(0.5f, AISteering.Compute(22.5f, DeadZone, Range), Tolerance);
            Assert.AreEqual(-0.5f, AISteering.Compute(-22.5f, DeadZone, Range), Tolerance);
        }

        [TestCase(45f, 1f)]
        [TestCase(120f, 1f)]
        [TestCase(-45f, -1f)]
        [TestCase(-180f, -1f)]
        public void LargeErrors_SaturateAtFullLock(float angle, float expected)
        {
            Assert.AreEqual(expected, AISteering.Compute(angle, DeadZone, Range), Tolerance);
        }

        [Test]
        public void AnErrorExactlyAtTheDeadZoneEdge_StartsSteering()
        {
            Assert.AreEqual(DeadZone / Range, AISteering.Compute(DeadZone, DeadZone, Range), Tolerance);
        }
    }
}
