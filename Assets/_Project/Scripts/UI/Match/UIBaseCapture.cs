using Tanks.Core;
using Tanks.Match;
using UnityEngine;
using UnityEngine.UI;

namespace Tanks.UI
{
    public class UIBaseCapture : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private ConditionTeamBaseCapture conditionTeamBaseCapture;

        private bool isVisible;

        private void Start()
        {
            ToggleUI(false);
        }

        private void Update()
        {
            if (Player.Local == null) return;

            slider.value = conditionTeamBaseCapture.CapturePercentage();

            if (slider.value == 0 && isVisible)
            {
                ToggleUI(false);
            }

            if (slider.value != 0 && !isVisible)
            {
                ToggleUI(true);
            }
        }

        private void ToggleUI(bool toggle)
        {
            isVisible = toggle;

            foreach (RectTransform child in slider.transform)
            {
                child.gameObject.SetActive(toggle);
            }
        }
    }
}
