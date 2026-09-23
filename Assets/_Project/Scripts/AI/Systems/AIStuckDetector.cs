using System;
using Tanks.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tanks.AI
{
    // Notices when a vehicle that's trying to drive makes no progress and does a reverse-then-turn to get free.
    public sealed class AIStuckDetector
    {
        private readonly UnstuckSettings settings;
        private readonly Func<float> random01;

        private Vector3 samplePosition;
        private float sampleYaw;
        private float nextSampleTime;
        private float reverseUntil;
        private float turnUntil;
        private float turnDirection = 1f;

        public bool IsRecovering => IsRecoveringAt(Time.time);

        public AIStuckDetector(UnstuckSettings settings, Func<float> random01 = null)
        {
            this.settings = settings;
            this.random01 = random01 ?? new Func<float>(() => Random.value);
        }

        public bool IsRecoveringAt(float time)
        {
            return time < turnUntil;
        }

        public void Reset()
        {
            reverseUntil = 0f;
            turnUntil = 0f;
        }

        public void ApplyRecoveryInput(Vehicle vehicle)
        {
            vehicle.SetInputControl(GetRecoveryInput(Time.time));
        }

        // Reverses first, then turns away.
        public Vector3 GetRecoveryInput(float time)
        {
            return time < reverseUntil
                ? new Vector3(0f, 0f, -1f)
                : new Vector3(turnDirection, 0f, 0f);
        }

        public bool Update(Vehicle vehicle, bool isDrivingForward)
        {
            return Update(vehicle.transform.position, vehicle.transform.eulerAngles.y, Time.time, isDrivingForward);
        }

        // Returns true on the frame a recovery manoeuvre begins.
        public bool Update(Vector3 position, float yaw, float time, bool isDrivingForward)
        {
            if (!isDrivingForward)
            {
                samplePosition = position;
                sampleYaw = yaw;
                nextSampleTime = time + settings.CheckInterval;
                return false;
            }

            if (time < nextSampleTime) return false;

            Vector3 moved = position - samplePosition;
            moved.y = 0f;
            float turned = Mathf.Abs(Mathf.DeltaAngle(sampleYaw, yaw));

            samplePosition = position;
            sampleYaw = yaw;
            nextSampleTime = time + settings.CheckInterval;

            if (moved.magnitude >= settings.MinDistance || turned >= settings.MinTurnAngle) return false;

            reverseUntil = time + settings.ReverseTime;
            turnUntil = reverseUntil + settings.TurnTime;
            turnDirection = random01() < 0.5f ? -1f : 1f;

            return true;
        }
    }
}
