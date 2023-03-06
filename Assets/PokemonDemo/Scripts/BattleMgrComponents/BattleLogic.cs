using System;
using System.Collections.Generic;
using System.Linq;
using CoreScripts.BattleComponents;
using Cysharp.Threading.Tasks;
using Managers.BattleMgrComponents;
using PokemonDemo.Scripts.BattlePlayables.Skills;
using PokemonDemo.Scripts.BattlePlayer;
using PokemonDemo.Scripts.Enum;
using PokemonDemo.Scripts.Managers;
using PokemonDemo.Scripts.PokemonLogic;
using PokemonDemo.Scripts.PokemonLogic.BuffResults;
using PokemonDemo.Scripts.PokemonLogic.PokemonData;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

namespace PokemonDemo.Scripts.BattleMgrComponents
{
    public static class BattleLogic
    {
        //Common
        private static bool ProbTrigger(float prob)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            if (rand.NextDouble() < prob)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static double GetRandomDouble()
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            return rand.NextDouble();
        }

        private static int GetNumberFromAToB(int a, int b)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());
            return rand.Next(a, b);
        }


        //Basic Math Calculation

        private static int GetPokemonAttack(Pokemon source, bool criticalHit)
        {
            int attack = source.Attack;
            int statsChange = source.GetStatusChange(PokemonStat.Attack);
            if (criticalHit)
                statsChange = statsChange > 0 ? statsChange : 0;
            float changeRate = statsChange >= 0 ? (statsChange + 2f) / 2f : 2f / (math.abs(statsChange) + 2);
            Debug.Log("Change Rate: " + changeRate);
            attack = (int)(attack * changeRate);
            return attack;
        }

        private static int GetPokemonDefense(Pokemon source, bool criticalHit)
        {
            int defense = source.Defence;
            int statsChange = source.GetStatusChange(PokemonStat.Defence);
            if (criticalHit)
                statsChange = statsChange > 0 ? 0 : statsChange;
            float changeRate = statsChange >= 0 ? (statsChange + 2f) / 2f : 2f / (math.abs(statsChange) + 2);
            defense = (int)(defense * changeRate);
            return defense;
        }

        private static async UniTask<int> GetPokemonSpecialAttack(Pokemon source, bool criticalHit)
        {
            int attack = source.SpecialAttack;
            int statsChange = source.GetStatusChange(PokemonStat.SpecialAttack);
            if (criticalHit)
                statsChange = statsChange > 0 ? statsChange : 0;
            float changeRate = statsChange >= 0 ? (statsChange + 2f) / 2f : 2f / (math.abs(statsChange) + 2);
            attack = (int)(attack * changeRate);
            PokemonCommonResult result = new PokemonCommonResult
            {
                PokemonStat =
                {
                    [(int)PokemonStat.SpecialAttack] = attack
                }
            };
            result = (PokemonCommonResult)await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.GettingSpecialAttack, result, source);
            return result.PokemonStat[(int)PokemonStat.SpecialAttack];
        }

        private static int GetPokemonSpecialDefense(Pokemon source, bool criticalHit)
        {
            int defense = source.SpecialDefence;
            int statsChange = source.GetStatusChange(PokemonStat.SpecialDefence);
            if (criticalHit)
                statsChange = statsChange > 0 ? 0 : statsChange;
            float changeRate = statsChange >= 0 ? (statsChange + 2f) / 2f : 2f / (math.abs(statsChange) + 2);
            defense = (int)(defense * changeRate);
            return defense;
        }

        public static int GetPokemonSpeed(Pokemon source)
        {
            int speed = source.Speed;
            int statsChange = source.GetStatusChange(PokemonStat.Speed);
            float changeRate = statsChange >= 0 ? (statsChange + 2f) / 2f : 2f / (math.abs(statsChange) + 2);
            speed = (int)(speed * changeRate);
            return speed;
        }

        private static int BaseDamageCalculate(int attack, int defense, int power, int level)
        {
            return (int)Math.Ceiling((2f * level + 10f) / 250f * attack / defense * power + 2f);
        }

        private static int TypeDamageCalculate(float damage, CommonSkillTemplate template, Pokemon source, Pokemon target)
        {
            float sameTypeAttackBonus = source.Type == template.Type ? 1.5f : 1f;
            return (int)(damage * PokemonMgr.Instance.GetTypeResistance((int)template.Type, (int)target.Type) * sameTypeAttackBonus);
        }

        private static async UniTask<Tuple<SkillEffect, PokemonCommonResult>> DamageCalculate(Pokemon source, Pokemon target, CommonSkillTemplate template)
        {
            bool criticalHit = await CriticalHitCalculate(source.GetStatusChange(PokemonStat.CriticalHit) + template.CriticalRate);
            SkillEffect effect = SkillEffect.SuperEffective;
            var result = new PokemonCommonResult
            {
                Power = template.Power
            };
            result = (PokemonCommonResult)await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.CalculatingSkillPower, result);

            int damage = 0;
            if (template.SkillType == SkillType.Physical)
            {
                damage = BaseDamageCalculate(GetPokemonAttack(source, criticalHit), GetPokemonDefense(target, criticalHit), result.Power, source.Level);
            }

            if (template.SkillType == SkillType.Special)
            {
                damage = BaseDamageCalculate(await GetPokemonSpecialAttack(source, criticalHit), GetPokemonSpecialDefense(target, criticalHit), result.Power, source.Level);
            }

            damage = TypeDamageCalculate(damage, template, source, target);
            damage = criticalHit ? (int)(damage * Constant.GameConfig.CriticalDamageIncrease) : damage;

            result.Damage = -damage;
            result.STemplate = template;
            result.SkillSource = source;
            result.SkillTarget = target;
            result = (PokemonCommonResult)await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.CalculatingFinalDamage, result);

            if (Math.Abs(PokemonMgr.Instance.GetTypeResistance((int)template.Type, (int)target.Type) - 0.5) < 0.001)
            {
                effect = SkillEffect.NotVeryEffective;
            }

            if (Math.Abs(PokemonMgr.Instance.GetTypeResistance((int)template.Type, (int)target.Type) - 0) < 0.001)
            {
                effect = SkillEffect.NotEffective;
            }

            if (Math.Abs(PokemonMgr.Instance.GetTypeResistance((int)template.Type, (int)target.Type) - 1) < 0.001)
            {
                effect = SkillEffect.Effective;
            }

            if (Math.Abs(PokemonMgr.Instance.GetTypeResistance((int)template.Type, (int)target.Type) - 2) < 0.001)
            {
                effect = SkillEffect.SuperEffective;
            }

            if (criticalHit)
            {
                effect = SkillEffect.CriticalDamage;
            }

            return new Tuple<SkillEffect, PokemonCommonResult>(effect, result);
        }

        private static async UniTask<bool> HitAccuracyCalculate(Pokemon source, Pokemon target, CommonSkillTemplate template)
        {
            //A=(B*E*F*G)
            int b = (int)Math.Floor(255f * template.Accuracy / 100f);
            var result = new PokemonCommonResult
            {
                Accuracy = b,
                MustHit = false
            };
            result = (PokemonCommonResult)await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.GettingSkillAccuracy, result);
            if (result.MustHit)
            {
                return true;
            }

            b = (int)result.Accuracy;

            int diff = source.GetStatusChange(PokemonStat.Accuracy) - target.GetStatusChange(PokemonStat.Evasion);
            diff = diff > 6 ? 6 : (diff < -6 ? -6 : diff);
            float f = diff >= 0 ? (diff + 3f) / 3f : 3f / (math.abs(diff) + 3f);
            float accuracy = b * f;
            result.Accuracy = accuracy;
            result.MustHit = false;
            result = (PokemonCommonResult)await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.CalculatingHit, result);
            if (result.MustHit)
            {
                return true;
            }

            int range = (int)(result.Accuracy / 255 * 100);
            Debug.Log("[BattleLogic] Accuracy range is " + range);
            return GetNumberFromAToB(0, 100) < range;
        }

        private static async UniTask<bool> CriticalHitCalculate(int criticalValue)
        {
            criticalValue = criticalValue > 3 ? 3 : (criticalValue < 0 ? 0 : criticalValue);
            float criticalRate = (float)(criticalValue == 0 ? 1f / 24f : 1f / Math.Pow(2, 3 - criticalValue));
            bool isCritical = GetRandomDouble() < criticalRate;
            var result = new PokemonCommonResult
            {
                CriticalRate = criticalRate,
                IsCritical = isCritical
            };
            result = (PokemonCommonResult)await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.CalculatingCriticalDamage, result);

            return result.IsCritical;
        }

        // if damage can be blocked
        private static async UniTask<bool> CanApplyDamage(Pokemon source, Pokemon target, CommonSkillTemplate template)
        {
            var task0 = BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.BeforeApplyDamage, new PokemonCommonResult(){Priority = (int)DamageApplyPriority.Normal, ShouldSuccess = true, Message = null, STemplate = template}, source);
            var task1 = BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.BeforeTakingDamage, new PokemonCommonResult(){Priority = (int)DamageApplyPriority.Normal, ShouldSuccess = true, Message = null, STemplate = template}, target);
            var (sourceResult, targetResult) = await UniTask.WhenAll(task0, task1);
            PokemonCommonResult result = (PokemonCommonResult)(((PokemonCommonResult)sourceResult).Priority > ((PokemonCommonResult)targetResult).Priority ? sourceResult : targetResult);
            bool success = result.ShouldSuccess;

            if (result.Message != null)
            {
                await BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(result.Message);
            }

            return success;
        }

        private static async UniTask<PokemonBuffRecorder> TryAddBuff(Pokemon source, Pokemon target, CommonSkillTemplate template, int buffId)
        {
            var buffTemplate = PokemonMgr.Instance.GetBuffTemplateByID(buffId);
            if (ProbTrigger(template.SpecialEffectProb))
            {
                if (BuffMgr.Instance.ExistActiveBuff(target, PokemonMgr.Instance.GetBuffTemplateByID(buffId)))
                {
                    await BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(target.GetName() + " already get " + buffTemplate.Name);
                    return null;
                }

                await BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(target.GetName() + " get " + buffTemplate.Name);
                PokemonBuffRecorder recorder = (PokemonBuffRecorder)await BuffMgr.Instance.AddBuff(source, target, buffId);
                return recorder;
            }

            return null;
        }

        private static async UniTask<PokemonBuffRecorder> MustAddBuff(Pokemon source, Pokemon target, CommonSkillTemplate template, int buffId)
        {
            var buffTemplate = PokemonMgr.Instance.GetBuffTemplateByID(buffId);
            if (BuffMgr.Instance.ExistActiveBuff(target, PokemonMgr.Instance.GetBuffTemplateByID(buffId)))
            {
                await BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(target.GetName() + " already get " + buffTemplate.Name);
                return null;
            }

            PokemonBuffRecorder recorder = (PokemonBuffRecorder)await BuffMgr.Instance.AddBuff(source, target, buffId);
            return recorder;
        }

        private static readonly Func<Pokemon, Pokemon, float, CommonSkillTemplate, UniTask<int>> DamageMaxHpByPercentage = async (source, target, percentage, template) =>
        {
            int damage = -(int)Math.Ceiling((target.GetHpMax()) * (percentage));
            await TrySetHp(damage, source, target, template);
            return damage;
        };

        //if source is null, it means it is a system/environment command, it will always return the target
        public static async UniTask<List<Pokemon>> FindTarget(int[] indices, Pokemon source)
        {
            List<Pokemon> targets = new List<Pokemon>();
            foreach (var index in indices)
            {
                var target = BattleMgr.Instance.OnStagePokemon[index];
                if (target != null)
                {
                    PokemonCommonResult output = (PokemonCommonResult)await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.WhenGettingTarget, new PokemonCommonResult(), target);
                    if (source != null && output.CanBeTargeted != true && !Equals(target, source))
                    {
                        continue;
                    }

                    targets.Add(target);
                }
            }

            return targets;
        }

        private static async UniTask TrySetHp(int hpChange, Pokemon source, Pokemon target, CommonSkillTemplate template)
        {
            bool success = await target.ChangeHp(hpChange);
            if (!success)
                return;
            PokemonCommonResult cResult = new PokemonCommonResult
            {
                SkillSource = source,
                SkillTarget = target,
                Damage = hpChange,
                STemplate = template
            };
            if (hpChange < 0)
            {
                await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.AfterTakingDamage, cResult, target);
            }

            if (hpChange > 0)
            {
                await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.AfterHealDone, cResult, target);
                await BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(target.GetName() + " 's HP recover  " + hpChange);
            }
        }

        //Skills


        public struct Skill
        {
            internal static async UniTask PrintSkillEffectResult(SkillEffect result)
            {
                switch (result)
                {
                    case SkillEffect.SuperEffective:
                        await BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText("Supper effective");
                        break;
                    case SkillEffect.Effective:
                        await BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText("Effective");
                        break;
                    case SkillEffect.NotVeryEffective:
                        await BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText("Not very effective");
                        break;
                    case SkillEffect.NotEffective:
                        await BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText("Not effective");
                        break;
                    case SkillEffect.CriticalDamage:
                        await BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText("Critical hit!");
                        break;
                }
            }

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> OnlyForTest = async (input, source, _, _) =>
            {
                var pokemonSource = (Pokemon)source;
                await BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(pokemonSource.GetName() + " doesn't do anything");
                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> TryDamageMaxHpByPercentage = async (input, source, target, template) =>
            {
                var skillResult = (PokemonCommonResult)input;
                var pokemonSource = (Pokemon)source;
                var pokemonTarget = (Pokemon)target;
                var skillTemplate = (CommonSkillTemplate)template;
                int damage = await DamageMaxHpByPercentage(pokemonSource, pokemonTarget, skillTemplate.PercentageDamage, skillTemplate);
                skillResult.Damage = damage;
                return input;
            };

            private static readonly Func<Pokemon, Pokemon, CommonSkillTemplate, UniTask<PokemonCommonResult>> ApplyDamage = async (source, target, template) =>
            {
                bool hit = await HitAccuracyCalculate(source, target, template);
                if (!hit)
                {
                    await BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText("But it doesn't hit " + target.Name);
                    return new PokemonCommonResult() { ShouldContinueSkill = false };
                }

                var success = await CanApplyDamage(source, target, template);
                if (success)
                {
                    var result = await DamageCalculate(source, target, template);
                    await PrintSkillEffectResult(result.Item1);
                    await TrySetHp(result.Item2.Damage, source, target, template);
                    return result.Item2;
                }
                else
                {
                    await PrintSkillEffectResult(SkillEffect.NotEffective);
                    return new PokemonCommonResult() { ShouldContinueSkill = false };
                }
            };


            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> TryApplyDamage = async (input, source, target, template) =>
            {
                var skillResult = (PokemonCommonResult)input;
                var pokemonSource = (Pokemon)source;
                var pokemonTarget = (Pokemon)target;
                var skillTemplate = (CommonSkillTemplate)template;
                var result = await ApplyDamage(pokemonSource, pokemonTarget, skillTemplate);
                int damage = result.Damage;
                skillResult.Damage = damage;
                BattleMgr.Instance.BattleStack[^1].Damages ??= new Dictionary<Pokemon, int>();
                BattleMgr.Instance.BattleStack[^1].Damages.Add(pokemonTarget, damage);
                return result.ShouldContinueSkill ? input : null;
            };


            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> FocusPunchCharge = async (input, source, target, _) =>
            {
                var skillResult = (PokemonCommonResult)input;
                var pokemonSource = (Pokemon)source;
                var pokemonTarget = (Pokemon)target;

                await BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(pokemonSource.Name + " starts charging");
                await BuffMgr.Instance.AddBuff(source, source, 3);
                PokemonCommonResult skillPreLoadResult = new PokemonCommonResult()
                {
                    TargetsByIndices = new[] { BattleMgr.Instance.GetPokemonOnstagePosition(pokemonTarget) }
                };
                BattleMgr.Instance.LoadPokemonSkill(pokemonSource, PokemonMgr.Instance.GetSkillTemplateByID(10264), skillPreLoadResult);
                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> TryAddBuffByProb = async (input, source, target, template) =>
            {
                var pokemonSource = (Pokemon)source;
                var pokemonTarget = (Pokemon)target;
                var skillTemplate = (CommonSkillTemplate)template;
                var recorder = await TryAddBuff(pokemonSource, pokemonTarget, skillTemplate, skillTemplate.AddBuffID);
                return recorder != null ? input : null;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> TryAddGuard = async (input, source, target, _) =>
            {
                var skillResult = (PokemonCommonResult)input;
                var pokemonSource = (Pokemon)source;
                var pokemonTarget = (Pokemon)target;
                if (BuffMgr.Instance.ExistActiveBuff(pokemonTarget,  PokemonMgr.Instance.GetBuffTemplateByID(1)))
                {
                    await BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(pokemonTarget.GetName() + " already guard");
                    return null;
                }

                if (BattleMgr.Instance.IsLastSkill())
                {
                    await BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(pokemonTarget.GetName() + " fail to guard it self");
                    return null;
                }

                await BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(pokemonTarget.GetName() + " guard itself");
                await BuffMgr.Instance.AddBuff(source, target, 1);
                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> TryAddBuffDirect = async (input, source, target, template) =>
            {
                var pokemonSource = (Pokemon)source;
                var pokemonTarget = (Pokemon)target;
                var skillTemplate = (CommonSkillTemplate)template;
                await MustAddBuff(pokemonSource, pokemonTarget, skillTemplate, skillTemplate.AddBuffID);
                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> TryAddWeather = async (input, _, target, template) =>
            {
                var skillTemplate = (CommonSkillTemplate)template;

                int weatherLength = 3;

                var result = await BattleMgr.Instance.SetWeather(skillTemplate.WeatherType);
                if (result)
                {
                    PokemonBuffRecorder recorder = await BuffMgr.Instance.AddBuff(null, null, 4, false, true);
                    recorder.EffectLastRound = weatherLength;
                    //debut pokemon add buff
                    recorder = await BuffMgr.Instance.AddBuff(null, null, 5, false, true);
                    recorder.EffectLastRound = weatherLength;
                    recorder.WeatherType = skillTemplate.WeatherType;

                    int[] indices = Enumerable.Range(0, BattleMgr.Instance.OnStagePokemon.Length).ToArray();
                    List<Pokemon> pokemons = await FindTarget(indices, null);
                    foreach (var pokemon in pokemons)
                    {
                        switch (skillTemplate.WeatherType)
                        {
                            case Weather.HarshSunlight:
                                var buff = await BuffMgr.Instance.AddBuff(null, pokemon, 6, false, true);
                                buff.EffectLastRound = weatherLength;
                                buff.SType = PokemonType.Fire;
                                buff.ChangeFactor = 1.5f;

                                buff = await BuffMgr.Instance.AddBuff(null, pokemon, 6, false, true);
                                buff.EffectLastRound = weatherLength;
                                buff.SType = PokemonType.Water;
                                buff.ChangeFactor = 0.5f;
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    }

                    return input;
                }

                return null;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> TryChangePokemonStat = async (input, _, target, template) =>
            {
                var skillResult = (PokemonCommonResult)input;
                var pokemonTarget = (Pokemon)target;

                var skillTemplate = (CommonSkillTemplate)template;
                for (int i = 0; i < skillTemplate.PokemonStatType.Length; i++)
                {
                    pokemonTarget.SetStatusChange(skillTemplate.PokemonStatType[i], skillTemplate.PokemonStatPoint[i]);
                    await BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(pokemonTarget.Name + "'s " + skillTemplate.PokemonStatType[i] + " change " + skillTemplate.PokemonStatPoint[i]);
                }

                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> TryAddLeechSeed = async (input, source, target, template) =>
            {
                var skillResult = (PokemonCommonResult)input;
                var pokemonSource = (Pokemon)source;
                var pokemonTarget = (Pokemon)target;
                var skillTemplate = (CommonSkillTemplate)template;
                var recorder = await TryAddBuff(pokemonSource, pokemonTarget, skillTemplate, 7);
                if (recorder != null)
                {
                    await BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText("A seed is planted on " + pokemonTarget.GetName());
                    recorder.SourceIndex = BattleMgr.Instance.GetPokemonOnstagePosition(pokemonSource);
                    return input;
                }

                return null;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> RecoverFromPreviousDamage = async (input, source, target, template) =>
            {
                var skillResult = (PokemonCommonResult)input;
                var pokemonSource = (Pokemon)source;
                var pokemonTarget = (Pokemon)target;
                var skillTemplate = (CommonSkillTemplate)template;
                int recoverHp = (int)(-skillResult.Damage * 0.5f);

                await TrySetHp(recoverHp, pokemonSource, pokemonSource, skillTemplate);
                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> TrySynthesis = async (input, source, _, template) =>
            {
                var pokemonSource = (Pokemon)source;
                var skillTemplate = (CommonSkillTemplate)template;
                float recoverPercentage = skillTemplate.PercentageDamage;
                if (BattleMgr.Instance.GetWeather() == Weather.HarshSunlight)
                {
                    recoverPercentage = 2 / 3f;
                }

                await TrySetHp((int)(pokemonSource.HpMax * recoverPercentage), pokemonSource, pokemonSource, skillTemplate);
                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> TryAddParalysis = async (input, source, target, template) =>
            {
                var skillResult = (PokemonCommonResult)input;
                var pokemonSource = (Pokemon)source;
                var pokemonTarget = (Pokemon)target;
                var skillTemplate = (CommonSkillTemplate)template;
                if (ProbTrigger(skillTemplate.SpecialEffectProb))
                {
                    if (BuffMgr.Instance.ExistActiveBuff(pokemonTarget,  PokemonMgr.Instance.GetBuffTemplateByID(8)) || BuffMgr.Instance.ExistActiveBuff(pokemonTarget,  PokemonMgr.Instance.GetBuffTemplateByID(9)))
                    {
                        await BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(pokemonTarget.GetName() + " already get paralysis!");
                        return null;
                    }

                    var task0 = await MustAddBuff(pokemonSource, pokemonTarget, skillTemplate, 8);
                    var task1 = await MustAddBuff(pokemonSource, pokemonTarget, skillTemplate, 9);
                    if (task0 == null && task1 == null)
                    {
                        return null;
                    }

                    await BattleMgr.Instance.SetCommandText(pokemonTarget.Name + " get paralysis");
                    return input;
                }

                return null;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> HiddenPower = async (input, source, target, template) =>
            {
                var skillResult = (PokemonCommonResult)input;
                var pokemonSource = (Pokemon)source;
                var pokemonTarget = (Pokemon)target;
                var skillTemplate = (CommonSkillTemplate)template;
                PokemonType targetType = pokemonSource.Type;
                if (math.abs(PokemonMgr.Instance.GetTypeResistance((int)targetType, (int)pokemonTarget.Type) - 2) > 0.01)
                {
                    for (int i = 0; i < 13; i++)
                    {
                        if (math.abs(PokemonMgr.Instance.GetTypeResistance(i, (int)pokemonTarget.Type) - 2) < 0.01)
                        {
                            targetType = (PokemonType)i;
                            break;
                        }
                    }
                }

                CommonSkillTemplate adaptiveTemplate = CommonSkillTemplate.CopySkill(skillTemplate);
                adaptiveTemplate.Type = targetType;
                var result = await ApplyDamage(pokemonSource, pokemonTarget, adaptiveTemplate);
                return result.ShouldContinueSkill ? input : null;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> ProbChangeStat = async (input, _, target, template) =>
            {
                var pokemonTarget = (Pokemon)target;
                var skillTemplate = (CommonSkillTemplate)template;
                if (ProbTrigger(skillTemplate.SpecialEffectProb))
                {
                    for (int i = 0; i < skillTemplate.PokemonStatType.Length; i++)
                    {
                        pokemonTarget.SetStatusChange(skillTemplate.PokemonStatType[i], skillTemplate.PokemonStatPoint[i]);
                        await BattleMgr.Instance.SetCommandText(pokemonTarget.Name + "'s " + skillTemplate.PokemonStatType[i] + " change " + skillTemplate.PokemonStatPoint[i]);
                    }
                }

                await UniTask.Yield();
                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> Transform = async (input, source, target, _) =>
            {
                var pokemonSource = (Pokemon)source;
                var pokemonTarget = (Pokemon)target;

                await BattleMgr.Instance.BattleScenePanelTwoPlayerUI.GetPokemonBattleInfo(BattleMgr.Instance.GetPokemonOnstagePosition(pokemonSource)).SetPokemonImg(pokemonTarget.ImageKey);
                BattleMgr.Instance.BattleScenePanelTwoPlayerUI.GetPokemonBattleInfo(BattleMgr.Instance.GetPokemonOnstagePosition(pokemonSource)).SetAttributeText(pokemonTarget.Attribute.Name);

                pokemonSource.Attack = pokemonTarget.Attack;
                pokemonSource.Defence = pokemonTarget.Defence;
                pokemonSource.SpecialAttack = pokemonTarget.SpecialAttack;
                pokemonSource.SpecialDefence = pokemonTarget.SpecialDefence;
                pokemonSource.Speed = pokemonTarget.Speed;
                pokemonSource.HpMax = pokemonTarget.HpMax;
                await pokemonSource.ChangeHp(0);

                BattleMgr.Instance.PlayerLogStore(pokemonSource.RuntimeID + "_RUNTIME_SKILL_LIST", pokemonSource.RuntimeSkillList);
                pokemonSource.RuntimeSkillList = new List<PokemonRuntimeSkillData>();
                foreach (var skill in pokemonTarget.RuntimeSkillList)
                {
                    pokemonSource.RuntimeSkillList.Add(new PokemonRuntimeSkillData()
                    {
                        SkillTemplate = skill.SkillTemplate,
                        Pp = 5
                    });
                }

                BattleMgr.Instance.PlayerLogStore(pokemonSource.RuntimeID + "_ATTRIBUTE", pokemonSource.Attribute);
                await pokemonSource.Attribute.RemoveAttribute(pokemonSource);
                pokemonSource.Attribute = pokemonTarget.Attribute;
                await pokemonSource.Attribute.InitAttribute(pokemonSource);

                pokemonSource.Type = pokemonTarget.Type;
                BattleMgr.Instance.PlayerLogStore(pokemonSource.RuntimeID + "_STATUS_CHANGE", pokemonSource.GetAllStatus());
                pokemonSource.ChangeAllStatus(pokemonTarget.GetAllStatus());

                await BuffMgr.Instance.AddBuff(pokemonSource, pokemonSource, 10);

                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> TryPlayRoughDeBuff = async (input, _, target, template) =>
            {
                var skillResult = (PokemonCommonResult)input;
                var pokemonTarget = (Pokemon)target;
                var skillTemplate = (CommonSkillTemplate)template;
                if (ProbTrigger(skillTemplate.SpecialEffectProb))
                {
                    if (pokemonTarget.GetStatusChange(PokemonStat.Attack) > 0 || pokemonTarget.GetStatusChange(PokemonStat.SpecialAttack) > 0)
                    {
                        pokemonTarget.SetStatusChange(PokemonStat.Attack, -pokemonTarget.GetStatusChange(PokemonStat.Attack));
                        pokemonTarget.SetStatusChange(PokemonStat.SpecialAttack, -pokemonTarget.GetStatusChange(PokemonStat.SpecialAttack));
                        await BattleMgr.Instance.SetCommandText(pokemonTarget.Name + "'s attack and special attack return to origin");
                    }
                    else
                    {
                        pokemonTarget.SetStatusChange(PokemonStat.Attack, -1);
                        pokemonTarget.SetStatusChange(PokemonStat.SpecialAttack, -1);
                        await BattleMgr.Instance.SetCommandText(pokemonTarget.Name + "'s attack and special attack decrease");
                    }
                }

                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> SetHidePokemon = async (input, source, target, _) =>
            {
                var pokemonSource = (Pokemon)source;
                var pokemonTarget = (Pokemon)target;
                await BuffMgr.Instance.AddBuff(source, source, 12);
                PokemonBuffRecorder recorder = (PokemonBuffRecorder)await BuffMgr.Instance.AddBuff(source, source, 13);

                recorder.SkillTargets = new[] { BattleMgr.Instance.GetPokemonOnstagePosition(pokemonTarget) };
                BattleMgr.Instance.BattleScenePanelTwoPlayerUI.GetPokemonBattleInfo(BattleMgr.Instance.GetPokemonOnstagePosition(pokemonSource)).SetPokemonImgActive(false);


                await BattleMgr.Instance.SetCommandText(pokemonSource.Name + " hides itself!");

                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> SetPokemonAppear = async (input, source, _, _) =>
            {
                var skillResult = (PokemonCommonResult)input;
                var pokemonSource = (Pokemon)source;
                await BuffMgr.Instance.RemoveBuffByTarget(pokemonSource, PokemonMgr.Instance.GetBuffTemplateByID(12));
                await BuffMgr.Instance.RemoveBuffByTarget(pokemonSource, PokemonMgr.Instance.GetBuffTemplateByID(13));
                BattleMgr.Instance.BattleScenePanelTwoPlayerUI.GetPokemonBattleInfo(BattleMgr.Instance.GetPokemonOnstagePosition(pokemonSource)).SetPokemonImgActive(true);

                await BattleMgr.Instance.SetCommandText(pokemonSource.Name + " appears!");

                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> AddTaunt = async (input, source, target, _) =>
            {
                var skillResult = (PokemonCommonResult)input;
                var pokemonSource = (Pokemon)source;
                var pokemonTarget = (Pokemon)target;
                if (BuffMgr.Instance.ExistActiveBuff(pokemonTarget,  PokemonMgr.Instance.GetBuffTemplateByID(14)))
                {
                    await BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(pokemonTarget.GetName() + " already get taunt");
                    return null;
                }

                await BuffMgr.Instance.AddBuff(source, target, 14);
                await BattleMgr.Instance.SetCommandText(pokemonTarget.Name + " can only use attack skill!");

                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> TryCounter = async (input, source, _, template) =>
            {
                var pokemonSource = (Pokemon)source;
                var skillTemplate = (CommonSkillTemplate)template;

                int lastSkillIndex = Int32.MinValue;
                foreach (var pokemon in BattleMgr.Instance.OnStagePokemon)
                {
                    if (BattleMgr.Instance.PlayerInGame[pokemon.TrainerID].PlayerInfo.teamID != BattleMgr.Instance.PlayerInGame[pokemonSource.TrainerID].PlayerInfo.teamID)
                    {
                        BattleStackItem lastSkill = BattleMgr.Instance.GetPokemonLastSkillData(pokemon);
                        int skillIndex = BattleMgr.Instance.BattleStack.IndexOf(lastSkill);
                        if (lastSkill == null)
                        {
                            continue;
                        }

                        if (lastSkill.Template.SkillType != SkillType.Physical)
                        {
                            continue;
                        }

                        if (!lastSkill.Targets.Contains(source))
                        {
                            continue;
                        }

                        lastSkillIndex = lastSkillIndex > skillIndex ? lastSkillIndex : skillIndex;
                    }
                }

                if (lastSkillIndex != Int32.MinValue)
                {
                    await BattleMgr.Instance.SetCommandText("Counter back " + (BattleMgr.Instance.BattleStack[lastSkillIndex].Damages[pokemonSource] * 2) + " damage!");
                    await TrySetHp(BattleMgr.Instance.BattleStack[lastSkillIndex].Damages[pokemonSource] * 2, pokemonSource, BattleMgr.Instance.BattleStack[lastSkillIndex].Source, skillTemplate);
                    return input;
                }


                await BattleMgr.Instance.SetCommandText("But failed to counter back!");
                return null;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> TryDestinyBond = async (input, source, target, template) =>
            {
                var pokemonSource = (Pokemon)source;
                var pokemonTarget = (Pokemon)target;
                var skillTemplate = (CommonSkillTemplate)template;
                if (pokemonSource.GetHp() > pokemonTarget.GetHp())
                {
                    await BattleMgr.Instance.SetCommandText("But failed!");
                }

                int hpDiff = pokemonSource.GetHp() - pokemonTarget.GetHp();
                PokemonBuffRecorder recorder = (PokemonBuffRecorder)await BuffMgr.Instance.AddBuff(source, source, 15);
                recorder.EffectLastRound = 2;
                recorder.ForbiddenCommonSkill = skillTemplate;
                await TrySetHp(hpDiff, pokemonSource, pokemonTarget, skillTemplate);
                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> RecoverHpByValue = async (input, source, target, template) =>
            {
                var pokemonSource = (Pokemon)source;
                var pokemonTarget = (Pokemon)target;
                var skillTemplate = (CommonSkillTemplate)template;
                await TrySetHp(skillTemplate.RecoveryPoint, pokemonSource, pokemonTarget, skillTemplate);
                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> RecoverPpByValue = async (input, source, target, template) =>
            {
                var skillResult = (PokemonCommonResult)input;
                var pokemonTarget = (Pokemon)target;
                var skillTemplate = (CommonSkillTemplate)template;
                List<PokemonRuntimeSkillData> skillData = skillResult.PokemonSkillsDic[pokemonTarget];
                if (skillData == null)
                {
                    throw new Exception("it should be impossible to be null");
                }

                bool success = false;
                foreach (var skill in skillData)
                {
                    bool status = await pokemonTarget.SetPpByIndex(skill, skillTemplate.RecoveryPoint);
                    success = status || success;
                }

                return success ? input : null;
            };
        }

        public struct Buff
        {
            //Buffs Callback
            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> PoisonBuff = async (input, buffSource, buffTarget, recorder) =>
            {
                var source = (Pokemon)buffSource;
                var target = (Pokemon)buffTarget;
                await BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(target.GetName() + " takes " + ((CommonSkillTemplate)recorder.Template).PercentageDamage * target.HpMax + " poison damage");
                await TrySetHp(-(int)(target.GetHpMax() * ((CommonSkillTemplate)recorder.Template).PercentageDamage), source, target, null);
                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> ForTestBuff = async (input, _, _, _) =>
            {
                await UniTask.Yield();
                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> GuardBuff = async (previousResult, buffSource, _, _) =>
            {
                var source = (Pokemon)buffSource;
                var commonResult = (PokemonCommonResult)previousResult;
                await UniTask.Yield();
                PokemonCommonResult result = new PokemonCommonResult()
                {
                    Priority = (int)(DamageApplyPriority.Guard),
                    ShouldSuccess = false, 
                    Message = source.Name + " guard the damage!",
                    STemplate = commonResult.STemplate
                };
                result = result.Priority > commonResult.Priority ? result : commonResult;
                return result;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> MoldBreakerBuff = async (previousResult, buffSource, _, _) =>
            {
                var source = (Pokemon)buffSource;
                var commonResult = (PokemonCommonResult)previousResult;
                await UniTask.Yield();
                PokemonCommonResult result = default(PokemonCommonResult);
                if (previousResult == default(PokemonCommonResult))
                {
                    System.Diagnostics.Debug.Assert(previousResult != null, nameof(previousResult) + " != null");
                    result = new PokemonCommonResult()
                    {
                        Priority = (int)(DamageApplyPriority.MoldBreaker),
                        ShouldSuccess = true,
                        Message = source.Name + " activate mold breaker!",
                        STemplate = commonResult.STemplate
                    };
                }

                System.Diagnostics.Debug.Assert(result != null, nameof(result) + " != null");
                result = result.Priority > commonResult.Priority ? result : commonResult;
                if (result != previousResult)
                {
                    result.Message = source.Name + " activate mold breaker!";
                }

                return result;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> CancelSkillExecution = async (input, _, buffTarget, recorder) =>
            {
                var target = (Pokemon)buffTarget;

                foreach (var id in ((CommonSkillTemplate)recorder.Template).TargetSkillID)
                {
                    var i = BattleMgr.Instance.CancelSkill(target, id);
                    if (i == 1)
                    {
                        await BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(target.Name + "'s " + PokemonMgr.Instance.GetSkillTemplateByID(id).Name + " is interrupted!");
                    }
                }


                return input;
            };

            public static readonly Func<IBattleEntity, IBattleEntity, ABuffRecorder, UniTask> CheckWeatherEnd = async (_, _, _) => { await BattleMgr.Instance.SetWeather(Weather.None); };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> ChangeSkillDamageBySource = async (input, _, buffTarget, recorder) =>
            {
                var commonResult = (PokemonCommonResult)input;
                var pokemonRecorder = (PokemonBuffRecorder)recorder;
                // if this buff is 
                if (commonResult.SkillSource != null && buffTarget != null && !Equals(commonResult.SkillSource, buffTarget))
                {
                    return input;
                }

                if (commonResult.STemplate.Type == pokemonRecorder.SType)
                {
                    commonResult.Damage = (int)(pokemonRecorder.ChangeFactor * commonResult.Damage);
                }

                await UniTask.Yield();
                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> AddWeatherBuffAfterDebut = async (input, buffSource, _, recorder) =>
            {
                var commonResult = (PokemonCommonResult)input;
                var pokemonRecorder = (PokemonBuffRecorder)recorder;
                int length = recorder.EffectLastRound;

                switch (pokemonRecorder.WeatherType)
                {
                    case Weather.HarshSunlight:
                    {
                        PokemonBuffRecorder buff = (PokemonBuffRecorder)await BuffMgr.Instance.AddBuff(buffSource, commonResult.DebutPokemon, 6);
                        buff.EffectLastRound = length;
                        buff.SType = PokemonType.Fire;
                        buff.ChangeFactor = 1.5f;

                        buff = (PokemonBuffRecorder)await BuffMgr.Instance.AddBuff(buffSource, commonResult.DebutPokemon, 6);
                        buff.EffectLastRound = length;
                        buff.SType = PokemonType.Water;
                        buff.ChangeFactor = 0.5f;
                        break;
                    }
                    default:
                        throw new NotImplementedException();
                }

                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> LeechSeed = async (input, buffSource, buffTarget, recorder) =>
            {
                var source = (Pokemon)buffSource;
                var target = (Pokemon)buffTarget;
                var commonResult = (PokemonCommonResult)input;
                var pokemonRecorder = (PokemonBuffRecorder)recorder;
                int damage = await DamageMaxHpByPercentage(null, target, ((CommonSkillTemplate)recorder.Template).PercentageDamage, null);
                var list = await FindTarget(new[] { pokemonRecorder.SourceIndex }, source);
                if (list.Count == 0)
                {
                    return input;
                }

                Pokemon pokemon = (await FindTarget(new[] { pokemonRecorder.SourceIndex }, source))[0];
                await TrySetHp(-damage, source, pokemon, new CommonSkillTemplate());
                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> DecreaseSpeedByPercentage = async (input, _, buffTarget, recorder) =>
            {
                var target = (Pokemon)buffTarget;
                var commonResult = (PokemonCommonResult)input;
                var pokemonRecorder = (PokemonBuffRecorder)recorder;
                if (commonResult.BuffKey != 9)
                    return input;
                target.Speed = (int)(target.Speed * ((CommonSkillTemplate)recorder.Template).PokemonDataChangePercentage);
                BattleMgr.Instance.UpdateSkillPriority();
                await UniTask.Yield();
                return input;
            };

            public static readonly Func<IBattleEntity, IBattleEntity, ABuffRecorder, UniTask> RecoverDecreaseSpeedByPercentage = async (_, buffTarget, recorder) =>
            {
                var target = (Pokemon)buffTarget;
                var buffRecorder = (PokemonBuffRecorder)recorder;
                target.Speed = (int)(target.Speed * (1 / ((CommonSkillTemplate)recorder.Template).PokemonDataChangePercentage));
                BattleMgr.Instance.UpdateSkillPriority();
                await UniTask.Yield();
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> IfBuffAllowMove = async (input, _, buffTarget, recorder) =>
            {
                var target = (Pokemon)buffTarget;
                var commonResult = (PokemonCommonResult)input;
                var pokemonRecorder = (PokemonBuffRecorder)recorder;
                if (ProbTrigger(((CommonSkillTemplate)recorder.Template).SpecialEffectProb))
                {
                    if (commonResult.Priority < (int)((CommonSkillTemplate)recorder.Template).MovePriority)
                    {
                        commonResult.Priority = (int)((CommonSkillTemplate)recorder.Template).MovePriority;
                        commonResult.CanMove = false;
                        commonResult.Message = target.Name + ((CommonSkillTemplate)recorder.Template).Message;
                    }
                }

                await UniTask.Yield();
                return input;
            };

            // target is ditto
            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> CancelTransform = async (input, buffSource, buffTarget, recorder) =>
            {
                var source = (Pokemon)buffSource;
                var target = (Pokemon)buffTarget;
                var commonResult = (PokemonCommonResult)input;
                var pokemonRecorder = (PokemonBuffRecorder)recorder;
                _ = BattleMgr.Instance.BattleScenePanelTwoPlayerUI.GetPokemonBattleInfo(BattleMgr.Instance.GetPokemonOnstagePosition(target)).SetPokemonImg(target.ImageKey);
                BattleMgr.Instance.BattleScenePanelTwoPlayerUI.GetPokemonBattleInfo(BattleMgr.Instance.GetPokemonOnstagePosition(target)).SetAttributeText(BattleMgr.Instance.PlayerLogGet<AttributesTemplate>(target.RuntimeID + "_ATTRIBUTE").Name);

                PokemonBasicInfo info = PokemonMgr.Instance.GetPokemonByID(132);

                target.Attack = info.Attack;
                target.Defence = info.Defence;
                target.SpecialAttack = info.SpecialAttack;
                target.SpecialDefence = info.SpecialDefence;
                target.Speed = info.Speed;
                target.HpMax = info.HpMax;
                await target.ChangeHp(0);

                await target.Attribute.RemoveAttribute(target);
                target.Attribute = BattleMgr.Instance.PlayerLogGet<AttributesTemplate>(target.RuntimeID + "_ATTRIBUTE");
                await target.Attribute.InitAttribute(target);

                target.Type = info.Type;
                target.ChangeAllStatus(BattleMgr.Instance.PlayerLogGet<int[]>(target.RuntimeID + "_STATUS_CHANGE"));
                target.RuntimeSkillList = BattleMgr.Instance.PlayerLogGet<List<PokemonRuntimeSkillData>>(target.RuntimeID + "_RUNTIME_SKILL_LIST");

                BattleMgr.Instance.PlayerLogDelete(target.RuntimeID + "_ATTRIBUTE");
                BattleMgr.Instance.PlayerLogDelete(target.RuntimeID + "_STATUS_CHANGE");
                BattleMgr.Instance.PlayerLogDelete(target.RuntimeID + "_RUNTIME_SKILL_LIST");
                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> CannotBeTarget = async (input, _, _, _) =>
            {
                var commonResult = (PokemonCommonResult)input;
                commonResult.CanBeTargeted = false;
                await UniTask.Yield();
                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> AutoAddSkillPlayable = async (input, _, buffTarget, recorder) =>
            {
                var target = (Pokemon)buffTarget;
                var commonResult = (PokemonCommonResult)input;
                var pokemonRecorder = (PokemonBuffRecorder)recorder;
                foreach (var id in ((CommonSkillTemplate)recorder.Template).TargetSkillID)
                {
                    CommonSkillTemplate template;
                    switch (PokemonMgr.Instance.GetSkillTemplateByID(id).TargetType)
                    {
                        case SkillTargetType.Self:
                        case SkillTargetType.All:
                        case SkillTargetType.AllEnemy:
                        case SkillTargetType.AllTeammate:
                        case SkillTargetType.AllExceptSelf:
                            template = PokemonMgr.Instance.GetSkillTemplateByID(id);
                            await template.SendLoadSkillRequest(target);
                            break;
                        case SkillTargetType.OneEnemy:
                        case SkillTargetType.OneTeammate:
                        case SkillTargetType.FirstAvailableEnemy:
                        case SkillTargetType.FirstEnemy:
                        case SkillTargetType.FirstAvailableTeammate:
                        case SkillTargetType.FirstTeammate:
                        case SkillTargetType.None:
                        default:
                            if (pokemonRecorder.SkillTargets != null)
                            {
                                PokemonCommonResult skillPreLoadResult = new PokemonCommonResult()
                                {
                                    TargetsByIndices = pokemonRecorder.SkillTargets
                                };
                                BattleMgr.Instance.LoadPokemonSkill(target, PokemonMgr.Instance.GetSkillTemplateByID(id), skillPreLoadResult);
                            }
                            else
                            {
                                template = PokemonMgr.Instance.GetSkillTemplateByID(id);
                                await template.SendLoadSkillRequest(target);
                            }

                            break;
                    }
                }

                BattleMgr.Instance.PlayerInGame[target.TrainerID].RemovePokemonFromRequestList(target);

                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> AttackSkillOnly = async (input, _, buffTarget, recorder) =>
            {
                var target = (Pokemon)buffTarget;
                var commonResult = (PokemonCommonResult)input;
                var pokemonRecorder = (PokemonBuffRecorder)recorder;
                CommonSkillTemplate template = commonResult.STemplate;
                if (template.SkillType == SkillType.Status)
                {
                    commonResult.CanMove = false;
                    commonResult.Message = target.Name + ((CommonSkillTemplate)recorder.Template).Message;
                }

                await UniTask.Yield();
                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> ForbiddenSkill = async (input, _, buffTarget, recorder) =>
            {
                var target = (Pokemon)buffTarget;
                var commonResult = (PokemonCommonResult)input;
                var pokemonRecorder = (PokemonBuffRecorder)recorder;
                if (pokemonRecorder.ForbiddenCommonSkill.ID == commonResult.STemplate.ID)
                {
                    commonResult.CanMove = false;
                    commonResult.Message = target.Name + ((CommonSkillTemplate)recorder.Template).Message;
                }

                await UniTask.Yield();
                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> IncreaseDamageWhenLowHp = async (input, _, buffTarget, recorder) =>
            {
                var target = (Pokemon)buffTarget;
                var commonResult = (PokemonCommonResult)input;
                var pokemonRecorder = (PokemonBuffRecorder)recorder;
                if (commonResult.STemplate.Type != ((CommonSkillTemplate)recorder.Template).Type)
                {
                    return input;
                }

                if (target.GetHp() < (target.HpMax * 1 / 3f))
                {
                    commonResult.Damage = (int)(1.5f * commonResult.Damage);
                }

                await UniTask.Yield();
                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> WeatherStatChange = async (input, _, buffTarget, recorder) =>
            {
                var target = (Pokemon)buffTarget;
                var commonResult = (PokemonCommonResult)input;
                var pokemonRecorder = (PokemonBuffRecorder)recorder;
                switch (commonResult.TargetWeather)
                {
                    case Weather.HarshSunlight:
                    case Weather.ExtremelyHarshSunlight:
                    case Weather.StrongSunLight:
                        switch (((CommonSkillTemplate)recorder.Template).ChangeStat)
                        {
                            case PokemonStat.SpecialAttack:
                                target.SpecialAttack = (int)(target.SpecialAttack * 1.5f);
                                break;
                            case PokemonStat.Speed:
                                target.Speed = (int)(target.SpecialAttack * 2f);
                                BattleMgr.Instance.UpdateSkillPriority();
                                break;
                            default:
                                throw new NotImplementedException();
                        }

                        break;
                    case Weather.None:
                        return input;
                    default:
                        throw new NotImplementedException();
                }

                await UniTask.Yield();
                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> WeatherMatchSetHp = async (input, _, buffTarget, _) =>
            {
                var target = (Pokemon)buffTarget;
                var commonResult = (PokemonCommonResult)input;
                if (BattleMgr.Instance.GetWeather() == Weather.HarshSunlight || BattleMgr.Instance.GetWeather() == Weather.ExtremelyHarshSunlight || BattleMgr.Instance.GetWeather() == Weather.StrongSunLight)
                {
                    await BattleMgr.Instance.SetCommandText(target.TrainerID + " " + target.Name + "'s Hp decrease 1/8 because of solar power attribute");
                    await DamageMaxHpByPercentage(null, target, 1 / 8f, null);
                }

                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> RecoverStatAtWeatherEnd = async (input, _, buffTarget, recorder) =>
            {
                var target = (Pokemon)buffTarget;
                var commonResult = (PokemonCommonResult)input;
                var pokemonRecorder = (PokemonBuffRecorder)recorder;
                switch (BattleMgr.Instance.GetWeather())
                {
                    case Weather.HarshSunlight:
                    case Weather.ExtremelyHarshSunlight:
                    case Weather.StrongSunLight:
                        switch (((CommonSkillTemplate)recorder.Template).ChangeStat)
                        {
                            case PokemonStat.Speed:
                                target.Speed = (int)(target.Speed - target.Speed * 0.5f);
                                BattleMgr.Instance.UpdateSkillPriority();
                                break;
                            case PokemonStat.SpecialAttack:
                                target.SpecialAttack = (int)(target.SpecialAttack - target.SpecialAttack * 2 / 3f);
                                break;
                            default:
                                throw new NotImplementedException();
                        }

                        break;
                    case Weather.None:
                        return input;
                    default:
                        throw new NotImplementedException();
                }

                await UniTask.Yield();
                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> StaticBuff = async (input, _, buffTarget, _) =>
            {
                var target = (Pokemon)buffTarget;
                var commonResult = (PokemonCommonResult)input;
                if (commonResult.STemplate == null)
                {
                    return input;
                }

                // here we make it simpler, every physical is touch skill
                if (commonResult.STemplate.SkillType == SkillType.Physical)
                {
                    CommonSkillTemplate temp = new CommonSkillTemplate
                    {
                        SpecialEffectProb = 0.3f
                    };
                    await Skill.TryAddParalysis(input, buffTarget, commonResult.SkillSource, temp);
                }

                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> TypeDamageResistantWithStatChange = async (input, _, buffTarget, recorder) =>
            {
                var target = (Pokemon)buffTarget;
                var commonResult = (PokemonCommonResult)input;
                var pokemonRecorder = (PokemonBuffRecorder)recorder;
                if (commonResult.Priority > (int)((CommonSkillTemplate)recorder.Template).BuffDamageApplyPriority)
                {
                    return input;
                }

                if (commonResult.STemplate.Type != ((CommonSkillTemplate)recorder.Template).Type)
                {
                    return input;
                }

                commonResult.ShouldSuccess = false;
                commonResult.Priority = (int)((CommonSkillTemplate)recorder.Template).BuffDamageApplyPriority;

                for (int i = 0; i < ((CommonSkillTemplate)recorder.Template).PokemonStatType.Length; i++)
                {
                    commonResult.Message = "No Effect, " + target.Name + "'s " + ((CommonSkillTemplate)recorder.Template).PokemonStatType[i] + " increase by " + ((CommonSkillTemplate)recorder.Template).PokemonStatPoint[i];
                    target.SetStatusChange(((CommonSkillTemplate)recorder.Template).PokemonStatType[i], ((CommonSkillTemplate)recorder.Template).PokemonStatPoint[i]);
                }

                await UniTask.Yield();
                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> Limber = async (input, _, buffTarget, _) =>
            {
                var target = (Pokemon)buffTarget;
                var commonResult = (PokemonCommonResult)input;
                if (commonResult.BuffKey == 8)
                {
                    commonResult.CanAddBuff = false;
                }

                if (commonResult.BuffKey == 9)
                {
                    await BattleMgr.Instance.SetCommandText(target.Name + " won't get paralysis because of limber");
                    commonResult.CanAddBuff = false;
                }

                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> ImposterDebut = async (input, _, buffTarget, _) =>
            {
                var target = (Pokemon)buffTarget;
                var commonResult = (PokemonCommonResult)input;
                await PokemonMgr.Instance.GetSkillTemplateByID(144).SendLoadSkillRequest(target);
                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> SheerForce = async (input, _, buffTarget, _) =>
            {
                var target = (Pokemon)buffTarget;
                var commonResult = (PokemonCommonResult)input;
                if (commonResult.STemplate.ProcedureFunctions.Length <= 1)
                {
                    return input;
                }

                bool hasDamageSkill = false;
                foreach (var function in commonResult.STemplate.ProcedureFunctions)
                {
                    if (function == Skill.TryApplyDamage)
                    {
                        hasDamageSkill = true;
                    }
                }

                if (!hasDamageSkill)
                {
                    return input;
                }

                await BattleMgr.Instance.SetCommandText(target.Name + "'s sheer force activated");

                foreach (var pokemonTarget in commonResult.TargetsByPokemons)
                {
                    foreach (var func in commonResult.STemplate.ProcedureFunctions)
                    {
                        if (input == null)
                            return null;
                        if (func == Skill.TryApplyDamage)
                        {
                            var template = CommonSkillTemplate.CopySkill(commonResult.STemplate);
                            template.Power = (int)(template.Power * 1.3f);
                            bool hit = await HitAccuracyCalculate(target, pokemonTarget, commonResult.STemplate);
                            int damage;
                            if (!hit)
                            {
                                await BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText("But it doesn't hit " + pokemonTarget.Name);
                            }

                            var success = await CanApplyDamage(target, pokemonTarget, commonResult.STemplate);
                            if (success)
                            {
                                var result = await DamageCalculate(target, pokemonTarget, commonResult.STemplate);
                                await Skill.PrintSkillEffectResult(result.Item1);
                                damage = result.Item2.Damage;
                                await pokemonTarget.ChangeHp(damage); // sheer force doesn't trigger post-damage effect: https://bulbapedia.bulbagarden.net/wiki/Sheer_Force_(Ability)
                            }
                            else
                            {
                                await Skill.PrintSkillEffectResult(SkillEffect.NotEffective);
                                damage = 0;
                            }

                            BattleMgr.Instance.BattleStack[^1].Damages ??= new Dictionary<Pokemon, int>();
                            BattleMgr.Instance.BattleStack[^1].Damages.Add(pokemonTarget, damage);
                            break;
                        }
                        else
                        {
                            input = (PokemonCommonResult)await func(input, buffTarget, pokemonTarget, commonResult.STemplate);
                        }
                    }
                }

                commonResult.CanMove = false;
                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> CursedBody = async (input, _, buffTarget, recorder) =>
            {
                var target = (Pokemon)buffTarget;
                var commonResult = (PokemonCommonResult)input;
                var pokemonRecorder = (PokemonBuffRecorder)recorder;
                if (ProbTrigger(((CommonSkillTemplate)recorder.Template).SpecialEffectProb))
                {
                    await BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(target.Name + "'s cursed body activated");
                    await BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(commonResult.SkillSource.Name + " can not use " + commonResult.STemplate.Name);
                    PokemonBuffRecorder buffRecorder = (PokemonBuffRecorder)await BuffMgr.Instance.AddBuff(buffTarget, commonResult.SkillSource, 15);
                    buffRecorder.EffectLastRound = 3;
                    buffRecorder.ForbiddenCommonSkill = commonResult.STemplate;
                }

                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> BorrowedTime = async (input, buffSource, buffTarget, recorder) =>
            {
                var source = (Pokemon)buffSource;
                var target = (Pokemon)buffTarget;
                var commonResult = (PokemonCommonResult)input;
                var pokemonRecorder = (PokemonBuffRecorder)recorder;
                if (!Equals(commonResult.SkillTarget, buffTarget))
                    return input;
                commonResult.Damage = math.abs(commonResult.Damage);
                await BattleMgr.Instance.SetCommandText(target.Name + " recover " + commonResult.Damage + " HP because of borrow time");
                commonResult.ShouldContinueSkill = false;
                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> Kizuna = async (input, buffSource, buffTarget, recorder) =>
            {
                var source = (Pokemon)buffSource;
                var target = (Pokemon)buffTarget;
                var commonResult = (PokemonCommonResult)input;
                var pokemonRecorder = (PokemonBuffRecorder)recorder;
                await UniTask.Delay(BattleMgr.Instance.AwaitTime);
                await source.ChangeHp(commonResult.Damage);
                await BattleMgr.Instance.SetCommandText(source.Name + " recover " + commonResult.Damage + " HP because of Kizuna");
                return input;
            };
        }

        public struct PreLoadProcedure
        {
            public static readonly Func<ASkillResult, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> SelectIndicesTarget = async (input, source, template) =>
            {
                var pokemonSource = (Pokemon)source;
                var skillTemplate = (CommonSkillTemplate)template;
                var skillResult = (PokemonCommonResult)input;
                APokemonBattlePlayer player = BattleMgr.Instance.PlayerInGame[pokemonSource.TrainerID];
                int[] targets = BattleMgr.Instance.TryAutoGetTarget(pokemonSource, skillTemplate.TargetType);
                if (targets == null)
                {
                    targets = await player.SelectIndicesTarget(skillTemplate, pokemonSource);
                }

                if (targets == null)
                    return null;

                skillResult.TargetsByIndices = targets;

                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> SelectPokemonsTarget = async (input, source, template) =>
            {
                var pokemonSource = (Pokemon)source;
                var skillTemplate = (CommonSkillTemplate)template;
                var skillResult = (PokemonCommonResult)input;
                APokemonBattlePlayer player = BattleMgr.Instance.PlayerInGame[pokemonSource.TrainerID];
                int[] targets = BattleMgr.Instance.TryAutoGetTarget(pokemonSource, skillTemplate.TargetType);
                if (targets == null)
                {
                    targets = await player.SelectIndicesTarget(skillTemplate, pokemonSource);
                }

                if (targets == null)
                    return null;


                List<Pokemon> pokemons = await FindTarget(targets, null);

                skillResult.TargetsByPokemons = pokemons;

                return input;
            };

            public static readonly Func<ASkillResult, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> SelectSkillsFromPokemonsTarget = async (input, source, template) =>
            {
                var pokemonSource = (Pokemon)source;
                var skillTemplate = (CommonSkillTemplate)template;
                var skillResult = (PokemonCommonResult)input;
                APokemonBattlePlayer player = BattleMgr.Instance.PlayerInGame[pokemonSource.TrainerID];
                skillResult.PokemonSkillsDic = new Dictionary<Pokemon, List<PokemonRuntimeSkillData>>();
                foreach (var target in skillResult.TargetsByPokemons)
                {
                    var skillsOfTarget = await player.SelectSkillFromTarget(skillTemplate.NeedToSelectSkillCount, target);
                    if (skillsOfTarget == null)
                        return null;
                    skillResult.PokemonSkillsDic.Add(target, skillsOfTarget);
                }

                return input;
            };
        }
    }
}