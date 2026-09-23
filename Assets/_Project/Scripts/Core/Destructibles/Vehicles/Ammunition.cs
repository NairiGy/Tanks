using Mirror;
using UnityEngine;

namespace Tanks.Core
{
    [RequireComponent(typeof(Turret))]
    public class Ammunition : NetworkBehaviour
    {
        [SerializeField] private Projectile projectile;
        public Projectile Projectile => projectile;
        [SerializeField] private uint stockAmount;
        public uint StockAmount => stockAmount;

        public bool TryDrawAmmo(uint drawAmount = 1)
        {
            if (drawAmount > stockAmount) return false;

            stockAmount -= drawAmount;
            return true;
        }
    }
}
