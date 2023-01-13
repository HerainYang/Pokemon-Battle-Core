using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Enum;
using Managers.BattleMgrComponents.BattlePlayables.Skills;
using UnityEngine;

namespace Managers.BattleMgrComponents.PokemonLogic
{
    public class PokemonMgr
    {
        private static PokemonMgr _instance;
        private Dictionary<int, PokemonBasicInfo> _pokemonConfig;
        private Dictionary<int, AttributesTemplate> _attributesConfig;

        private Dictionary<int, CommonSkillTemplate> _skillConfig;
        private Dictionary<int, CommonSkillTemplate> _buffConfig;

        private double[][] _typeChart;


        public static PokemonMgr Instance
        {
            get { return _instance ??= new PokemonMgr(); }
        }

        private void InitTypeChart()
        {
            _typeChart = new[]
            {
                new[] { 1, 1, 1, 1, 1, 0.5, 1, 0, 0.5, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                new[] { 2, 1, 0.5, 0.5, 1, 2, 0.5, 0, 2, 1, 1, 1, 1, 0.5, 2, 1, 2, 0.5 },
                new[] { 1, 2, 1, 1, 1, 0.5, 2, 1, 0.5, 1, 1, 2, 0.5, 1, 1, 1, 1, 1 },
                new[] { 1, 1, 1, 0.5, 0.5, 0.5, 1, 0.5, 0, 1, 1, 2, 1, 1, 1, 1, 1, 2 },
                new[] { 1, 1, 0, 2, 1, 2, 0.5, 1, 2, 2, 1, 0.5, 2, 1, 1, 1, 1, 1 },
                new[] { 1, 0.5, 2, 1, 0.5, 1, 2, 1, 0.5, 2, 1, 1, 1, 1, 2, 1, 1, 1 },
                new[] { 1, 0.5, 0.5, 0.5, 1, 1, 1, 0.5, 0.5, 0.5, 1, 2, 1, 2, 1, 1, 2, 0.5 },
                new[] { 0, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, 0.5, 1 },
                new[] { 1, 1, 1, 1, 1, 2, 1, 1, 0.5, 0.5, 0.5, 1, 0.5, 1, 2, 1, 1, 2 },
                new[] { 1, 1, 1, 1, 1, 0.5, 2, 1, 2, 0.5, 0.5, 2, 1, 1, 2, 0.5, 1, 1 },
                new[] { 1, 1, 1, 1, 2, 2, 1, 1, 1, 2, 0.5, 0.5, 1, 1, 1, 0.5, 1, 1 },
                new[] { 1, 1, 0.5, 0.5, 2, 2, 0.5, 1, 0.5, 0.5, 2, 0.5, 1, 1, 1, 0.5, 1, 1 },
                new[] { 1, 1, 2, 1, 0, 1, 1, 1, 1, 1, 2, 0.5, 0.5, 1, 1, 0.5, 1, 1 },
                new[] { 1, 2, 1, 2, 1, 1, 1, 1, 0.5, 1, 1, 1, 1, 0.5, 1, 1, 0, 1 },
                new[] { 1, 1, 2, 1, 2, 1, 1, 1, 0.5, 0.5, 0.5, 2, 1, 1, 0.5, 2, 1, 1 },
                new[] { 1, 1, 1, 1, 1, 1, 1, 1, 0.5, 1, 1, 1, 1, 1, 1, 2, 1, 0 },
                new[] { 1, 0.5, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 2, 1, 1, 0.5, 0.5 },
                new[] { 1, 2, 1, 0.5, 1, 1, 1, 1, 0.5, 0.5, 1, 1, 1, 1, 1, 2, 2, 1 }
            };
        }

        public PokemonMgr()
        {
            InitTypeChart();
            var buffManager = BuffMgr.Instance;
            _pokemonConfig = new Dictionary<int, PokemonBasicInfo>();
            _pokemonConfig.Add(4, new PokemonBasicInfo(4, "Charizard", 360, 293, 280, 348, 295, 328, PokemonType.Fire, new[] { 187, 7, 241, 20002 }, new[] { 94 }, "PokeImg[xiaohuolong]"));
            _pokemonConfig.Add(1, new PokemonBasicInfo(1, "Bulbasaur", 364, 289, 291, 328, 328, 284, PokemonType.Grass, new[] { 73, 202, 188, 235 }, new[] { 34 }, "PokeImg[wangba]"));
            _pokemonConfig.Add(25, new PokemonBasicInfo(25, "Pikachu", 324, 306, 229, 306, 284, 350, PokemonType.Electric, new[] { 417, 85, 237, 411 }, new[] { 9, 31 }, "PokeImg[pikaqiu]"));
            _pokemonConfig.Add(132, new PokemonBasicInfo(132, "Ditto", 300, 214, 214, 214, 214, 214, PokemonType.Normal, new[] { 144 }, new[] { 150 }, "PokeImg[baibianguai]"));
            _pokemonConfig.Add(878, new PokemonBasicInfo(878, "Copperajah", 448, 394, 260, 284, 260, 174, PokemonType.Steel, new[] { 442, 583, 91, 249 }, new[] { 134, 125 }, "PokeImg[daxiang]"));
            _pokemonConfig.Add(94, new PokemonBasicInfo(94, "Gengar", 324, 251, 240, 394, 273, 350, PokemonType.Ghost, new []{269, 68, 194, 247}, new []{130}, "PokeImg[genggui]"));

            // Attributes
            _attributesConfig = new Dictionary<int, AttributesTemplate>();
            _attributesConfig.Add(66, new AttributesTemplate("Blaze", new[] { 16 }));
            _attributesConfig.Add(94, new AttributesTemplate("Solar Power", new[] { 17, 18, 22, 23 }));
            _attributesConfig.Add(65, new AttributesTemplate("Overgrow", new[] { 19 }));
            _attributesConfig.Add(34, new AttributesTemplate("Chlorophyll", new[] { 20, 21, 24 }));
            _attributesConfig.Add(9, new AttributesTemplate("Static", new[] { 25 }));
            _attributesConfig.Add(31, new AttributesTemplate("Lightning Rod", new[] { 26 }));
            _attributesConfig.Add(7, new AttributesTemplate("Limber", new[] { 27 }));
            _attributesConfig.Add(150, new AttributesTemplate("Imposter", new[] { 28 }));
            _attributesConfig.Add(134, new AttributesTemplate("Heavy Metal", new[] { 1000 }));
            _attributesConfig.Add(125, new AttributesTemplate("Sheer Force", new[] { 29 }));
            
            _attributesConfig.Add(130, new AttributesTemplate("Cursed Body", new []{ 30 }));

            // skills
            LoadSkill();
            
            //buffs
            LoadBuff();
        }

        private void LoadSkill()
        {
            _skillConfig = new Dictionary<int, CommonSkillTemplate>();
            CommonSkillTemplate tempCommonSkill;

            {
                tempCommonSkill = new CommonSkillTemplate("Belly Drum", 187, PokemonType.Normal, SkillType.Status, 10, SkillTargetType.Self, new[] { BattleLogic.TryDamageMaxHpByPercentage, BattleLogic.TryChangePokemonStat })
                {
                    PercentageDamage = 0.5f,
                    PokemonStatType = new[] { PokemonStat.Attack },
                    PokemonStatPoint = new[] { 6 }
                };
                _skillConfig.Add(187, tempCommonSkill);

                tempCommonSkill = new CommonSkillTemplate("Fire Punch", 7, PokemonType.Fire, SkillType.Physical, 15, SkillTargetType.OneEnemy, new[] { BattleLogic.TryApplyDamage })
                {
                    Power = 75,
                    Accuracy = 100,
                };
                _skillConfig.Add(7, tempCommonSkill);

                tempCommonSkill = new CommonSkillTemplate("Focus Punch", 264, PokemonType.Fighting, SkillType.Physical, 5, 20, SkillTargetType.OneEnemy, new[] { BattleLogic.FocusPunchCharge });
                _skillConfig.Add(264, tempCommonSkill);
                tempCommonSkill = new CommonSkillTemplate("Focus Punch Attack", 10264, PokemonType.Fighting, SkillType.Physical, -3, 20, SkillTargetType.OneEnemy, new[] { BattleLogic.TryApplyDamage })
                {
                    Power = 150,
                    Accuracy = 100
                };
                _skillConfig.Add(10264, tempCommonSkill);

                tempCommonSkill = new CommonSkillTemplate("Void Skill For Test", 10000, PokemonType.Normal, SkillType.Physical, 15, SkillTargetType.OneEnemy, new[] { BattleLogic.OnlyForTest });
                _skillConfig.Add(10000, tempCommonSkill);

                tempCommonSkill = new CommonSkillTemplate("Test Poison", 10001, PokemonType.Poison, SkillType.Status, 15, SkillTargetType.OneEnemy, new[] { BattleLogic.TryAddBuffByProb })
                {
                    SpecialEffectProb = 1,
                    AddBuffID = 0
                };
                _skillConfig.Add(10001, tempCommonSkill);

                tempCommonSkill = new CommonSkillTemplate("Protect", 182, PokemonType.Normal, SkillType.Status, 4, 15, SkillTargetType.Self, new[] { BattleLogic.TryAddGuard });
                _skillConfig.Add(182, tempCommonSkill);

                tempCommonSkill = new CommonSkillTemplate("Leech Seed", 73, PokemonType.Grass, SkillType.Status, 10, SkillTargetType.OneEnemy, new[] { BattleLogic.TryAddLeechSeed })
                {
                    SpecialEffectProb = 1
                };
                _skillConfig.Add(73, tempCommonSkill);

                tempCommonSkill = new CommonSkillTemplate("Giga Drain", 202, PokemonType.Grass, SkillType.Special, 10, SkillTargetType.OneEnemy, new[] { BattleLogic.TryGigaDrain })
                {
                    Power = 75,
                    Accuracy = 100
                };
                _skillConfig.Add(202, tempCommonSkill);

                tempCommonSkill = new CommonSkillTemplate("Sludge Bomb", 188, PokemonType.Poison, SkillType.Special, 10, SkillTargetType.OneEnemy, new[] { BattleLogic.TryApplyDamage, BattleLogic.TryAddBuffByProb })
                {
                    AddBuffID = 0,
                    Power = 90,
                    Accuracy = 100,
                    SpecialEffectProb = 0.3f
                };
                _skillConfig.Add(188, tempCommonSkill);

                tempCommonSkill = new CommonSkillTemplate("Synthesis", 235, PokemonType.Grass, SkillType.Physical, 5, SkillTargetType.Self, new[] { BattleLogic.TrySynthesis })
                {
                    PercentageDamage = 0.5f
                };
                _skillConfig.Add(235, tempCommonSkill);

                tempCommonSkill = new CommonSkillTemplate("Sunny Day", 241, PokemonType.Fire, SkillType.Status, 5, SkillTargetType.Self, new[] { BattleLogic.TryAddWeather })
                {
                    WeatherType = Weather.HarshSunlight
                };
                _skillConfig.Add(241, tempCommonSkill);

                tempCommonSkill = new CommonSkillTemplate("Nasty Plot", 417, PokemonType.Ghost, SkillType.Status, 20, SkillTargetType.Self, new[] { BattleLogic.TryChangePokemonStat })
                {
                    PokemonStatPoint = new[] { 2 },
                    PokemonStatType = new[] { PokemonStat.SpecialAttack }
                };
                _skillConfig.Add(417, tempCommonSkill);

                tempCommonSkill = new CommonSkillTemplate("Thunderbolt", 85, PokemonType.Electric, SkillType.Special, 15, SkillTargetType.OneEnemy, new[] { BattleLogic.TryApplyDamage, BattleLogic.TryAddParalysis })
                {
                    SpecialEffectProb = 0.1f,
                    Accuracy = 100,
                    Power = 90
                };
                _skillConfig.Add(85, tempCommonSkill);

                tempCommonSkill = new CommonSkillTemplate("Hidden Power", 237, PokemonType.Normal, SkillType.Special, 15, SkillTargetType.OneEnemy, new[] { BattleLogic.HiddenPower })
                {
                    Accuracy = 100,
                    Power = 50
                };
                _skillConfig.Add(237, tempCommonSkill);

                tempCommonSkill = new CommonSkillTemplate("Focus Blast", 411, PokemonType.Fighting, SkillType.Special, 5, SkillTargetType.OneEnemy, new[] { BattleLogic.TryApplyDamage, BattleLogic.ProbChangeStat })
                {
                    Accuracy = 70,
                    Power = 120,
                    SpecialEffectProb = 0.1f,
                    PokemonStatPoint = new[] { -1 },
                    PokemonStatType = new[] { PokemonStat.SpecialDefence }
                };
                _skillConfig.Add(411, tempCommonSkill);

                tempCommonSkill = new CommonSkillTemplate("Transform", 144, PokemonType.Normal, SkillType.Status, 10, SkillTargetType.OneEnemy, new[] { BattleLogic.Transform });
                _skillConfig.Add(144, tempCommonSkill);

                tempCommonSkill = new CommonSkillTemplate("Iron Head", 442, PokemonType.Steel, SkillType.Physical, 15, SkillTargetType.OneEnemy, new[] { BattleLogic.TryApplyDamage, BattleLogic.TryAddBuffByProb })
                {
                    Accuracy = 100,
                    Power = 85,
                    SpecialEffectProb = 1f,
                    AddBuffID = 11
                };
                _skillConfig.Add(442, tempCommonSkill);

                tempCommonSkill = new CommonSkillTemplate("Play Rough", 583, PokemonType.Fairy, SkillType.Physical, 10, SkillTargetType.OneEnemy, new[] { BattleLogic.TryApplyDamage, BattleLogic.TryPlayRoughDeBuff })
                {
                    Power = 90,
                    Accuracy = 90,
                    SpecialEffectProb = 0.2f
                };
                _skillConfig.Add(583, tempCommonSkill);

                tempCommonSkill = new CommonSkillTemplate("Dig", 91, PokemonType.Ground, SkillType.Physical, 10, SkillTargetType.OneEnemy, new[] { BattleLogic.SetHidePokemon });
                _skillConfig.Add(91, tempCommonSkill);
                tempCommonSkill = new CommonSkillTemplate("Dig Attack", 10091, PokemonType.Ground, SkillType.Physical, 10, SkillTargetType.OneEnemy, new[] { BattleLogic.SetPokemonAppear, BattleLogic.TryApplyDamage })
                {
                    Power = 80,
                    Accuracy = 100
                };
                _skillConfig.Add(10091, tempCommonSkill);

                tempCommonSkill = new CommonSkillTemplate("Rock Smash", 249, PokemonType.Fighting, SkillType.Physical, 15, SkillTargetType.OneEnemy, new[] { BattleLogic.TryApplyDamage, BattleLogic.ProbChangeStat })
                {
                    Accuracy = 100,
                    Power = 40,
                    SpecialEffectProb = 0.5f,
                    PokemonStatPoint = new[] { -1 },
                    PokemonStatType = new[] { PokemonStat.Defence }
                };
                _skillConfig.Add(249, tempCommonSkill);

                tempCommonSkill = new CommonSkillTemplate("Taunt", 269, PokemonType.Dark, SkillType.Status, 20, SkillTargetType.OneEnemy, new[] { BattleLogic.AddTaunt });
                _skillConfig.Add(269, tempCommonSkill);

                tempCommonSkill = new CommonSkillTemplate("Counter", 68, PokemonType.Fighting, SkillType.Status, 20, SkillTargetType.Self, new[] { BattleLogic.TryCounter });
                _skillConfig.Add(68, tempCommonSkill);

                tempCommonSkill = new CommonSkillTemplate("Destiny Bond", 194, PokemonType.Ghost, SkillType.Status, 5, SkillTargetType.OneEnemy, new[] { BattleLogic.TryDestinyBond });
                _skillConfig.Add(194, tempCommonSkill);

                tempCommonSkill = new CommonSkillTemplate("Shadow Ball", 247, PokemonType.Ghost, SkillType.Special, 15, SkillTargetType.OneEnemy, new[] { BattleLogic.TryApplyDamage, BattleLogic.ProbChangeStat })
                {
                    Power = 80,
                    Accuracy = 100,
                    SpecialEffectProb = 0.2f,
                    PokemonStatPoint = new[] { -1 },
                    PokemonStatType = new[] { PokemonStat.SpecialDefence }
                };
                _skillConfig.Add(247, tempCommonSkill);


                tempCommonSkill = new CommonSkillTemplate("Borrowed Time", 20001, PokemonType.Normal, SkillType.Status, 5, SkillTargetType.Self, new[] { BattleLogic.TryAddBuffDirect })
                {
                    AddBuffID = 31
                };
                _skillConfig.Add(20001, tempCommonSkill);

                tempCommonSkill = new CommonSkillTemplate("Kizuna", 20002, PokemonType.Normal, SkillType.Status, 5, SkillTargetType.OneEnemy, new[] { BattleLogic.TryAddBuffDirect })
                {
                    AddBuffID = 32
                };
                _skillConfig.Add(20002, tempCommonSkill);
            }
        }

        private void LoadBuff()
        {
            _buffConfig = new Dictionary<int, CommonSkillTemplate>();
            CommonSkillTemplate tempBuff;

            {
                tempBuff = new CommonSkillTemplate(0, "Poison", 8, BattleLogic.PoisonBuff, null, Constant.BuffExecutionTimeKey.EndOfRound)
                {
                    PercentageDamage = 0.25f
                };
                _buffConfig.Add(0, tempBuff);

                tempBuff = new CommonSkillTemplate(1, "Guard", 1, BattleLogic.GuardBuff, null, Constant.BuffExecutionTimeKey.BeforeTakingDamage);
                _buffConfig.Add(1, tempBuff);

                tempBuff = new CommonSkillTemplate(2, "Mold Breaker", 1, BattleLogic.MoldBreakerBuff, null, Constant.BuffExecutionTimeKey.BeforeApplyDamage);
                _buffConfig.Add(2, tempBuff);

                tempBuff = new CommonSkillTemplate(1000, "Void", 1, BattleLogic.ForTestBuff, null, Constant.BuffExecutionTimeKey.EndOfRound);
                _buffConfig.Add(1000, tempBuff);

                tempBuff = new CommonSkillTemplate(3, "Cancel Focus Charge", 1, BattleLogic.CancelSkillExecution, null, Constant.BuffExecutionTimeKey.AfterTakingDamage)
                {
                    TargetSkillID = 10264
                };
                _buffConfig.Add(3, tempBuff);

                tempBuff = new CommonSkillTemplate(4, "Check Weather End", Int32.MaxValue, null, BattleLogic.CheckWeatherEnd, Constant.BuffExecutionTimeKey.None);
                _buffConfig.Add(4, tempBuff);

                tempBuff = new CommonSkillTemplate(5, "Debut Add Weather Effect", Int32.MaxValue, BattleLogic.AddWeatherBuffAfterDebut, null, Constant.BuffExecutionTimeKey.AfterDebut);
                _buffConfig.Add(5, tempBuff);

                tempBuff = new CommonSkillTemplate(6, "Change Skill Damage By Source", Int32.MaxValue, BattleLogic.ChangeSkillDamageBySource, null, Constant.BuffExecutionTimeKey.CalculatingFinalDamage);
                _buffConfig.Add(6, tempBuff);

                tempBuff = new CommonSkillTemplate(7, "Leech Seed", 8, BattleLogic.LeechSeed, null, Constant.BuffExecutionTimeKey.EndOfRound)
                {
                    PercentageDamage = 1 / 8f
                };
                _buffConfig.Add(7, tempBuff);

                tempBuff = new CommonSkillTemplate(8, "Paralysis", 5, BattleLogic.IfBuffAllowMove, null, Constant.BuffExecutionTimeKey.BeforeMove)
                {
                    SpecialEffectProb = 0.25f,
                    MovePriority = MovePriority.Paralysis,
                    Message = " cannot move due to paralysis!"
                };
                _buffConfig.Add(8, tempBuff);

                tempBuff = new CommonSkillTemplate(9, "Paralysis", 5, BattleLogic.DecreaseSpeedByPercentage, BattleLogic.RecoverDecreaseSpeedByPercentage, Constant.BuffExecutionTimeKey.OnAddBuff)
                {
                    PokemonDataChangePercentage = 0.5f
                };
                _buffConfig.Add(9, tempBuff);

                tempBuff = new CommonSkillTemplate(10, "Cancel Transform", Int32.MaxValue, BattleLogic.CancelTransform, null, Constant.BuffExecutionTimeKey.BeforeWithdraw);
                _buffConfig.Add(10, tempBuff);

                tempBuff = new CommonSkillTemplate(11, "Flinch", 2, BattleLogic.IfBuffAllowMove, null, Constant.BuffExecutionTimeKey.BeforeMove)
                {
                    SpecialEffectProb = 1f,
                    Message = " cannot move due to Flinch!",
                    MovePriority = MovePriority.Flinch
                };
                _buffConfig.Add(11, tempBuff);

                tempBuff = new CommonSkillTemplate(12, "Cannot be targeted", Int32.MaxValue, BattleLogic.CannotBeTarget, null, Constant.BuffExecutionTimeKey.WhenGettingTarget);
                _buffConfig.Add(12, tempBuff);

                tempBuff = new CommonSkillTemplate(13, "Dig Attack", Int32.MaxValue, BattleLogic.AutoAddSkillPlayable, null, Constant.BuffExecutionTimeKey.BeforeRequirePokemonCommand)
                {
                    TargetSkillID = 10091
                };
                _buffConfig.Add(13, tempBuff);

                tempBuff = new CommonSkillTemplate(14, "Attack Skill Only", 4, BattleLogic.AttackSkillOnly, null, Constant.BuffExecutionTimeKey.BeforeMove)
                {
                    Message = " cannot move due to it can only use attack skill!"
                };
                _buffConfig.Add(14, tempBuff);

                tempBuff = new CommonSkillTemplate(15, "Forbidden Skill", Int32.MaxValue, BattleLogic.ForbiddenSkill, null, Constant.BuffExecutionTimeKey.BeforeMove)
                {
                    Message = " failed to use this skill!"
                };
                _buffConfig.Add(15, tempBuff);

                tempBuff = new CommonSkillTemplate(16, "Blaze", Int32.MaxValue, BattleLogic.DamageUpWhenHpDown, null, Constant.BuffExecutionTimeKey.CalculatingFinalDamage)
                {
                    Type = PokemonType.Fire
                };
                _buffConfig.Add(16, tempBuff);

                tempBuff = new CommonSkillTemplate(17, "Solar Power SA Up", Int32.MaxValue, BattleLogic.WeatherStatChange, null, Constant.BuffExecutionTimeKey.OnWeatherChange)
                {
                    ChangeStat = PokemonStat.SpecialAttack
                };
                _buffConfig.Add(17, tempBuff);

                tempBuff = new CommonSkillTemplate(22, "Cancel Solar Power", Int32.MaxValue, BattleLogic.RecoverStatAtWeatherEnd, null, Constant.BuffExecutionTimeKey.OnWeatherChange)
                {
                    ChangeStat = PokemonStat.SpecialAttack
                };
                _buffConfig.Add(22, tempBuff);

                tempBuff = new CommonSkillTemplate(23, "Solar Power SA Up On Debut", Int32.MaxValue, BattleLogic.WeatherStatChange, null, Constant.BuffExecutionTimeKey.AfterDebut)
                {
                    ChangeStat = PokemonStat.SpecialAttack
                };
                _buffConfig.Add(23, tempBuff);

                tempBuff = new CommonSkillTemplate(18, "Solar Power Hp Down", Int32.MaxValue, BattleLogic.WeatherMatchSetHp, null, Constant.BuffExecutionTimeKey.EndOfRound);
                _buffConfig.Add(18, tempBuff);

                tempBuff = new CommonSkillTemplate(19, "Overgrow", Int32.MaxValue, BattleLogic.DamageUpWhenHpDown, null, Constant.BuffExecutionTimeKey.CalculatingFinalDamage)
                {
                    Type = PokemonType.Grass
                };
                _buffConfig.Add(19, tempBuff);

                tempBuff = new CommonSkillTemplate(20, "Chlorophyll Speed Up", Int32.MaxValue, BattleLogic.WeatherStatChange, null, Constant.BuffExecutionTimeKey.OnWeatherChange)
                {
                    ChangeStat = PokemonStat.Speed
                };
                _buffConfig.Add(20, tempBuff);

                tempBuff = new CommonSkillTemplate(21, "Cancel Chlorophyll", Int32.MaxValue, BattleLogic.RecoverStatAtWeatherEnd, null, Constant.BuffExecutionTimeKey.OnWeatherChange)
                {
                    ChangeStat = PokemonStat.Speed
                };
                _buffConfig.Add(21, tempBuff);

                tempBuff = new CommonSkillTemplate(24, "Chlorophyll Speed Up On Debut", Int32.MaxValue, BattleLogic.WeatherStatChange, null, Constant.BuffExecutionTimeKey.AfterDebut)
                {
                    ChangeStat = PokemonStat.Speed
                };
                _buffConfig.Add(24, tempBuff);

                tempBuff = new CommonSkillTemplate(25, "Static", Int32.MaxValue, BattleLogic.StaticBuff, null, Constant.BuffExecutionTimeKey.AfterTakingDamage);
                _buffConfig.Add(25, tempBuff);

                tempBuff = new CommonSkillTemplate(26, "Lightning Rod", Int32.MaxValue, BattleLogic.TypeDamageResistantWithStatChange, null, Constant.BuffExecutionTimeKey.BeforeTakingDamage)
                {
                    BuffDamageApplyPriority = DamageApplyPriority.LightningRod,
                    Type = PokemonType.Electric,
                    PokemonStatType = new[] { PokemonStat.SpecialAttack },
                    PokemonStatPoint = new[] { 2 }
                };
                _buffConfig.Add(26, tempBuff);

                tempBuff = new CommonSkillTemplate(27, "Limber", Int32.MaxValue, BattleLogic.Limber, null, Constant.BuffExecutionTimeKey.BeforeAddBuff);
                _buffConfig.Add(27, tempBuff);

                tempBuff = new CommonSkillTemplate(28, "Imposter On Debut", Int32.MaxValue, BattleLogic.ImposterDebut, null, Constant.BuffExecutionTimeKey.AfterDebut);
                _buffConfig.Add(28, tempBuff);

                tempBuff = new CommonSkillTemplate(29, "Sheer Force", Int32.MaxValue, BattleLogic.SheerForce, null, Constant.BuffExecutionTimeKey.BeforeMove);
                _buffConfig.Add(29, tempBuff);

                tempBuff = new CommonSkillTemplate(30, "Cursed Body", Int32.MaxValue, BattleLogic.CursedBody, null, Constant.BuffExecutionTimeKey.AfterTakingDamage)
                {
                    SpecialEffectProb = 0.3f
                };
                _buffConfig.Add(30, tempBuff);

                tempBuff = new CommonSkillTemplate(31, "Borrowed Time", 5, BattleLogic.BorrowedTime, null, Constant.BuffExecutionTimeKey.CalculatingFinalDamage);
                _buffConfig.Add(31, tempBuff);

                tempBuff = new CommonSkillTemplate(32, "Kizuna", 5, BattleLogic.Kizuna, null, Constant.BuffExecutionTimeKey.AfterHealDone);
                _buffConfig.Add(32, tempBuff);
            }
        }

        public PokemonBasicInfo GetPokemonByID(int id)
        {
            return _pokemonConfig[id];
        }

        public AttributesTemplate GetAttributeByID(int id)
        {
            return _attributesConfig[id];
        }
        
        public CommonSkillTemplate GetSkillTemplateByID(int id)
        {
            return _skillConfig[id];
        }

        public double GetTypeResistance(int attackType, int defenseType)
        {
            return _typeChart[attackType][defenseType];
        }

        public CommonSkillTemplate GetBuffTemplateByID(int id)
        {
            return _buffConfig[id];
        }
    }
}