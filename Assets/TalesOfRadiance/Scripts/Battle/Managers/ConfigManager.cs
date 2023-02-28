using System;
using System.Collections.Generic;
using TalesOfRadiance.Scripts.Battle.BattleComponents;
using TalesOfRadiance.Scripts.Battle.BattleComponents.BattleLogics;
using UnityEngine;
using Types = TalesOfRadiance.Scripts.Battle.Constant.Types;

namespace TalesOfRadiance.Scripts.Battle.Managers
{
    public class ConfigManager
    {
        private static ConfigManager _instance;
        private readonly Dictionary<int, HeroTemplate> _heroTemplates = new Dictionary<int, HeroTemplate>();
        private readonly Dictionary<int, SkillTemplate> _skillTemplates = new Dictionary<int, SkillTemplate>();
        private readonly Dictionary<int, SkillTemplate> _buffTemplates = new Dictionary<int, SkillTemplate>();

        public static ConfigManager Instance
        {
            get { return _instance ??= new ConfigManager(); }
        }

        private ConfigManager()
        {
            InitHero();
            InitSkill();
            InitBuff();
        }

        private readonly int[][][] _squadTypeConfig = new[]
        {
            new[]
            {
                new[] { 1, 1, 0 },
                new[] { 0, 0, 1 },
                new[] { 1, 1, 0 }
            },
            new[]
            {
                new[] { 0, 1, 1 },
                new[] { 1, 0, 0 },
                new[] { 0, 1, 1 }
            },
            new[]
            {
                new[] { 0, 0, 1 },
                new[] { 1, 1, 1 },
                new[] { 0, 0, 1 }
            },
            new[]
            {
                new[] { 0, 0, 1 },
                new[] { 1, 1, 1 },
                new[] { 0, 0, 1 }
            },
            new[]
            {
                new[] { 0, 0, 1 },
                new[] { 1, 1, 1 },
                new[] { 0, 0, 1 }
            },
            new[]
            {
                new[] { 0, 0, 1 },
                new[] { 1, 1, 1 },
                new[] { 0, 0, 1 }
            },
        };

        private readonly Vector3[] _anchorPosition = new[]
        {
            new Vector3(-2, 0, 2),
            new Vector3(0, 0, 2),
            new Vector3(2, 0, 2),

            new Vector3(-2, 0, 0),
            new Vector3(0, 0, 0),
            new Vector3(2, 0, 0),

            new Vector3(-2, 0, -2),
            new Vector3(0, 0, -2),
            new Vector3(2, 0, -2),
            
            //god weapon position
            
            new Vector3(4, 0, 4)
        };

