using System;
using NUnit.Framework;
using Tanks.Core;

namespace Tanks.Tests
{
    public class TeamUtilsTests
    {
        [TestCase(Team.Blue, Team.Red)]
        [TestCase(Team.Red, Team.Blue)]
        public void GetOpponent_ReturnsTheOtherTeam(Team team, Team expected)
        {
            Assert.AreEqual(expected, TeamUtils.GetOpponent(team));
        }

        [Test]
        public void GetOpponent_ThrowsForNone()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => TeamUtils.GetOpponent(Team.None));
        }
    }
}
