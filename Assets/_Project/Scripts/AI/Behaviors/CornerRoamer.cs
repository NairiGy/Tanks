using System.Collections.Generic;
using Tanks.Core;
using UnityEngine;

namespace Tanks.AI
{
    // Drives through a few random map corners before letting the bot head for its real objective.
    public sealed class CornerRoamer
    {
        private readonly CornerRoamingSettings settings;
        private readonly AIMapInfo map;

        private IReadOnlyList<Vector3> corners;
        private int legsRemaining;
        private int currentIndex = -1;
        private int lastIndex = -1;
        private float legDeadline;

        public CornerRoamer(CornerRoamingSettings settings, AIMapInfo map)
        {
            this.settings = settings;
            this.map = map;
        }

        public void Reset()
        {
            legsRemaining = Random.Range(settings.MinLegs, settings.MaxLegs + 1);
            currentIndex = -1;
            lastIndex = -1;
        }

        public void OnNavigationFailed()
        {
            if (currentIndex >= 0)
            {
                FinishLeg();
            }
        }

        public bool TryGetDestination(Vehicle vehicle, out Vector3 destination)
        {
            destination = default;

            if (legsRemaining <= 0) return false;

            corners ??= map.GetCornerPoints(settings.MapInset, settings.SampleDistance);

            if (corners.Count == 0)
            {
                legsRemaining = 0;
                return false;
            }

            if (currentIndex < 0)
            {
                currentIndex = PickCornerIndex();
                legDeadline = Time.time + settings.LegTimeout;
            }

            Vector3 corner = corners[currentIndex];

            Vector3 flat = corner - vehicle.transform.position;
            flat.y = 0f;

            if (flat.magnitude <= settings.ArriveDistance || Time.time >= legDeadline)
            {
                FinishLeg();
                return TryGetDestination(vehicle, out destination);
            }

            destination = corner;
            return true;
        }

        private void FinishLeg()
        {
            lastIndex = currentIndex;
            currentIndex = -1;
            legsRemaining--;
        }

        private int PickCornerIndex()
        {
            if (corners.Count == 1) return 0;

            int index;

            do
            {
                index = Random.Range(0, corners.Count);
            }
            while (index == lastIndex);

            return index;
        }
    }
}
