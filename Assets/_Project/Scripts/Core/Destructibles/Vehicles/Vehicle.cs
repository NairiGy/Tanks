using System;
using Mirror;
using UnityEngine;

namespace Tanks.Core
{
    public class Vehicle : Destructible
    {
        [SerializeField] protected float maxLinearVelocity;

        [Header("Sounds")]
        [SerializeField] protected AudioSource engineSfx;
        [SerializeField] protected float engineSfxPitchModifier;
        [SerializeField] protected GameObject vehicleLights;
        [SerializeField] private VehicleModule[] vehicleModules;
        public VehicleModule[] VehicleModules => vehicleModules;

        public Turret Turret;
        [Header("Vehicle")]
        [SerializeField] protected Transform zoomOpticsPos;
        public Transform ZoomOpticsPoint => zoomOpticsPos;
        private bool isOn;
        public virtual float LinearVelocity => 0;

        [SyncVar] private Vector3 netAimPoint;

        [SyncVar(hook = nameof(OnOwnerChanged))] private NetworkIdentity ownerIdentity;
        public NetworkIdentity Owner => ownerIdentity;
        public Player OwnerPlayer => ownerIdentity != null ? ownerIdentity.GetComponent<Player>() : null;
        public bool IsBotControlled => OwnerPlayer != null && OwnerPlayer.IsBot;
        public event Action<Player> OnOwnerReady;

        private const float AimPointSendThresholdSqr = 0.01f;

        public Vector3 NetAimPoint
        {
            get => netAimPoint;

            set
            {
                if ((value - netAimPoint).sqrMagnitude < AimPointSendThresholdSqr) return;

                netAimPoint = value;
                CmdSetNetAimPoint(value);
            }
        }

        public float NormalizedLinearVelocity
        {
            get
            {
                if (Mathf.Approximately(LinearVelocity, 0)) return 0;

                return Mathf.Clamp01(LinearVelocity / maxLinearVelocity);
            }
        }

        protected Vector3 inputControl;

        [Header("Visibility")]
        [SerializeField] private float alwaysVisibleRadius = 20f;
        [SerializeField] private float maxDetectionRange = 150f;
        [SerializeField] private float baseDisguise = 0.6f;
        [SerializeField] private float movingDisguisePenalty = 0.3f;
        [SerializeField] private float firingDisguisePenalty = 0.5f;
        [SerializeField] private float firingDisguisePenaltyDuration = 5f;
        [SerializeField] private float disguiseRecoverySpeed = 0.1f;
        [SerializeField] private float movementDisguiseSpeedThreshold = 0.5f;
        [SerializeField] private float bushDisguiseBonus = 0.3f;

        public float AlwaysVisibleRadius => alwaysVisibleRadius;
        public float MaxDetectionRange => maxDetectionRange;
        public float CurrentDisguise => currentDisguise;

        private float currentDisguise;
        private float lastFiredTime = float.NegativeInfinity;
        private int bushCount;

        public override void OnStartServer()
        {
            base.OnStartServer();

            currentDisguise = baseDisguise;
        }

        protected virtual void Update()
        {
            UpdateEngineSFX();

            if (isServer)
            {
                UpdateDisguise();
            }
        }

        private void UpdateEngineSFX()
        {
            if (engineSfx != null && isOn)
            {
                engineSfx.pitch = 1.0f + NormalizedLinearVelocity * engineSfxPitchModifier;
                engineSfx.volume = 0.5f + NormalizedLinearVelocity;
            }
        }

        private void UpdateDisguise()
        {
            float target = DisguiseMath.TargetDisguise(
                baseDisguise,
                LinearVelocity > movementDisguiseSpeedThreshold,
                movingDisguisePenalty,
                bushCount > 0,
                bushDisguiseBonus,
                Time.time - lastFiredTime < firingDisguisePenaltyDuration,
                firingDisguisePenalty);

            currentDisguise = Mathf.MoveTowards(currentDisguise, target, disguiseRecoverySpeed * Time.deltaTime);
        }

        [Server]
        public void SetOwner(NetworkIdentity owner)
        {
            ownerIdentity = owner;
        }

        private void OnOwnerChanged(NetworkIdentity oldOwner, NetworkIdentity newOwner)
        {
            OnOwnerReady?.Invoke(OwnerPlayer);
        }

        [Server]
        public void SvSetAimPoint(Vector3 aimPoint)
        {
            netAimPoint = aimPoint;
        }

        [Command]
        private void CmdSetNetAimPoint(Vector3 aimPoint)
        {
            if (!IsFinite(aimPoint)) return;

            netAimPoint = aimPoint;
        }

        private static bool IsFinite(Vector3 vector)
        {
            return float.IsFinite(vector.x) && float.IsFinite(vector.y) && float.IsFinite(vector.z);
        }

        public void SetInputControl(Vector3 inputControl)
        {
            this.inputControl = inputControl.normalized;
        }

        public virtual void TurnOnVehicle()
        {
            isOn = true;
            if (vehicleLights)
            {
                vehicleLights.SetActive(true);
            }

        }

        public virtual void TurnOffVehicle()
        {
            isOn = false;
            if (vehicleLights)
            {
                vehicleLights.SetActive(false);
            }
        }

        public void SetVisibility(bool isVisible)
        {
            if (isVisible)
            {
                SetLayerToAll("Default");
            }
            else
            {
                SetLayerToAll("Ignore Main Camera");
            }
        }

        private void SetLayerToAll(string layerName)
        {
            gameObject.layer = LayerMask.NameToLayer(layerName);

            foreach (Transform child in transform.GetComponentsInChildren<Transform>())
            {
                if (child.gameObject.layer == LayerMask.NameToLayer("MinimapIcon")) continue;
                child.gameObject.layer = LayerMask.NameToLayer(layerName);
            }
        }

        public void Fire()
        {
            Turret.Fire();
        }

        [Server]
        public void NotifyFired()
        {
            lastFiredTime = Time.time;
        }

        public void EnterBush()
        {
            bushCount++;

            if (bushCount == 1 && isServer)
            {
                NetworkServer.RebuildObservers(netIdentity, false);
            }
        }

        public void ExitBush()
        {
            bushCount = Mathf.Max(0, bushCount - 1);

            if (bushCount == 0 && isServer)
            {
                NetworkServer.RebuildObservers(netIdentity, false);
            }
        }
    }
}
