using Tanks.Core;

namespace Tanks.Match
{
    public sealed class DeathmatchTracker
    {
        private bool redSeen;
        private bool blueSeen;

        public void Reset()
        {
            redSeen = false;
            blueSeen = false;
        }

        // Null while undecided; otherwise the winner, or Team.None if both teams were wiped at once.
        public Team? Evaluate(int redAlive, int blueAlive)
        {
            if (redAlive > 0) redSeen = true;
            if (blueAlive > 0) blueSeen = true;

            bool redEliminated = redSeen && redAlive == 0;
            bool blueEliminated = blueSeen && blueAlive == 0;

            if (!redEliminated && !blueEliminated) return null;
            if (redEliminated && blueEliminated) return Team.None;

            return redEliminated ? Team.Blue : Team.Red;
        }
    }
}