        private void InitHero()
        {
            _heroTemplates.Add(0, new HeroTemplate(0, "海姆达尔", "TORC_ch006")
            {
                Attack = 75396,
                MaxHealth = 1052726,
                Defence = 2542,
                Speed = 1804,

                CriticalRate = 0.02f,
                CriticalDamage = 1.64f,
                ControlRate = 0.125f,
                AntiControl = 0.02f,
                AntiCritical = 0.325f,
                Accuracy = 1.02f,
                DamageAvoid = 0.1f,
                DodgeRate = 0.02f,
                HealRate = 0f,
                GetHealRate = 0f,
                DamageIncrease = 0.021f,
                PhysicalDamageIncrease = 0f,
                SpecialDamageIncrease = 0f,
                PhysicalDamageAvoid = 0.105f,
                SpecialDamageAvoid = 0f,
                SustainDamageIncrease = 0f,
                SustainDamageAvoid = 0f,
                
                SkillIndices = new []{1, 2, 3, 4}
            });
            _heroTemplates.Add(1, new HeroTemplate(1, "神灵大祭司", "TORC_ch020")
            {
                Attack = 82849,
                MaxHealth = 820257,
                Defence = 2157,
                Speed = 1817,

                CriticalRate = 0.02f,
                CriticalDamage = 1.54f,
                ControlRate = 0.02f,
                AntiControl = 0.125f,
                AntiCritical = 0.125f,
                Accuracy = 1.02f,
                DamageAvoid = 0f,
                DodgeRate = 0.02f,
                HealRate = 0.1f,
                GetHealRate = 0f,
                DamageIncrease = 0.021f,
                PhysicalDamageIncrease = 0.1f,
                SpecialDamageIncrease = 0f,
                PhysicalDamageAvoid = 0f,
                SpecialDamageAvoid = 0.15f,
                SustainDamageIncrease = 0f,
                SustainDamageAvoid = 0.003f,
            });
            _heroTemplates.Add(2, new HeroTemplate(2, "红莲哪吒", "TORC_ch028")
            {
                Attack = 203688,
                MaxHealth = 1561792,
                Defence = 3480,
                Speed = 2007,

                CriticalRate = 0.495f,
                CriticalDamage = 1.95f,
                ControlRate = 0.02f,
                AntiControl = 0.02f,
                AntiCritical = 0.25f,
                Accuracy = 1.02f,
                DamageAvoid = 0f,
                DodgeRate = 0.02f,
                HealRate = 0f,
                GetHealRate = 0f,
                DamageIncrease = 0.171f,
                PhysicalDamageIncrease = 0f,
                SpecialDamageIncrease = 0f,
                PhysicalDamageAvoid = 0f,
                SpecialDamageAvoid = 0f,
                SustainDamageIncrease = 0f,
                SustainDamageAvoid = 0.003f,
            });
            _heroTemplates.Add(3, new HeroTemplate(3, "麒麟", "TORC_ch065")
            {
                Attack = 109362,
                MaxHealth = 724466,
                Defence = 2739,
                Speed = 1829,

                CriticalRate = 0.285f,
                CriticalDamage = 1.85f,
                ControlRate = 0.02f,
                AntiControl = 0.02f,
                AntiCritical = 0.07f,
                Accuracy = 1.02f,
                DamageAvoid = 0f,
                DodgeRate = 0.02f,
                HealRate = 0f,
                GetHealRate = 0f,
                DamageIncrease = 0.171f,
                PhysicalDamageIncrease = 0.15f,
                SpecialDamageIncrease = 0f,
                PhysicalDamageAvoid = 0.15f,
                SpecialDamageAvoid = 0f,
                SustainDamageIncrease = 0f,
                SustainDamageAvoid = 0.003f,
            });
            _heroTemplates.Add(4, new HeroTemplate(4, "女娲", "TORC_ch123")
            {
                Attack = 103524,
                MaxHealth = 1027987,
                Defence = 2265,
                Speed = 2224,

                CriticalRate = 0.02f,
                CriticalDamage = 1.54f,
                ControlRate = 0.02f,
                AntiControl = 0.325f,
                AntiCritical = 0.175f,
                Accuracy = 1.02f,
                DamageAvoid = 0f,
                DodgeRate = 0.02f,
                HealRate = 0f,
                GetHealRate = 0f,
                DamageIncrease = 0.021f,
                PhysicalDamageIncrease = 0f,
                SpecialDamageIncrease = 0f,
                PhysicalDamageAvoid = 0f,
                SpecialDamageAvoid = 0.105f,
                SustainDamageIncrease = 0f,
                SustainDamageAvoid = 0.093f,
                
                SkillIndices = new []{5, 7}
            });
        }

        private void InitBuff()
        {
            _buffTemplates.Add(0, new SkillTemplate(0, "BUFF测试", 3, Types.BuffType.Positive, BattleLogic.ForBuffTest, null, "FORTESTBUFF"));
            _buffTemplates.Add(1, new SkillTemplate(1, "ChangeHpOnDebut", Int32.MaxValue, Types.BuffType.Positive, BattleLogic.ChangeHpMax, BattleLogic.UndoChangeHpMax, Constant.Constant.BuffEventKey.AfterDebut)
            {
                ValueChangeRate = 0.4f,
                RecoverHp = true
            });
            _buffTemplates.Add(2, new SkillTemplate(2, "ChangeDefenceOnDebut", Int32.MaxValue, Types.BuffType.Positive, BattleLogic.ChangeDefense, BattleLogic.UndoChangeDefense, Constant.Constant.BuffEventKey.AfterDebut)
            {
                ValueChangeRate = 0.2f
            });
            _buffTemplates.Add(3, new SkillTemplate(3, "IncreaseDamageAvoidWhenHpDecrease", Int32.MaxValue, Types.BuffType.Positive, BattleLogic.IncreaseDamageAvoidWhenHpDecrease, BattleLogic.UndoIncreaseDamageAvoidWhenHpDecrease, Constant.Constant.BuffEventKey.AfterDamage)
            {
                ValueChangeRate = 3,
            });
            _buffTemplates.Add(4, new SkillTemplate(4, "持续回复", 2, Types.BuffType.Positive, BattleLogic.ContinueHeal, null, Constant.Constant.BuffEventKey.AfterRound)
            {
                PercentageDamageRate = 0.15f,
            });
            _buffTemplates.Add(5, new SkillTemplate(5, "烧伤", 2, Types.BuffType.Negative, BattleLogic.Burnt, null, Constant.Constant.BuffEventKey.AfterRound)
            {
                PercentageDamageRate = 0.2f,
            });
            _buffTemplates.Add(6, new SkillTemplate(6, "烈焰审判", 2, Types.BuffType.Negative, BattleLogic.FireJudgement, null, Constant.Constant.BuffEventKey.AfterApplyDamage)
            {
                PercentageDamageRate = 0.2f,
            });
            _buffTemplates.Add(7, new SkillTemplate(7, "破晓守护前置", Int32.MaxValue, Types.BuffType.Positive, BattleLogic.DawnProtectPrefix, BattleLogic.UndoDawnProtect, Constant.Constant.BuffEventKey.BeforeRound)
            {
                TargetCount = 1
            });
            
            _buffTemplates.Add(8, new SkillTemplate(8, "破晓守护", 1, Types.BuffType.Positive, BattleLogic.DawnProtect, null, Constant.Constant.BuffEventKey.BeforeDamage)
            {
                PercentageDamageRate = 0.75f
            });
            
            _buffTemplates.Add(9, new SkillTemplate(9, "永生幻境", 2, Types.BuffType.Positive, BattleLogic.Immortal, BattleLogic.UndoImmortal, Constant.Constant.BuffEventKey.BeforeFaint)
            {
                PercentageDamageRate = 0.75f
            });
        }

