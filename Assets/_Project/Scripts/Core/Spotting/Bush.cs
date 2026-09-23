using UnityEngine;

namespace Tanks.Core
{
    [RequireComponent(typeof(Collider))]
    public class Bush : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            var vehicle = other.transform.root.GetComponent<Vehicle>();
            vehicle?.EnterBush();
        }

        private void OnTriggerExit(Collider other)
        {
            var vehicle = other.transform.root.GetComponent<Vehicle>();
            vehicle?.ExitBush();
        }
    }
}
