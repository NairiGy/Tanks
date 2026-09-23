using UnityEngine;

namespace Tanks.Core
{
    [System.Serializable]
    public class TrackWheelRow
    {
        [SerializeField] private WheelCollider[] colliders;
        [SerializeField] private Transform[] meshes;

        [SerializeField] private float trackCoefficient;

        [SerializeField] private bool motor;
        public bool Motor => motor;
        [SerializeField] private bool steering;
        public bool Steering => steering;

        [SerializeField] private Renderer trackRenderer;

        public float minRpm;

        private bool disabled;

        public void SetTorque(float torque)
        {
            if (disabled) return;

            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].motorTorque = torque;
            }
        }

        public void Brake(float brakeTorque)
        {
            if (disabled) return;

            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].brakeTorque = brakeTorque;
            }
        }

        public void Reset()
        {
            if (disabled) return;

            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].brakeTorque = 0;
                colliders[i].motorTorque = 0;
            }
        }

        public void UpdateMeshTransform()
        {
            bool anyGrounded = false;
            float firstRpm = 0f;
            float minAbsRpm = 0f;

            for (int i = 0; i < colliders.Length; i++)
            {
                if (!colliders[i].isGrounded) continue;

                float rpm = colliders[i].rpm;

                if (!anyGrounded)
                {
                    anyGrounded = true;
                    firstRpm = rpm;
                    minAbsRpm = Mathf.Abs(rpm);
                }
                else
                {
                    minAbsRpm = Mathf.Min(minAbsRpm, Mathf.Abs(rpm));
                }
            }

            if (anyGrounded)
            {
                minRpm = minAbsRpm * Mathf.Sign(firstRpm);
            }

            float angle = minRpm * 360.0f / 60f * Time.fixedDeltaTime;

            for (int i = 0; i < meshes.Length; i++)
            {
                colliders[i].GetWorldPose(out Vector3 pos, out Quaternion rot);

                meshes[i].position = pos;
                meshes[i].Rotate(angle, 0, 0);
            }

            float step = minRpm * 360.0f / 60f * Time.fixedDeltaTime * trackCoefficient;
            float currentOffset = trackRenderer.material.GetVector("_UvOffset").y;
            trackRenderer.material.SetVector("_UvOffset", new Vector4(0, currentOffset + step, 0, 0));
        }

        public void SetSidewayStiffness(float stiffness)
        {
            if (disabled) return;

            for (int i = 0; i < colliders.Length; i++)
            {
                WheelFrictionCurve wheelFrictionCurve = colliders[i].sidewaysFriction;
                wheelFrictionCurve.stiffness = stiffness;

                colliders[i].sidewaysFriction = wheelFrictionCurve;
            }
        }

        public void UpdateMeshRotationByRpm(float rpm)
        {
            float angle = rpm * 360.0f / 60f * Time.fixedDeltaTime;

            for (int i = 0; i < meshes.Length; i++)
            {
                colliders[i].GetWorldPose(out Vector3 pos, out Quaternion rot);

                meshes[i].position = pos;
                meshes[i].Rotate(angle, 0, 0);
            }
        }

        public void ForceStop()
        {
            disabled = true;

            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].motorTorque = 0f;
                colliders[i].brakeTorque = float.MaxValue;
            }

            UpdateMeshTransform();
        }

        public void SetEnabled()
        {
            disabled = false;
        }
    }
}