        private void InitSkill()
        {
            _skillTemplates.Add(0, new SkillTemplate(0, "普通攻击", new[] { BattleLogic.NormalAttack })
            {
                DamageIncreaseRate = 1f
            });

            _skillTemplates.Add(1, new SkillTemplate(1, "天卫之躯", new[] { BattleLogic.CleanAllNegativeBuff, BattleLogic.HealWithExceedShield, BattleLogic.TryAddContinuousHealBuff }, new []{BattleLogic.SelectSelf})
            {
                PercentageDamageRate = 0.2f,
                InitCd = 1,
                Cd = 4
            });
            
            _skillTemplates.Add(2, new SkillTemplate(2, "烈焰重斩", new[] { BattleLogic.TryApplyPercentageDamage, BattleLogic.TryAddBuffInBuffList }, new []{BattleLogic.SelectAppendFront})
            {
                PercentageDamageRate = 0.2f,
                AddBuffPossibility = new []{0.7f},
                AddBuffIndex = new []{ new []{5, 6}},
                InitCd = 1,
                Cd = 4
            });
            
            _skillTemplates.Add(3, new SkillTemplate(3, "不屈守护神", new []{1, 2, 3}));
            
            _skillTemplates.Add(4, new SkillTemplate(4, "破晓守护", new []{7}));
            
            _skillTemplates.Add(5, new SkillTemplate(5, "炼石补天", new [] {BattleLogic.StealOneBuff}, new []{BattleLogic.SelectRandomEnemy})
            {
                InitCd = 1,
                Cd = 2,
                TargetCount = 3
            });
            
            _skillTemplates.Add(7, new SkillTemplate(7, "抟土造人I", new [] {BattleLogic.TryBringBackToLife, BattleLogic.ExecuteNextSkillPlayable}, new []{BattleLogic.SelectOneFaintTeammate})
            {
                NextSkillID = 8,
                InitCd = 2,
                Cd = 4,
            });
            
            _skillTemplates.Add(8, new SkillTemplate(8, "抟土造人II", new [] {BattleLogic.TryAddBuffInBuffList, BattleLogic.TryHealByAttack}, new []{BattleLogic.SelectAllTeammate, BattleLogic.SelectLowestHpFromPrevious})
            {
                AddBuffPossibility = new []{1f},
                AddBuffIndex = new []{ new []{9}},
                TargetCount = 2,
                DamageIncreaseRate = 1.5f
            });
        }

        public HeroTemplate GetHeroTemplateByID(int id)
        {
            return _heroTemplates[id];
        }

        public SkillTemplate GetBuffTemplateByID(int id)
        {
            return _buffTemplates[id];
        }

        public int[][] GetSquadTypeByID(int id)
        {
            return _squadTypeConfig[id];
        }

        public Vector3 GetAnchorPosition(int id)
        {
            return _anchorPosition[id];
        }

        public SkillTemplate GetSkillTemplateByID(int id)
        {
            return _skillTemplates[id];
        }
    }
}