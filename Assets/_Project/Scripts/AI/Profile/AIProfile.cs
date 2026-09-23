using UnityEngine;

namespace Tanks.AI
{
    [CreateAssetMenu(fileName = "AIProfile", menuName = "Tanks/AI Profile")]
    public class AIProfile : ScriptableObject
    {
        [SerializeField] private NavigationSettings navigation = new NavigationSettings();
        [SerializeField] private CombatSettings combat = new CombatSettings();
        [SerializeField] private UnstuckSettings unstuck = new UnstuckSettings();
        [SerializeField] private BushHidingSettings bushHiding = new BushHidingSettings();
        [SerializeField] private CornerRoamingSettings cornerRoaming = new CornerRoamingSettings();

        public NavigationSettings Navigation => navigation;
        public CombatSettings Combat => combat;
        public UnstuckSettings Unstuck => unstuck;
        public BushHidingSettings BushHiding => bushHiding;
        public CornerRoamingSettings CornerRoaming => cornerRoaming;
    }
}
