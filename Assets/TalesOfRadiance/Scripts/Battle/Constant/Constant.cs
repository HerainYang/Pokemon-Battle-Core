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

        public struct BuffEventKey
        {
            public const string AfterDebut = "AFTER_DEBUT";
            public const string BeforeDamage = "BEFORE_DAMAGE";
            public const string AfterDamage = "AFTER_DAMAGE";
            public const string BeforeRound = "BEFORE_ROUND";
            public const string AfterRound = "AFTER_ROUND";
            public const string AfterHeal = "AFTER_HEAL";
            public const string AfterApplyDamage = "AFTER_APPLY_DAMAGE";
            public const string BeforeFaint = "BEFORE_FAINT";
            public const string AfterAddBuff = "AFTER_ADD_BUFF";
            public const string BeforeExecuteBuff = "BEFORE_EXECUTE_BUFF";
        }
    }
}