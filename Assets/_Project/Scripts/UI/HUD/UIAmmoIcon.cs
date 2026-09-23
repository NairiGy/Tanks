using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Tanks.UI
{
    public class UIAmmoIcon : MonoBehaviour
    {
        [SerializeField] private Image image;
        [FormerlySerializedAs("ammountText")]
        [SerializeField] private TMPro.TMP_Text amountText;
        [SerializeField] private Color defaultColor;
        [SerializeField] private Color highlightColor;

        public void SetAmmoIcon(Sprite sprite)
        {
            image.sprite = sprite;
        }

        public void SetAmmoAmount(uint amount)
        {
            amountText.text = amount.ToString();
        }

        public void ToggleHighlight(bool isHighlighted)
        {
            image.color = isHighlighted ? highlightColor : defaultColor;
        }

    }
}
