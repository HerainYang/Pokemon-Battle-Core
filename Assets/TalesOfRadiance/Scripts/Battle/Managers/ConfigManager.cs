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
                
                SkillIndices = new []{12, 14, 18, 19, 20}
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
                
                SkillIndices = new []{21, 25}
            });
            _heroTemplates.Add(4, new HeroTemplate(4, "女娲", "TORC_ch021")
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
                
                SkillIndices = new []{5, 7, 9, 11}
            });
        }

        private void InitBuff()
        {
            _buffTemplates.Add(0, new SkillTemplate(0, "BUFF测试", 3, Types.BuffType.Positive, BattleLogic.ForBuffTest, null, "FORTESTBUFF"));
            _buffTemplates.Add(1, new SkillTemplate(1, "ChangeHpOnDebut", Int32.MaxValue, Types.BuffType.Positive, BattleLogic.ChangeHpMax, BattleLogic.UndoChangeHpMax, Constant.Constant.BuffEventKey.AfterDebut)
            {
                ValueChangeRate = 0.4f
            });
            _buffTemplates.Add(2, new SkillTemplate(2, "ChangeDefenceOnDebut", Int32.MaxValue, Types.BuffType.Positive, BattleLogic.ChangeDefense, BattleLogic.UndoChangeDefense, Constant.Constant.BuffEventKey.AfterDebut)
            {
                ValueChangeRate = 0.2f
            });
            _buffTemplates.Add(3, new SkillTemplate(3, "IncreaseDamageAvoidWhenHpDecrease", Int32.MaxValue, Types.BuffType.Positive, BattleLogic.IncreaseDamageAvoidWhenHpDecrease, BattleLogic.UndoIncreaseDamageAvoidWhenHpDecrease, Constant.Constant.BuffEventKey.AfterDamageActive)
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
            _buffTemplates.Add(6, new SkillTemplate(6, "烈焰审判", 2, Types.BuffType.Negative, BattleLogic.FireJudgement, null, Constant.Constant.BuffEventKey.AfterApplyDamageActive)
            {
                PercentageDamageRate = 0.2f,
                HaveBuffNumberLimit = true
            });
            _buffTemplates.Add(7, new SkillTemplate(7, "破晓守护I", Int32.MaxValue, Types.BuffType.Positive, BattleLogic.DawnProtectPrefix, BattleLogic.UndoDawnProtect, Constant.Constant.BuffEventKey.BeforeRound)
            {
                TargetCount = 1
            });
            
            _buffTemplates.Add(8, new SkillTemplate(8, "破晓守护", 1, Types.BuffType.Positive, BattleLogic.DawnProtect, null, Constant.Constant.BuffEventKey.BeforeDamageActive)
            {
                PercentageDamageRate = 0.75f,
                HaveBuffNumberLimit = true
            });
            
            _buffTemplates.Add(9, new SkillTemplate(9, "永生幻境", 2, Types.BuffType.Positive, BattleLogic.Immortal, BattleLogic.UndoImmortal, Constant.Constant.BuffEventKey.BeforeFaint)
            {
                PercentageDamageRate = 0.75f,
                HaveBuffNumberLimit = true
            });
            
            _buffTemplates.Add(10, new SkillTemplate(10, "始母之血", Int32.MaxValue, Types.BuffType.Positive, BattleLogic.DamageToTeamHeal, null, Constant.Constant.BuffEventKey.AfterApplyDamageActive)
            {
                PercentageDamageRate = 0.15f
            });
            
            _buffTemplates.Add(11, new SkillTemplate(11, "IncreaseAttack", 2, Types.BuffType.Positive, BattleLogic.ChangeAttack, BattleLogic.UndoChangeAttack, Constant.Constant.BuffEventKey.AfterAddThisBuff)
            {
                ValueChangeRate = 0.25f
            });
            
            _buffTemplates.Add(12, new SkillTemplate(12, "IncreaseAttackOnDebut", Int32.MaxValue, Types.BuffType.Positive, BattleLogic.ChangeAttack, BattleLogic.UndoChangeAttack, Constant.Constant.BuffEventKey.AfterDebut)
            {
                ValueChangeRate = 0.4f
            });
            
            _buffTemplates.Add(13, new SkillTemplate(13, "IncreaseAttack", Int32.MaxValue, Types.BuffType.Positive, BattleLogic.ChangeHpMax, BattleLogic.UndoChangeHpMax, Constant.Constant.BuffEventKey.AfterDebut)
            {
                ValueChangeRate = 0.2f
            });
            
            _buffTemplates.Add(14, new SkillTemplate(14, "焚天红莲BuffI", Int32.MaxValue, Types.BuffType.Positive, BattleLogic.BurntLotus1, null, Constant.Constant.BuffEventKey.BeforeExecuteBuff)
            {
                ValueChangeRate = 0.15f
            });
            
            _buffTemplates.Add(15, new SkillTemplate(15, "焚天红莲BuffII", Int32.MaxValue, Types.BuffType.Positive, BattleLogic.BurntLotus2, null, Constant.Constant.BuffEventKey.AfterRound));
            
            _buffTemplates.Add(16, new SkillTemplate(16, "圣火", 2, Types.BuffType.Negative, BattleLogic.Burnt, null, Constant.Constant.BuffEventKey.AfterRound)
            {
                PercentageDamageRate = 0.2f,
                HaveBuffNumberLimit = true,
                BuffNumberLimit = 6,
                CurrentBuffRemovePriority = Types.BuffRemovePriority.HolyFire
            });

            _buffTemplates.Add(17, new SkillTemplate(17, "化莲", 4, Types.BuffType.Positive, BattleLogic.ForBuffTest, null, Constant.Constant.BuffEventKey.AfterAddThisBuff)
            {
                HaveBuffNumberLimit = true,
                CurrentBuffRemovePriority = Types.BuffRemovePriority.BecomeLotus
            });
            
            _buffTemplates.Add(18, new SkillTemplate(18, "圣境化莲", Int32.MaxValue, Types.BuffType.Positive, BattleLogic.AccumulateDamageAddBuffInList, BattleLogic.RemoveAllBuffInList, Constant.Constant.BuffEventKey.AfterDamageActive)
            {
                HaveBuffNumberLimit = true,
                ValueChangeRate = 0.09f,
                AddBuffIndex = new []{ new []{19}},
            });
            
            _buffTemplates.Add(19, new SkillTemplate(19, "心莲", Int32.MaxValue, Types.BuffType.Positive, BattleLogic.ChangeDamageAvoid, BattleLogic.UndoChangeDamageAvoid, Constant.Constant.BuffEventKey.AfterAddThisBuff)
            {
                HaveBuffNumberLimit = true,
                BuffNumberLimit = 9,
                CurrentBuffRemovePriority = Types.BuffRemovePriority.HeartLotus,
                ValueChangeRate = 0.012f
            });
            
            _buffTemplates.Add(20, new SkillTemplate(20, "心莲CheckNumber", Int32.MaxValue, Types.BuffType.Positive, BattleLogic.CheckHeartLotusNumber, null, Constant.Constant.BuffEventKey.AfterAddBuff)
            {
                HaveBuffNumberLimit = true,
            });
            
            _buffTemplates.Add(21, new SkillTemplate(21, "IncreaseAttack", Int32.MaxValue, Types.BuffType.Positive, BattleLogic.ChangeAttack, BattleLogic.UndoChangeAttack, Constant.Constant.BuffEventKey.AfterAddThisBuff)
            {
                ValueChangeRate = 0.4f
            });
            
            _buffTemplates.Add(22, new SkillTemplate(22, "ChangeHpOnDebut", Int32.MaxValue, Types.BuffType.Positive, BattleLogic.ChangeHpMax, BattleLogic.UndoChangeHpMax, Constant.Constant.BuffEventKey.AfterDebut)
            {
                ValueChangeRate = 0.2f
            });
            
            _buffTemplates.Add(23, new SkillTemplate(23, "ChangeCriticalRate", Int32.MaxValue, Types.BuffType.Positive, BattleLogic.ChangeCriticalRate, BattleLogic.UndoChangeCriticalRate, Constant.Constant.BuffEventKey.AfterDebut)
            {
                ValueChangeRate = 0.15f
            });

            _buffTemplates.Add(24, new SkillTemplate(24, "肉身成圣", Int32.MaxValue, Types.BuffType.Positive, BattleLogic.NormalBecomeGod, null, Constant.Constant.BuffEventKey.BeforeDamageActive)
            {
                ValueChangeRate = 0.05f
            });

            _buffTemplates.Add(25, new SkillTemplate(25, "羽翎守护", Int32.MaxValue, Types.BuffType.Positive, BattleLogic.FeatherProtect, null, Constant.Constant.BuffEventKey.BeforeDamageActive)
            {
                MultiLayerBuff = true,
                BuffNumberLimit = 4,
                CurrentBuffRemovePriority = Types.BuffRemovePriority.FeatherProtect
            });
            
            
            _buffTemplates.Add(26, new SkillTemplate(26, "万象更新Prefix", Int32.MaxValue, Types.BuffType.Positive, BattleLogic.EverythingUpdatePrefix, null, Constant.Constant.BuffEventKey.BeforeApplyDamageActive)
            {
                ValueChangeRate = 0.05f
            });

            _buffTemplates.Add(27, new SkillTemplate(27, "万象更新Postfix", Int32.MaxValue, Types.BuffType.Positive, BattleLogic.EverythingUpdatePostFix, null, Constant.Constant.BuffEventKey.AfterApplyDamageActive));
            
            _buffTemplates.Add(28, new SkillTemplate(21, "DecreaseAttack", Int32.MaxValue, Types.BuffType.Negative, BattleLogic.ChangeAttack, BattleLogic.UndoChangeAttack, Constant.Constant.BuffEventKey.AfterAddThisBuff)
            {
                ValueChangeRate = -0.6f
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
                Cd = 4,
                CurrentBuffRemovePriority = Types.BuffRemovePriority.NormalRemove
            });
            
            _skillTemplates.Add(2, new SkillTemplate(2, "烈焰重斩", new[] { BattleLogic.TryApplyPercentageDamage, BattleLogic.TryAddBuffInBuffList }, new []{BattleLogic.SelectAppendFront})
            {
                PercentageDamageRate = 0.2f,
                AddBuffPossibility = new []{0.7f},
                AddBuffIndex = new []{ new []{5, 6}},
                InitCd = 1,
                Cd = 4
            });
            
            _skillTemplates.Add(3, new SkillTemplate(3, "不屈守护神", new []{1, 2, 3}, new []{BattleLogic.SelectSelf}));
            
            _skillTemplates.Add(4, new SkillTemplate(4, "破晓守护", new []{7}, new []{BattleLogic.SelectSelf}));
            
            _skillTemplates.Add(5, new SkillTemplate(5, "炼石补天I", new [] {BattleLogic.StealOneBuff, BattleLogic.NormalAttack}, new []{BattleLogic.SelectRandomEnemy}, new []{ BattleLogic.MergeStealBuffIDs, BattleLogic.CallBackExecuteNextSkillPlayableWithInput})
            {
                InitCd = 1,
                Cd = 2,
                TargetCount = 3,
                NextSkillID = 6,
                DamageIncreaseRate = 1.05f,
                CurrentBuffRemovePriority = Types.BuffRemovePriority.NormalSteal
            });
            
            _skillTemplates.Add(6, new SkillTemplate(6, "炼石补天II", new [] {BattleLogic.TryAddBuffInInputStealBuffList}, new []{BattleLogic.SelectRandomTeammateExceptSelf}, new []{BattleLogic.CallBackExecuteNextSkillPlayableWithInput})
            {
                NextSkillID = 10
            });
            
            _skillTemplates.Add(10, new SkillTemplate(6, "炼石补天III", new [] {BattleLogic.TryAddBuffInBuffList}, new []{BattleLogic.SelectRandomTeammateExceptSelf})
            {
                AddBuffPossibility = new []{1f},
                AddBuffIndex = new []{ new []{11}},
                TargetCount = 4
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
            
            _skillTemplates.Add(9, new SkillTemplate(9, "始母之血", new []{10}));
            
            _skillTemplates.Add(11, new SkillTemplate(11, "创世女神", new []{12, 13}, new []{BattleLogic.SelectSelf}));
            
            _skillTemplates.Add(12, new SkillTemplate(12, "弑魂神枪", new [] {BattleLogic.LoopSkillNTime}, new []{BattleLogic.SelectAppendFront})
            {
                DamageIncreaseRate = 2.31f,
                LoopTime = 5,
                LoopSkillID = 16,
                LoopLoadPreLoadData = false,
                InitCd = 1,
                Cd = 10
            });
            
            _skillTemplates.Add(16, new SkillTemplate(16, "弑魂神枪Condition", new [] {BattleLogic.SoulSpearCondition}));
            
            
            _skillTemplates.Add(13, new SkillTemplate(13, "弑魂神枪I", new [] {BattleLogic.TryAddBuffInBuffList, BattleLogic.TryAddHolyFireWithCondition, BattleLogic.NormalAttack, BattleLogic.ResetCriticalRate}, null)
            {
                DamageIncreaseRate = 2.31f,
                AddBuffPossibility = new []{0.5f},
                AddBuffIndex = new []{ new []{5}},
            });
            
            _skillTemplates.Add(17, new SkillTemplate(17, "弑魂神枪II", new [] {BattleLogic.TryAddBuffInBuffList, BattleLogic.NormalAttack, BattleLogic.ResetParentSkillCd}, null)
            {
                DamageIncreaseRate = 2.81f,
                AddBuffPossibility = new []{0.55f, 0.5f},
                AddBuffIndex = new []{ new []{5}, new []{16}},
                ParentSkillID = 12
            });
            
            _skillTemplates.Add(14, new SkillTemplate(14, "焚天红莲", new []{14, 15}, new []{BattleLogic.SelectSelf}));
            
            _skillTemplates.Add(15, new SkillTemplate(15, "焚天红莲", new [] {BattleLogic.NormalAttack, BattleLogic.TryAddBuffInBuffList})
            {
                DamageIncreaseRate = 0.8f,
                AddBuffPossibility = new []{1f},
                AddBuffIndex = new []{ new []{5}},
                DamageDonePriority = Types.DamageDonePriority.BurntLotus
            });
            
            _skillTemplates.Add(18, new SkillTemplate(18, "圣境化莲", new []{18, 20}, new []{BattleLogic.SelectSelf}));
            
            _skillTemplates.Add(19, new SkillTemplate(19, "肉身成圣", new []{21, 22, 23}, new []{BattleLogic.SelectSelf}));
            
            _skillTemplates.Add(20, new SkillTemplate(20, "肉身成圣", new []{24}));
            
            _skillTemplates.Add(21, new SkillTemplate(21, "山川盛泽", new []{BattleLogic.MountainRiverCondition}, new []{BattleLogic.SelectSelf})
            {
                Cd = 2,
                InitCd = 1,
            });
            
            _skillTemplates.Add(22, new SkillTemplate(22, "川I", new []{BattleLogic.TryAddBuffInBuffList}, new []{BattleLogic.SelectAllTeammate, BattleLogic.DeselectSelfFromPrevious, BattleLogic.SelectHighestAttackFromPrevious, BattleLogic.SelectSelf}, new []{BattleLogic.CallBackExecuteNextSkillPlayableWithInput})
            {
                NextSkillID = 24,
                AddBuffIndex = new []{new []{25}},
                AddBuffPossibility = new []{1f},
            });
            
            _skillTemplates.Add(24, new SkillTemplate(24, "川II", new []{BattleLogic.Mountain}, new []{BattleLogic.SelectAppendBack, BattleLogic.CalculateTotalDamageToEach})
            {
                ValueChangeRate = 4.5f
            });
            
            _skillTemplates.Add(23, new SkillTemplate(23, "山III", new []{BattleLogic.NormalAttack}, new []{BattleLogic.SelectAppendFront, BattleLogic.SelectAppendMid}));
            
            _skillTemplates.Add(25, new SkillTemplate(25, "万象更新", new []{21, 22, 26, 27}, new []{BattleLogic.SelectSelf}));
            
            _skillTemplates.Add(26, new SkillTemplate(26, "星阳流光", new []{BattleLogic.TryAddBuffForTest}, new []{BattleLogic.SelectSelf})
            {
                Cd = 2,
                InitCd = 1
            });
            
            _skillTemplates.Add(27, new SkillTemplate(27, "炽阳", new []{BattleLogic.NormalAttack, BattleLogic.RemoveAllPositiveBuffByChance, BattleLogic.CopyNegativeBuffByTimes}, new []{BattleLogic.SelectAllEnemy})
            {
                DamageIncreaseRate = 1.5f,
                CurrentBuffRemovePriority = Types.BuffRemovePriority.NormalRemove,
                AddBuffPossibility = new[]{0.5f},
                LoopTime = 1,
                BuffNumberLimit = 3
            });
            // _skillTemplates.Add(28, new SkillTemplate(28, "星雨", new []{BattleLogic.NormalAttack, BattleLogic.RemoveAllPositiveBuffByChance}, new []{BattleLogic.SelectAllEnemy})
            // {
            //     DamageIncreaseRate = 7.52f,
            //     AddBuffIndex = new []{}
            // });

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