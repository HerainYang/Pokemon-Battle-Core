using System;

namespace TalesOfRadiance.Scripts.Battle
{
    [Serializable]
    public class BattleInitInfo
    {
        public string playerID;
        public int squadTypeID;
        public int godWeaponID;

        public int[][] SquadInfoByIndex = new[]
        {
            new int[3],
            new int[3],
            new int[3]
        };
    }
}