using System;

namespace Managers.BattleMgrComponents
{
    [System.Serializable]
    public class BasicPlayerInfo
    {
        public string playerID;
        public string name;
        public int[] pokemonIDs;
        public bool isAI;
        public int teamID;

        public int[] items;
    }
}