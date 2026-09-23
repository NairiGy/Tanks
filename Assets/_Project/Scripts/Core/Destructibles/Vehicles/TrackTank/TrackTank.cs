using Mirror;
using UnityEngine;

namespace Tanks.Core
{
    public enum TrackSide
    {
        Left,
        Right
    }

    [RequireComponent(typeof(Rigidbody))]
    public class TrackTank : Vehicle
    {
        public override float LinearVelocity => rb.linearVelocity.magnitude;

        [SerializeField] private TrackWheelRow leftWheelRow;
        [SerializeField] private TrackWheelRow rightWheelRow;

        [Header("Movement")]
        [SerializeField] private ParameterCurve forwardTorqueCurve;
        [SerializeField] private ParameterCurve backwardTorqueCurve;
        [SerializeField] private float maxForwardTorque;
        [SerializeField] private float maxBackwardTorque;
        [SerializeField] private float brakeTorque;
        [SerializeField] private float rollingResistance;

        [Header("Rotation")]
        [SerializeField] private float rotateTorqueInPlace;
        [SerializeField] private float rotateBrakeInPlace;
        [Space(2)]
        [SerializeField] private float rotateTorqueInMotion;
        [SerializeField] private float rotateBrakeInMotion;

        [Header("Friction")]
        [SerializeField] private float minSidewaysFrictionInPlace;
        [SerializeField] private float minSidewaysFrictionInMotion;

        private Rigidbody rb;
        private float currentTorque;

        [SerializeField] private GameObject destroyedPrefab;
        [SerializeField] private GameObject visualModel;
        private bool isWrecked;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (isOwned)
            {
                UpdateMotor();

                CmdUpdateWheelRpm(leftWheelRow.minRpm, rightWheelRow.minRpm);
            }
            else if (isServer && IsBotControlled)
            {
                UpdateMotor();

                SvUpdateWheelRpm(leftWheelRow.minRpm, rightWheelRow.minRpm);
            }
        }

        private void UpdateMotor()
        {
            float motorTorque = inputControl.z >= 0 ? maxForwardTorque * Mathf.RoundToInt(inputControl.z) : maxBackwardTorque * Mathf.RoundToInt(inputControl.z);
            float brakeTorque = this.brakeTorque * inputControl.y;
            float steering = inputControl.x;

            // Update torque

            if (motorTorque > 0)
            {
                currentTorque = forwardTorqueCurve.MoveForward(Time.fixedDeltaTime) * motorTorque;
            }

            if (motorTorque < 0)
            {
                currentTorque = backwardTorqueCurve.MoveForward(Time.fixedDeltaTime) * motorTorque;
            }

            if (motorTorque == 0)
            {
                currentTorque = forwardTorqueCurve.Restart();
                currentTorque = backwardTorqueCurve.Restart();

            }

            // Brake
            leftWheelRow.Brake(brakeTorque);
            rightWheelRow.Brake(brakeTorque);

            // Rolling
            if (motorTorque == 0 && steering == 0)
            {
                leftWheelRow.Brake(rollingResistance);
                rightWheelRow.Brake(rollingResistance);
            }
            else
            {
                leftWheelRow.Reset();
                rightWheelRow.Reset();
            }

            // Rotate in place
            if (motorTorque == 0 && steering != 0)
            {
                if (LinearVelocity < 0.5f)
                {
                    leftWheelRow.SetTorque(rotateTorqueInPlace);
                    rightWheelRow.SetTorque(rotateBrakeInPlace);
                }
                else if (steering != 0)
                {
                    if (steering < 0)
                    {
                        leftWheelRow.Brake(rotateBrakeInPlace);
                        rightWheelRow.SetTorque(rotateTorqueInPlace);
                    }

                    if (steering > 0)
                    {
                        leftWheelRow.SetTorque(rotateTorqueInPlace);
                        rightWheelRow.Brake(rotateBrakeInPlace);
                    }
                }

                leftWheelRow.SetSidewayStiffness(1.0f + minSidewaysFrictionInPlace - Mathf.Abs(steering));
                rightWheelRow.SetSidewayStiffness(1.0f + minSidewaysFrictionInPlace - Mathf.Abs(steering));
            }

            // Move
            if (motorTorque != 0)
            {
                if (steering == 0)
                {
                    if (LinearVelocity < maxLinearVelocity)
                    {
                        leftWheelRow.SetTorque(motorTorque);
                        rightWheelRow.SetTorque(motorTorque);
                    }
                }
                else // steering != 0 — only pivot-blend when actually steering
                {
                    if (LinearVelocity < 0.5f)
                    {
                        leftWheelRow.SetTorque(rotateTorqueInMotion);
                        rightWheelRow.SetTorque(rotateBrakeInMotion);
                    }
                    else
                    {
                        if (steering < 0)
                        {
                            leftWheelRow.Brake(rotateBrakeInMotion);
                            rightWheelRow.SetTorque(rotateTorqueInMotion);
                        }

                        if (steering > 0)
                        {
                            leftWheelRow.SetTorque(rotateTorqueInMotion);
                            rightWheelRow.Brake(rotateBrakeInMotion);
                        }
                    }
                }

                leftWheelRow.SetSidewayStiffness(1.0f + minSidewaysFrictionInMotion - Mathf.Abs(steering));
                rightWheelRow.SetSidewayStiffness(1.0f + minSidewaysFrictionInMotion - Mathf.Abs(steering));
            }

            leftWheelRow.UpdateMeshTransform();
            rightWheelRow.UpdateMeshTransform();
        }

        public void SetTrackDisabled(TrackSide side, bool disabled)
        {
            TrackWheelRow row = side == TrackSide.Left ? leftWheelRow : rightWheelRow;

            if (disabled)
            {
                row.ForceStop();
            }
            else
            {
                row.SetEnabled();
            }
        }

        [Command]
        private void CmdUpdateWheelRpm(float leftRpm, float rightRpm)
        {
            SvUpdateWheelRpm(leftRpm, rightRpm);
        }

        [Server]
        private void SvUpdateWheelRpm(float leftRpm, float rightRpm)
        {
            RpcUpdateWheelRpm(leftRpm, rightRpm);
        }

        [ClientRpc(includeOwner = false)]
        private void RpcUpdateWheelRpm(float leftRpm, float rightRpm)
        {
            leftWheelRow.minRpm = leftRpm;
            rightWheelRow.minRpm  = rightRpm;

            leftWheelRow.UpdateMeshRotationByRpm(leftRpm);
            rightWheelRow.UpdateMeshRotationByRpm(rightRpm);
        }

        protected override void OnDestroyed()
        {
            if (isWrecked) return;
            isWrecked = true;

            base.OnDestroyed();

            if (visualModel != null)
            {
                visualModel.SetActive(false);
            }

            Rigidbody body = GetComponent<Rigidbody>();
            if (body != null)
            {
                body.isKinematic = true;
            }

            GameObject ruinedVisualModel = Instantiate(destroyedPrefab);
            ruinedVisualModel.transform.position = transform.position;
            ruinedVisualModel.transform.rotation = transform.rotation;
        }
    }
}
