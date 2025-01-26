using System.Collections.Generic;

namespace GetyourCrown.Database
{
    public class PlayerData
    {
        public string Nickname = "Guest";
        public int Coins = 0;
        public Dictionary<int, bool> CharactersLocked { get; set; } = new Dictionary<int, bool>();
    }
}
