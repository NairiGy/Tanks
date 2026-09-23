using Tanks.Core;
using UnityEngine;

namespace Tanks.UI
{
    public class UIAmmoPanel : MonoBehaviour
    {
        [SerializeField] private UIAmmoIcon uiAmmoIconPrefab;
        private Turret turret;

        private bool initialized;

        private void Update()
        {
            if (initialized) return;

            if (Player.Local && Player.Local.ActiveVehicle && Player.Local.ActiveVehicle.Turret)
            {
                turret = Player.Local.ActiveVehicle.Turret;
                turret.AmmoCountChanged += HandleAmmoCountChanged;
                turret.AmmunitionsChanged += HandleAmmoChanged;
                DrawUI();
                initialized = true;
            }
        }

        private void OnDestroy()
        {
            if (turret == null) return;

            turret.AmmoCountChanged -= HandleAmmoCountChanged;
            turret.AmmunitionsChanged -= HandleAmmoChanged;
        }

        private void DrawUI()
        {
            if (turret == null) return;
            ClearUI();

            foreach (Ammunition ammo in turret.Ammunitions)
            {
                UIAmmoIcon uiAmmoIcon = Instantiate(uiAmmoIconPrefab, transform);

                uiAmmoIcon.SetAmmoIcon(ammo.Projectile.Props.Icon);
                uiAmmoIcon.SetAmmoAmount(ammo.StockAmount);
            }
        }

        private void ClearUI()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        private void HandleAmmoCountChanged()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            var children = gameObject.GetComponentsInChildren<UIAmmoIcon>();

            for (int i = 0; i < children.Length; i++)
            {
                children[i].SetAmmoAmount(turret.Ammunitions[i].StockAmount);
            }
        }

        private void HandleAmmoChanged(uint obj)
        {
            HighlightIcon(obj);
        }

        private void HighlightIcon(uint ind)
        {
            var children = gameObject.GetComponentsInChildren<UIAmmoIcon>();

            for (int i = 0; i < children.Length; i++)
            {
                children[i].ToggleHighlight(i == ind);
            }
        }
    }
}
