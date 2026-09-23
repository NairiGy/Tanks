using System;
using Tanks.Match;
using TMPro;
using UnityEngine;

namespace Tanks.UI
{
    public class UIMatchTimer : MonoBehaviour
    {
        [SerializeField] private ConditionMatchTimer conditionMatchTimer;

        [SerializeField] private TMP_Text matchTimerText;

        private void Update()
        {
            matchTimerText.text = TimeSpan.FromSeconds(conditionMatchTimer.TimeLeft).ToString(@"m\:ss");
        }
    }
}
