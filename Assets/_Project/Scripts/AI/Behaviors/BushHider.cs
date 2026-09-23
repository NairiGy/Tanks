using System.Collections.Generic;
using Tanks.Core;
using UnityEngine;

namespace Tanks.AI
{
    // Picks the nearest bush, drives into it, hides a while, then moves on. Used bushes go on cooldown
    // so the bot hops between bushes instead of looping back to the same one.
    public sealed class BushHider
    {
        private readonly BushHidingSettings settings;
        private readonly IReadOnlyList<Collider> areas;
        private readonly Dictionary<Collider, float> cooldownUntil = new Dictionary<Collider, float>();

        private Collider current;
        private float hideEndTime = -1f;
        private float approachDeadline;
        private float nextSearchTime;

        private bool IsApproaching => hideEndTime < 0f;

        public BushHider(BushHidingSettings settings, IReadOnlyList<Collider> areas)
        {
            this.settings = settings;
            this.areas = areas;
        }

        public void Reset()
        {
            current = null;
        }

        public void OnNavigationFailed()
        {
            if (current != null && IsApproaching)
            {
                Abandon();
            }
        }

        public bool TryGetDestination(Vehicle vehicle, out Vector3 destination)
        {
            destination = default;

            if (current == null)
            {
                if (Time.time < nextSearchTime) return false;

                nextSearchTime = Time.time + settings.SearchInterval;
                current = FindNearby(vehicle.transform.position);

                if (current == null) return false;

                hideEndTime = -1f;
                approachDeadline = Time.time + settings.ApproachTimeout;
            }

            if (IsApproaching)
            {
                if (IsInside(vehicle.transform.position, current))
                {
                    hideEndTime = Time.time + settings.HideDuration;
                }
                else if (Time.time >= approachDeadline)
                {
                    Abandon();
                    return false;
                }
            }
            else if (Time.time >= hideEndTime)
            {
                Abandon();
                return false;
            }

            destination = current.bounds.center;
            return true;
        }

        private Collider FindNearby(Vector3 position)
        {
            Collider nearest = null;
            float nearestSqrDistance = settings.SearchRadius * settings.SearchRadius;

            foreach (Collider area in areas)
            {
                if (area == null) continue;
                if (cooldownUntil.TryGetValue(area, out float until) && Time.time < until) continue;

                Vector3 flat = area.bounds.center - position;
                flat.y = 0f;

                if (flat.sqrMagnitude < nearestSqrDistance)
                {
                    nearestSqrDistance = flat.sqrMagnitude;
                    nearest = area;
                }
            }

            return nearest;
        }

        private static bool IsInside(Vector3 position, Collider area)
        {
            position.y = area.bounds.center.y;

            return (area.ClosestPoint(position) - position).sqrMagnitude < 0.01f;
        }

        private void Abandon()
        {
            cooldownUntil[current] = Time.time + settings.Cooldown;
            current = null;
        }
    }
}
