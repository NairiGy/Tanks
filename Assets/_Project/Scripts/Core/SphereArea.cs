using UnityEngine;

namespace Tanks.Core
{
    public class SphereArea : MonoBehaviour
    {
        [SerializeField] private float radius;
        [SerializeField] private Color color = Color.green;

        public Vector3 RandomInside
        {
            get
            {
                var pos = Random.insideUnitSphere  * radius + transform.position;
                pos.y = transform.position.y;
                return pos;
            }
        }

        public (Vector3 position, Quaternion rotation) RandomInsidePose
        {
            get
            {
                var rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
                return (RandomInside, rotation);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(transform.position, radius);
        }
    }
}
