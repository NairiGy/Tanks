using System;

namespace Tanks.Core
{
    public enum Team : byte
    {
        None = 0,
        Blue = 1,
        Red = 2
    }

    public static class TeamUtils
    {
        public static Team GetOpponent(Team team)
        {
            return team switch
            {
                Team.Blue => Team.Red,
                Team.Red => Team.Blue,
                _ => throw new ArgumentOutOfRangeException(nameof(team), team, "Unknown team")
            };
        }
    }
}
