using UnityEngine;

namespace TalesOfRadiance.Scripts.Battle.Constant
{
    public static class Constant
    {
        public static readonly string[] SquidTypeStrings =
        {
            "锋矢阵",
            "疾风阵",
            "灵法阵",
            "地绝阵",
            "圣灵阵",
            "天攻阵"
        };
        
        public static readonly string[] GodWeapon =
        {
            "盘古斧",
            "埃里克血斧",
            "阿瑞斯战腕",
            "永恒神枪",
            "罗睺之弓",
            "魔神之锤",
            "掌天瓶"
        };
        
        public struct EventKey
        {
            public static string TeamCommandCompleted = "TEAM_COMMAND_COMPLETED";
        }
    }
}