using Tanks.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Tanks.AI
{
    // Must be constructed from Awake or later - NavMeshPath can't be created in a field initializer.
    public sealed class AINavigator
    {
        private readonly NavigationSettings settings;
        private readonly NavMeshPath path = new NavMeshPath();

        private float nextRepathTime;
        private Vector3 pathDestination;
        private bool loggedNoNavMesh;

        public bool HasPath { get; private set; }
        public bool IsDrivingForward { get; private set; }

        public AINavigator(NavigationSettings settings)
        {
            this.settings = settings;
        }

        public void InvalidatePath()
        {
            HasPath = false;
        }

        public void DriveTowards(Vehicle vehicle, Vector3 worldTarget, float stopDistance)
        {
            IsDrivingForward = false;

            Vector3 vehiclePosition = vehicle.transform.position;

            Vector3 flatToDestination = worldTarget - vehiclePosition;
            flatToDestination.y = 0f;

            if (flatToDestination.magnitude <= stopDistance)
            {
                vehicle.SetInputControl(Vector3.zero);
                return;
            }

            bool destinationChanged = Vector3.Distance(pathDestination, worldTarget) > 1f;

            if (!HasPath || destinationChanged || Time.time >= nextRepathTime)
            {
                HasPath = RefreshPath(vehiclePosition, worldTarget);
            }

            if (!HasPath)
            {
                vehicle.SetInputControl(Vector3.zero);
                return;
            }

            Vector3 toSteerTarget = GetSteerTarget(vehiclePosition) - vehiclePosition;
            toSteerTarget.y = 0f;

            if (toSteerTarget.sqrMagnitude < 0.01f)
            {
                vehicle.SetInputControl(Vector3.zero);
                return;
            }

            float angle = Vector3.SignedAngle(vehicle.transform.forward, toSteerTarget.normalized, Vector3.up);
            float steering = AISteering.Compute(angle, settings.SteerDeadZoneAngle, settings.SteerAngleRange);

            IsDrivingForward = true;
            vehicle.SetInputControl(new Vector3(steering, 0f, 1f));
        }

        private bool RefreshPath(Vector3 from, Vector3 to)
        {
            float sampleDistance = settings.NavMeshSampleDistance;

            if (!NavMesh.SamplePosition(from, out NavMeshHit start, sampleDistance, NavMesh.AllAreas) ||
                !NavMesh.SamplePosition(to, out NavMeshHit end, sampleDistance, NavMesh.AllAreas))
            {
                if (!loggedNoNavMesh)
                {
                    loggedNoNavMesh = true;
                    Debug.LogWarning($"[AI] No NavMesh within {sampleDistance}m of {from} or of destination {to}. Is the NavMesh baked there?");
                }

                return false;
            }

            loggedNoNavMesh = false;

            if (!NavMesh.CalculatePath(start.position, end.position, NavMesh.AllAreas, path) || path.corners.Length < 2)
            {
                return false;
            }

            pathDestination = to;
            nextRepathTime = Time.time + settings.RepathInterval;

            return true;
        }

        private Vector3 GetSteerTarget(Vector3 vehiclePosition)
        {
            Vector3[] corners = path.corners;

            for (int i = 1; i < corners.Length; i++)
            {
                Vector3 flat = corners[i] - vehiclePosition;
                flat.y = 0f;

                if (flat.magnitude > settings.CornerReachDistance) return corners[i];
            }

            return corners[corners.Length - 1];
        }
    }
}
