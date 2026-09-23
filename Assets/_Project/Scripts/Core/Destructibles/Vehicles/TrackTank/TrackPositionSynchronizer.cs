using UnityEngine;

namespace Tanks.Core
{
    public class TrackPositionSynchronizer : MonoBehaviour
    {
        [SerializeField] private Transform wheel;

        private void Update()
        {
            if (wheel != null)
            {
                this.transform.position = wheel.position;
            }
        }

    }
}
