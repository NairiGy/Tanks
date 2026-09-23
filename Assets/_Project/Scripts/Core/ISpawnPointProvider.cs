using UnityEngine;

namespace Tanks.Core
{
    public interface ISpawnPointProvider
    {
        (Vector3 position, Quaternion rotation) GetSpawnPoint(Team team);
    }
}
