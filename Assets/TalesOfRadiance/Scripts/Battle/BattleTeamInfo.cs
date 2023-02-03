using System;
using UnityEngine;

namespace TalesOfRadiance.Scripts.Battle
{
    [Serializable]
    public class BattleTeamInfo
    {
        public string playerID;
        public int squadTypeID;
        public int godWeaponID;

        public int[] squadInfoByIndex = new int[9];

        public Guid RuntimeID;

        public BattleTeamInfo()
        {
            RuntimeID = Guid.NewGuid();
        }
    }
}