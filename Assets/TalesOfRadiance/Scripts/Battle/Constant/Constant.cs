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
            public const string BeforeDamageActive = "BEFORE_DAMAGE_ACTIVE";
            public const string AfterDamageActive = "AFTER_DAMAGE_ACTIVE";
            public const string BeforeDamagePassive = "BEFORE_DAMAGE_PASSIVE";
            public const string AfterDamagePassive = "AFTER_DAMAGE_PASSIVE";
            
            public const string BeforeHealActive = "BEFORE_HEAL_ACTIVE";
            public const string AfterHealActive = "AFTER_HEAL_ACTIVE";
            public const string BeforeHealPassive = "BEFORE_HEAL_PASSIVE";
            public const string AfterHealPassive = "AFTER_HEAL_PASSIVE";
            
            public const string BeforeRound = "BEFORE_ROUND";
            public const string AfterRound = "AFTER_ROUND";
            
            public const string AfterApplyDamageActive = "AFTER_APPLY_DAMAGE_ACTIVE";
            public const string BeforeApplyDamageActive = "BEFORE_APPLY_DAMAGE_ACTIVE";
            public const string AfterApplyDamagePassive = "AFTER_APPLY_DAMAGE_PASSIVE";
            public const string BeforeApplyDamagePassive = "BEFORE_APPLY_DAMAGE_PASSIVE";
            
            public const string AfterApplyHealActive = "AFTER_APPLY_HEAL_ACTIVE";
            public const string BeforeApplyHealActive = "BEFORE_APPLY_HEAL_ACTIVE";
            public const string AfterApplyHealPassive = "AFTER_APPLY_HEAL_PASSIVE";
            public const string BeforeApplyHealPassive = "BEFORE_APPLY_HEAL_PASSIVE";
            
            public const string BeforeFaint = "BEFORE_FAINT";
            public const string AfterAddThisBuff = "AFTER_ADD_THIS_BUFF";
            public const string AfterAddBuff = "AFTER_ADD_BUFF";
            public const string BeforeExecuteBuff = "BEFORE_EXECUTE_BUFF";
        }
    }
}