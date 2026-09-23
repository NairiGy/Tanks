using UnityEngine;

namespace Tanks.Core
{
    public class Armor : MonoBehaviour
    {
        [SerializeField] private int thickness;
        public int Thickness => thickness;

        [SerializeField] private Destructible destructible;
        public Destructible ProtectedDestructible => destructible;
    }
}
