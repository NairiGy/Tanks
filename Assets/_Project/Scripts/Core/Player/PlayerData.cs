namespace Tanks.Core
{
    [System.Serializable]
    public struct PlayerData : System.IEquatable<PlayerData>
    {
        public uint Id;
        public string Nickname;

        public PlayerData(uint id, string nickname)
        {
            Id = id;
            Nickname = nickname;
        }

        public bool Equals(PlayerData other)
        {
            return Id == other.Id && Nickname == other.Nickname;
        }
    }
}
