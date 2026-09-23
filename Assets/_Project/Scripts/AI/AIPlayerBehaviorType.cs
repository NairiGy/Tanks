namespace Tanks.AI
{
    public enum AIPlayerBehaviorType
    {
        Default, // moves towards enemy base, stops and fires if an enemy is spotted
        Capturer, // captures the enemy base over anything else, never stops to fight
        Defender, // defends own base over anything else
    }
}
