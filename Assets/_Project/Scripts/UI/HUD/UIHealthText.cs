using Tanks.Core;
using UnityEngine;

namespace Tanks.UI
{
    public class UIHealthText : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_Text text;

        private void Update()
        {
            if (Player.Local == null) return;
            if (Player.Local.ActiveVehicle == null) return;

            text.text = Player.Local.ActiveVehicle.HitPoints.ToString();
        }
    }
}
