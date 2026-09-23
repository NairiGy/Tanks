using UnityEngine;

namespace Tanks.Core
{
    public class VehicleMinimapIcon : MonoBehaviour
    {
        [SerializeField] private Color allyColor;
        [SerializeField] private Color enemyColor;
        [SerializeField] private Color ownColor;
        [SerializeField] private MeshRenderer iconRenderer;
        private MaterialPropertyBlock propBlock;
        [SerializeField] private Vehicle vehicle;

        private bool isDone;

        private void Start()
        {
            propBlock = new MaterialPropertyBlock();
        }

        private void Update()
        {
            if (isDone) return;

            if (Player.Local && vehicle.OwnerPlayer)
            {
                Setup();
                isDone = true;  
            }
        }

        private void Setup()
        {
            Color c;
            if (Player.Local == vehicle.OwnerPlayer)
                c = ownColor;
            else if (Player.Local.TeamId == vehicle.OwnerPlayer.TeamId)
                c = allyColor;
            else
                c = enemyColor;

            iconRenderer.GetPropertyBlock(propBlock);
            propBlock.SetColor("_BaseColor", c);
            iconRenderer.SetPropertyBlock(propBlock);
        }
    }
}
