using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Enum;
using JetBrains.Annotations;
using Managers.BattleMgrComponents.BattlePlayables.Skills;
using Managers.BattleMgrComponents.BattlePlayer;
using Managers.BattleMgrComponents.PokemonLogic;
using Managers.BattleMgrComponents.PokemonLogic.BuffResults;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

namespace Managers.BattleMgrComponents
{
    public class BattleLogic
    {
        public static void CheckSkillValidation(RunTimeSkillBase skill)
        {
            if (skill.Template.ProcedureFunctions == null)
                return;
            foreach (var func in skill.Template.ProcedureFunctions)
            {
                skill.Procedure.Add(func);
            }
        }

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
            CommonResult result = new CommonResult
            {
                PokemonStat =
                {
                    [(int)PokemonStat.SpecialAttack] = attack
                }
            };
            result = await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.GettingSpecialAttack, result, source);
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

        private static int TypeDamageCalculate(float damage, SkillTemplate template, Pokemon source, Pokemon target)
        {
            float sameTypeAttackBonus = source.Type == template.Type ? 1.5f : 1f;
            return (int)(damage * PokemonMgr.Instance.GetTypeResistance((int)template.Type, (int)target.Type) * sameTypeAttackBonus);
        }

        private static async UniTask<Tuple<SkillEffect, int>> DamageCalculate(Pokemon source, Pokemon target, SkillTemplate template)
        {
            bool criticalHit = await CriticalHitCalculate(source.GetStatusChange(PokemonStat.CriticalHit) + template.CriticalRate);
            SkillEffect effect = SkillEffect.SuperEffective;
            var result = new CommonResult
            {
                Power = template.Power
            };
            result = await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.CalculatingSkillPower, result);

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
            result.Damage = damage;
            result.STemplate = template;
            result.SkillSource = source;
            result = await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.CalculatingFinalDamage, result);

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

            return new Tuple<SkillEffect, int>(effect, result.Damage);
        }

        private static async UniTask<bool> HitAccuracyCalculate(Pokemon source, Pokemon target, SkillTemplate template)
        {
            //A=(B*E*F*G)
            int b = (int)Math.Floor(255f * template.Accuracy / 100f);
            var result = new CommonResult
            {
                Accuracy = b,
                MustHit = false
            };
            result = await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.GettingSkillAccuracy, result);
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
            result = await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.CalculatingHit, result);
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
            var result = new CommonResult
            {
                CriticalRate = criticalRate,
                IsCritical = isCritical
            };
            result = await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.CalculatingCriticalDamage, result);

            return result.IsCritical;
        }

        // if damage can be blocked
        private static async UniTask<bool> CanApplyDamage(Pokemon source, Pokemon target, SkillTemplate template)
        {
            var task0 = BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.BeforeApplyDamage, new BeforeDamageApplyResult((int)DamageApplyPriority.Normal, true, null, template), source);
            var task1 = BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.BeforeTakingDamage, new BeforeDamageApplyResult((int)DamageApplyPriority.Normal, true, null, template), target);
            var (sourceResult, targetResult) = await UniTask.WhenAll(task0, task1);
            var result = BeforeDamageApplyResult.Compare(sourceResult, targetResult);
            bool success = result.ShouldSuccess;

            if (result.Message != null)
            {
                BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(result.Message);
                await UniTask.Delay(BattleMgr.Instance.AwaitTime);
            }

            return success;
        }

        private static async UniTask<BuffRecorder> TryAddBuff(Pokemon source, Pokemon target, SkillTemplate template, int buffId)
        {
            var buffTemplate = PokemonMgr.Instance.GetBuffTemplateByID(buffId);
            if (ProbTrigger(template.SpecialEffectProb))
            {
                if (BuffMgr.Instance.Exist(target, buffId))
                {
                    BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(target.GetName() + " already get " + buffTemplate.Name);
                    return null;
                }

                BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(target.GetName() + " get " + buffTemplate.Name);
                var recorder = await BuffMgr.Instance.AddBuff(source, target, buffId);
                await UniTask.Delay(BattleMgr.Instance.AwaitTime);
                return recorder;
            }

            return null;
        }

        private static async UniTask<BuffRecorder> MustAddBuff(Pokemon source, Pokemon target, SkillTemplate template, int buffId)
        {
            var buffTemplate = PokemonMgr.Instance.GetBuffTemplateByID(buffId);
            if (BuffMgr.Instance.Exist(target, buffId))
            {
                BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(target.GetName() + " already get " + buffTemplate.Name);
                return null;
            }

            var recorder = await BuffMgr.Instance.AddBuff(source, target, buffId);
            return recorder;
        }

        private static readonly Func<Pokemon, Pokemon, float, SkillTemplate, UniTask<int>> DamageMaxHpByPercentage = async (source, target, percentage, template) =>
        {
            int damage = -(int)Math.Ceiling((target.GetHpMax()) * (percentage));
            await TrySetHp(damage, source, target, template);
            return damage;
        };

        private static readonly Func<Pokemon, Pokemon, int, UniTask> RecoverHp = async (_, target, point) =>
        {
            target.ChangeHp(point);
            BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(target.GetName() + " 's HP recover  " + point);
            await UniTask.Delay(BattleMgr.Instance.AwaitTime);
        };

        public static async UniTask<List<Pokemon>> FindTarget(int[] indices)
        {
            List<Pokemon> targets = new List<Pokemon>();
            foreach (var index in indices)
            {
                var target = BattleMgr.Instance.OnStagePokemon[index];
                if (target != null)
                {
                    var output = await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.WhenGettingTarget, new CommonResult(), target);
                    if (output.CanBeTargeted != true)
                    {
                        continue;
                    }
                    targets.Add(target);
                }
            }

            return targets;
        }

        private static bool TargetAvailable(Pokemon target)
        {
            return target.OnStage && !target.IsFaint;
        }

        //Skills

        private static void PrintSkillEffectResult(SkillEffect result)
        {
            switch (result)
            {
                case SkillEffect.SuperEffective:
                    BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText("Supper effective");
                    break;
                case SkillEffect.Effective:
                    BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText("Effective");
                    break;
                case SkillEffect.NotVeryEffective:
                    BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText("Not very effective");
                    break;
                case SkillEffect.NotEffective:
                    BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText("Not effective");
                    break;
                case SkillEffect.CriticalDamage:
                    BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText("Critical hit!");
                    break;
            }
        }

        public static readonly Func<Pokemon, Pokemon, SkillTemplate, UniTask<bool>> OnlyForTest = async (source, _, _) =>
        {
            BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(source.GetName() + " doesn't do anything");
            await UniTask.Delay(BattleMgr.Instance.AwaitTime);
            return true;
        };

        public static readonly Func<Pokemon, Pokemon, SkillTemplate, UniTask<bool>> TryDamageMaxHpByPercentage = async (source, target, template) =>
        {
            await DamageMaxHpByPercentage(source, target, template.PercentageDamage, template);
            return true;
        };

        private static readonly Func<Pokemon, Pokemon, SkillTemplate, UniTask<int>> ApplyDamage = async (source, target, template) =>
        {
            bool hit = await HitAccuracyCalculate(source, target, template);
            if (!hit)
            {
                BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText("But it doesn't hit " + target.Name);
                await UniTask.Delay(BattleMgr.Instance.AwaitTime);
                return 0;
            }

            var success = await CanApplyDamage(source, target, template);
            if (success)
            {
                var result = await DamageCalculate(source, target, template);
                PrintSkillEffectResult(result.Item1);
                await TrySetHp(-result.Item2, source, target, template);
                await UniTask.Delay(BattleMgr.Instance.AwaitTime);
                return -result.Item2;
            }
            else
            {
                PrintSkillEffectResult(SkillEffect.NotEffective);
                await UniTask.Delay(BattleMgr.Instance.AwaitTime);
                return 0;
            }
        };

        private static async UniTask TrySetHp(int hpChange, Pokemon source, Pokemon target, SkillTemplate template)
        {
            target.ChangeHp(hpChange);
            CommonResult cResult = new CommonResult
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
        }

        public static readonly Func<Pokemon, Pokemon, SkillTemplate, UniTask<bool>> TryApplyDamage = async (source, target, template) =>
        {
            int damage = await ApplyDamage(source, target, template);
            BattleMgr.Instance.BattleStack[^1].Damages ??= new Dictionary<Pokemon, int>();
            BattleMgr.Instance.BattleStack[^1].Damages.Add(target, damage);
            return damage != 0;
        };


        public static readonly Func<Pokemon, Pokemon, SkillTemplate, UniTask<bool>> FocusPunchCharge = async (source, _, _) =>
        {
            BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(source.Name + " starts charging");
            await UniTask.Delay(BattleMgr.Instance.AwaitTime);
            await BuffMgr.Instance.AddBuff(source, source, 3);
            BattleMgr.Instance.LoadPokemonSkillDirectly(source, 10264);
            return true;
        };

        public static readonly Func<Pokemon, Pokemon, SkillTemplate, UniTask<bool>> TryAddPoison = async (source, target, template) =>
        {
            if (!TargetAvailable(target))
                return false;
            var recorder = await TryAddBuff(source, target, template, 0);
            return recorder != null;
        };

        public static readonly Func<Pokemon, Pokemon, SkillTemplate, UniTask<bool>> TryAddGuard = async (source, target, _) =>
        {
            if (BuffMgr.Instance.Exist(target, 1))
            {
                BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(target.GetName() + " already guard");
                return false;
            }

            if (BattleMgr.Instance.IsLastSkill())
            {
                BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(target.GetName() + " fail to guard it self");
                return false;
            }

            BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(target.GetName() + " guard itself");
            await BuffMgr.Instance.AddBuff(source, target, 1);
            await UniTask.Delay(BattleMgr.Instance.AwaitTime);
            return true;
        };

        public static readonly Func<Pokemon, Pokemon, SkillTemplate, UniTask<bool>> TryAddWeather = async (_, target, template) =>
        {
            int weatherLength = 3;
            
            var result = await BattleMgr.Instance.SetWeather(template.WeatherType);
            if (result)
            {
                BuffRecorder recorder = await BuffMgr.Instance.AddBuff(null, null, 4, false, true);
                recorder.EffectLastRound = weatherLength;
                //debut pokemon add buff
                recorder = await BuffMgr.Instance.AddBuff(null, null, 5, false, true);
                recorder.EffectLastRound = weatherLength;
                recorder.WeatherType = template.WeatherType;

                int[] indices = await BattleMgr.Instance.RoughGetTarget(target, SkillTargetType.All);
                List<Pokemon> pokemons = await FindTarget(indices);
                foreach (var pokemon in pokemons)
                {
                    switch (template.WeatherType)
                    {
                        case Weather.HarshSunlight:
                            Debug.LogWarning("Add buffs!");
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
            }

            await UniTask.Delay(BattleMgr.Instance.AwaitTime);

            return true;
        };

        public static readonly Func<Pokemon, Pokemon, SkillTemplate, UniTask<bool>> TryChangePokemonStat = async (_, target, template) =>
        {
            for (int i = 0; i < template.PokemonStatType.Length; i++)
            {
                target.SetStatusChange(template.PokemonStatType[i], template.PokemonStatPoint[i]);
                BattleMgr.Instance.SetCommandText(target.Name + "'s " + template.PokemonStatType[i] + " change " + template.PokemonStatPoint[i]);
                await UniTask.Delay(BattleMgr.Instance.AwaitTime);
            }

            return true;
        };

        public static readonly Func<Pokemon, Pokemon, SkillTemplate, UniTask<bool>> TryAddLeechSeed = async (source, target, template) =>
        {
            BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText("A seed is planted on " + target.GetName());
            await UniTask.Delay(BattleMgr.Instance.AwaitTime);
            var recorder = await TryAddBuff(source, target, template, 7);
            if (recorder != null)
            {
                recorder.SourceIndex = BattleMgr.Instance.GetPokemonOnstagePosition(source);
                return true;
            }

            return false;
        };

        public static readonly Func<Pokemon, Pokemon, SkillTemplate, UniTask<bool>> TryGigaDrain = async (source, target, template) =>
        {
            int damage = await ApplyDamage(source, target, template);
            if (damage == 0)
            {
                return false;
            }

            await RecoverHp(null, source, (int)(-damage * 0.5f));
            return true;
        };

        public static readonly Func<Pokemon, Pokemon, SkillTemplate, UniTask<bool>> TrySynthesis = async (source, _, template) =>
        {
            float recoverPercentage = template.PercentageDamage;
            if (BattleMgr.Instance.GetWeather() == Weather.HarshSunlight)
            {
                recoverPercentage = 2 / 3f;
            }

            await RecoverHp(source, source, (int)(source.HpMax * recoverPercentage));
            return true;
        };

        public static readonly Func<Pokemon, Pokemon, SkillTemplate, UniTask<bool>> TryAddParalysis = async (source, target, template) =>
        {
            if (ProbTrigger(template.SpecialEffectProb))
            {
                if (BuffMgr.Instance.Exist(target, 8) || BuffMgr.Instance.Exist(target, 9))
                {
                    BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(target.GetName() + " already get paralysis!");
                    return false;
                }
                var task0 = await MustAddBuff(source, target, template, 8);
                var task1 = await MustAddBuff(source, target, template, 9);
                if (task0 == null && task1 == null)
                {
                    return false;
                }

                BattleMgr.Instance.SetCommandText(target.Name + " get paralysis");
                await UniTask.Delay(BattleMgr.Instance.AwaitTime);
                return true;
            }

            return false;
        };

        public static readonly Func<Pokemon, Pokemon, SkillTemplate, UniTask<bool>> TryAddFlinch = async (source, target, template) =>
        {
            if (ProbTrigger(template.SpecialEffectProb))
            {
                var task0 = MustAddBuff(source, target, template, 11);
                var r0 = await task0;
                if (r0 == null)
                {
                    return false;
                }

                BattleMgr.Instance.SetCommandText(target.Name + " get flinch");
                await UniTask.Delay(BattleMgr.Instance.AwaitTime);
                return true;
            }

            return false;
        };

        public static readonly Func<Pokemon, Pokemon, SkillTemplate, UniTask<bool>> HiddenPower = async (source, target, template) =>
        {
            PokemonType targetType = source.Type;
            if (math.abs(PokemonMgr.Instance.GetTypeResistance((int)targetType, (int)target.Type) - 2) > 0.01)
            {
                for (int i = 0; i < 13; i++)
                {
                    if (math.abs(PokemonMgr.Instance.GetTypeResistance(i, (int)target.Type) - 2) < 0.01)
                    {
                        targetType = (PokemonType)i;
                        break;
                    }
                }
            }

            SkillTemplate adaptiveTemplate = SkillTemplate.Copy(template);
            adaptiveTemplate.Type = targetType;
            var damage = await ApplyDamage(source, target, adaptiveTemplate);
            if (damage == 0)
            {
                return false;
            }

            return true;
        };

        public static readonly Func<Pokemon, Pokemon, SkillTemplate, UniTask<bool>> ProbChangeStat = async (_, target, template) =>
        {
            if (ProbTrigger(template.SpecialEffectProb))
            {
                for (int i = 0; i < template.PokemonStatType.Length; i++)
                {
                    target.SetStatusChange(template.PokemonStatType[i], template.PokemonStatPoint[i]);
                    BattleMgr.Instance.SetCommandText(target.Name + "'s " + template.PokemonStatType[i] + " change " + template.PokemonStatPoint[i]);
                    await UniTask.Delay(BattleMgr.Instance.AwaitTime);
                }
            }

            await UniTask.Yield();
            return true;
        };

        public static readonly Func<Pokemon, Pokemon, SkillTemplate, UniTask<bool>> Transform = async (source, target, _) =>
        {
            if (source.TrainerID == BattleMgr.Instance.LocalPlayer.PlayerInfo.playerID)
            {
                await BattleMgr.Instance.BattleScenePanelTwoPlayerUI.selfPokemonInfo.SetPokemonImg(target.ImageKey);
                BattleMgr.Instance.BattleScenePanelTwoPlayerUI.selfPokemonInfo.SetAttributeText(target.Attribute.Name);
            }
            else
            {
                await BattleMgr.Instance.BattleScenePanelTwoPlayerUI.opPokemonInfo.SetPokemonImg(target.ImageKey);
                BattleMgr.Instance.BattleScenePanelTwoPlayerUI.opPokemonInfo.SetAttributeText(target.Attribute.Name);
            }

            source.Attack = target.Attack;
            source.Defence = target.Defence;
            source.SpecialAttack = target.SpecialAttack;
            source.SpecialDefence = target.SpecialDefence;
            source.Speed = target.Speed;
            source.HpMax = target.HpMax;
            source.ChangeHp(0);

            source.SkillList = target.SkillList;
            BattleMgr.Instance.PlayerLogStore(source.RuntimeID + "_ATTRIBUTE", source.Attribute);
            source.Attribute.RemoveAttribute(source);
            source.Attribute = target.Attribute;
            await source.Attribute.InitAttribute(source);

            source.Type = target.Type;
            BattleMgr.Instance.PlayerLogStore(source.RuntimeID + "_STATUS_CHANGE", source.GetAllStatus());
            source.ChangeAllStatus(target.GetAllStatus());

            BattleMgr.Instance.PlayerLogStore(source.RuntimeID + "_SKILL_PP_LIST", source.Pps);
            source.Pps = new List<int>();
            for (int i = 0; i < source.SkillList.Length; i++)
            {
                source.Pps.Add(5);
            }

            await BuffMgr.Instance.AddBuff(source, source, 10);

            return true;
        };
        
        public static readonly Func<Pokemon, Pokemon, SkillTemplate, UniTask<bool>> TryPlayRoughDeBuff = async (_, target, template) =>
        {
            if (ProbTrigger(template.SpecialEffectProb))
            {
                if (target.GetStatusChange(PokemonStat.Attack) > 0 || target.GetStatusChange(PokemonStat.SpecialAttack) > 0)
                {
                    target.SetStatusChange(PokemonStat.Attack, -target.GetStatusChange(PokemonStat.Attack));
                    target.SetStatusChange(PokemonStat.SpecialAttack, -target.GetStatusChange(PokemonStat.SpecialAttack));
                    BattleMgr.Instance.SetCommandText(target.Name + "'s attack and special attack return to origin");
                    await UniTask.Delay(BattleMgr.Instance.AwaitTime);
                }
                else
                {
                    target.SetStatusChange(PokemonStat.Attack, -1);
                    target.SetStatusChange(PokemonStat.SpecialAttack, -1);
                    BattleMgr.Instance.SetCommandText(target.Name + "'s attack and special attack decrease");
                    await UniTask.Delay(BattleMgr.Instance.AwaitTime);
                }
            }

            return true;
        };

        public static readonly Func<Pokemon, Pokemon, SkillTemplate, UniTask<bool>> SetHidePokemon = async (source, _, _) =>
        {
            await BuffMgr.Instance.AddBuff(source, source, 12);
            await BuffMgr.Instance.AddBuff(source, source, 13);
            if (source.TrainerID == BattleMgr.Instance.LocalPlayer.PlayerInfo.playerID)
            {
                BattleMgr.Instance.BattleScenePanelTwoPlayerUI.selfPokemonInfo.SetPokemonImgActive(false);
            }
            else
            {
                BattleMgr.Instance.BattleScenePanelTwoPlayerUI.opPokemonInfo.SetPokemonImgActive(false);
            }
            
            BattleMgr.Instance.SetCommandText(source.Name + " hides itself!");
            await UniTask.Delay(BattleMgr.Instance.AwaitTime);

            return true;
        };

        public static readonly Func<Pokemon, Pokemon, SkillTemplate, UniTask<bool>> SetPokemonAppear = async (source, _, _) =>
        {
            BuffMgr.Instance.RemoveBuffByTarget(source, 12);
            BuffMgr.Instance.RemoveBuffByTarget(source, 13);
            if (source.TrainerID == BattleMgr.Instance.LocalPlayer.PlayerInfo.playerID)
            {
                BattleMgr.Instance.BattleScenePanelTwoPlayerUI.selfPokemonInfo.SetPokemonImgActive(true);
            }
            else
            {
                BattleMgr.Instance.BattleScenePanelTwoPlayerUI.opPokemonInfo.SetPokemonImgActive(true);
            }

            BattleMgr.Instance.SetCommandText(source.Name + " appears!");
            await UniTask.Delay(BattleMgr.Instance.AwaitTime);

            return true;
        };
        
        public static readonly Func<Pokemon, Pokemon, SkillTemplate, UniTask<bool>> AddTaunt = async (source, target, _) =>
        {
            if (BuffMgr.Instance.Exist(target, 14))
            {
                BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(target.GetName() + " already get taunt");
                return true;
            }

            await BuffMgr.Instance.AddBuff(source, target, 14);
            BattleMgr.Instance.SetCommandText(target.Name + " can only use attack skill!");
            await UniTask.Delay(BattleMgr.Instance.AwaitTime);

            return true;
        };

        public static readonly Func<Pokemon, Pokemon, SkillTemplate, UniTask<bool>> TryCounter = async (source, _, template) =>
        {
            int lastSkillIndex = Int32.MinValue;
            foreach (var pokemon in BattleMgr.Instance.OnStagePokemon)
            {
                if (BattleMgr.Instance.PlayerInGame[pokemon.TrainerID].PlayerInfo.teamID != BattleMgr.Instance.PlayerInGame[source.TrainerID].PlayerInfo.teamID)
                {
                    BattleStackItem lastSkill = BattleMgr.Instance.GetPokemonLastSkillData(pokemon);
                    Debug.LogWarning(lastSkill.Template.Name);
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
                BattleMgr.Instance.SetCommandText("Counter back " + (BattleMgr.Instance.BattleStack[lastSkillIndex].Damages[source] * 2) + " damage!");
                await TrySetHp(BattleMgr.Instance.BattleStack[lastSkillIndex].Damages[source] * 2, source, BattleMgr.Instance.BattleStack[lastSkillIndex].Source, template);
                await UniTask.Delay(BattleMgr.Instance.AwaitTime);
                return true;
            }


            BattleMgr.Instance.SetCommandText("But failed to counter back!");
            await UniTask.Delay(BattleMgr.Instance.AwaitTime);
            return false;
        };

        public static readonly Func<Pokemon, Pokemon, SkillTemplate, UniTask<bool>> TryDestinyBond = async (source, target, template) =>
        {
            if (source.GetHp() > target.GetHp())
            {
                BattleMgr.Instance.SetCommandText("But failed!");
                await UniTask.Delay(BattleMgr.Instance.AwaitTime);
            }

            int hpDiff = source.GetHp() - target.GetHp();
            var recorder = await BuffMgr.Instance.AddBuff(source, source, 15);
            recorder.EffectLastRound = 2;
            recorder.ForbiddenSkill = template;
            await TrySetHp(hpDiff, source, target, template);
            return true;
        };

        //Buffs Callback
        public static readonly Func<CommonResult, Pokemon, Pokemon, BuffRecorder, UniTask<CommonResult>> PoisonBuff = async (input, buffSource, buffTarget, recorder) =>
        {
            BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(buffTarget.GetName() + " takes " + recorder.Template.PercentageDamage * buffTarget.HpMax + " poison damage");
            await TrySetHp(-(int)(buffTarget.GetHpMax() * recorder.Template.PercentageDamage), buffSource, buffTarget, null);
            await UniTask.Delay(BattleMgr.Instance.AwaitTime);
            return input;
        };

        public static readonly Func<CommonResult, Pokemon, Pokemon, BuffRecorder, UniTask<CommonResult>> ForTestBuff = async (input, _, _, _) =>
        {
            await UniTask.Yield();
            return input;
        };

        public static readonly Func<BeforeDamageApplyResult, Pokemon, Pokemon, BuffRecorder, UniTask<BeforeDamageApplyResult>> GuardBuff = async (previousResult, buffSource, _, _) =>
        {
            await UniTask.Yield();
            BeforeDamageApplyResult result = new BeforeDamageApplyResult((int)(DamageApplyPriority.Guard), false, buffSource.Name + " guard the damage!", previousResult.Template);
            result = BeforeDamageApplyResult.Compare(result, previousResult);
            return result;
        };

        public static readonly Func<BeforeDamageApplyResult, Pokemon, Pokemon, BuffRecorder, UniTask<BeforeDamageApplyResult>> MoldBreakerBuff = async (previousResult, buffSource, _, _) =>
        {
            await UniTask.Yield();
            BeforeDamageApplyResult result = default(BeforeDamageApplyResult);
            if (previousResult == default(BeforeDamageApplyResult))
            {
                System.Diagnostics.Debug.Assert(previousResult != null, nameof(previousResult) + " != null");
                result = new BeforeDamageApplyResult((int)(DamageApplyPriority.MoldBreaker), true, buffSource.Name + " activate mold breaker!", previousResult.Template);
            }

            result = BeforeDamageApplyResult.Compare(result, previousResult);
            if (result != previousResult)
            {
                result.Message = buffSource.Name + " activate mold breaker!";
            }

            return result;
        };

        public static readonly Func<CommonResult, Pokemon, Pokemon, BuffRecorder, UniTask<CommonResult>> CancelSkillExecution = async (input, _, buffTarget, recorder) =>
        {
            var i = BattleMgr.Instance.CancelSkill(buffTarget, recorder.Template.TargetSkillID);
            if (i == 1)
            {
                BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(buffTarget.Name + "'s " + PokemonMgr.Instance.GetSkillTemplateByID(recorder.Template.TargetSkillID).Name + " is interrupted!");
                await UniTask.Delay(BattleMgr.Instance.AwaitTime);
            }

            return input;
        };

        public static readonly Func<Pokemon, Pokemon, BuffRecorder, UniTask> CheckWeatherEnd = async (_, _, _) => { await BattleMgr.Instance.SetWeather(Weather.None); };

        public static readonly Func<CommonResult, Pokemon, Pokemon, BuffRecorder, UniTask<CommonResult>> ChangeSkillDamageBySource = async (input, _, buffTarget, recorder) =>
        {
            // if this buff is 
            if (input.SkillSource != null && buffTarget != null && !Equals(input.SkillSource, buffTarget))
            {
                return input;
            }

            if (input.STemplate.Type == recorder.SType)
            {
                input.Damage = (int)(recorder.ChangeFactor * input.Damage);
            }

            await UniTask.Yield();
            return input;
        };

        public static readonly Func<CommonResult, Pokemon, Pokemon, BuffRecorder, UniTask<CommonResult>> AddWeatherBuffAfterDebut = async (input, buffSource, _, recorder) =>
        {
            int length = recorder.EffectLastRound;

            switch (recorder.WeatherType)
            {
                case Weather.HarshSunlight:
                {
                    var buff = await BuffMgr.Instance.AddBuff(buffSource, input.DebutPokemon, 6);
                    buff.EffectLastRound = length;
                    buff.SType = PokemonType.Fire;
                    buff.ChangeFactor = 1.5f;

                    buff = await BuffMgr.Instance.AddBuff(buffSource, input.DebutPokemon, 6);
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

        public static readonly Func<CommonResult, Pokemon, Pokemon, BuffRecorder, UniTask<CommonResult>> LeechSeed = async (input, _, buffTarget, recorder) =>
        {
            int damage = await DamageMaxHpByPercentage(null, buffTarget, recorder.Template.PercentageDamage, null);
            var list = await FindTarget(new[] { recorder.SourceIndex });
            if (list.Count == 0)
            {
                return input;
            }

            Pokemon pokemon = (await FindTarget(new[] { recorder.SourceIndex }))[0];
            await RecoverHp(null, pokemon, -damage);
            return input;
        };

        public static readonly Func<CommonResult, Pokemon, Pokemon, BuffRecorder, UniTask<CommonResult>> DecreaseSpeedByPercentage = async (input, _, buffTarget, recorder) =>
        {
            if (input.BuffKey != 9)
                return input;
            buffTarget.Speed = (int)(buffTarget.Speed * recorder.Template.ChangePercentage);
            BattleMgr.Instance.UpdateSkillPriority();
            await UniTask.Yield();
            return input;
        };
        
        public static readonly Func<Pokemon, Pokemon, BuffRecorder, UniTask> RecoverDecreaseSpeedByPercentage = async (_, buffTarget, recorder) =>
        {
            buffTarget.Speed = (int)(buffTarget.Speed * (1 / recorder.Template.ChangePercentage));
            BattleMgr.Instance.UpdateSkillPriority();
            await UniTask.Yield();
        };

        public static readonly Func<CommonResult, Pokemon, Pokemon, BuffRecorder, UniTask<CommonResult>> BuffCanMoveParalysis = async (input, _, buffTarget, recorder) =>
        {
            if (ProbTrigger(recorder.Template.SpecialEffectProb))
            {
                if (input.Priority < (int)MovePriority.Paralysis)
                {
                    input.Priority = (int)MovePriority.Paralysis;
                    input.CanMove = false;
                    input.Message = buffTarget.Name + recorder.Template.Message;
                }
            }

            await UniTask.Yield();
            return input;
        };
        
        public static readonly Func<CommonResult, Pokemon, Pokemon, BuffRecorder, UniTask<CommonResult>> BuffCanMoveFlinch = async (input, _, buffTarget, recorder) =>
        {
            if (ProbTrigger(recorder.Template.SpecialEffectProb))
            {
                if (input.Priority < (int)MovePriority.Flinch)
                {
                    input.Priority = (int)MovePriority.Flinch;
                    input.CanMove = false;
                    input.Message = buffTarget.Name + recorder.Template.Message;
                }
            }

            await UniTask.Yield();
            return input;
        };

        // target is ditto
        public static readonly Func<CommonResult, Pokemon, Pokemon, BuffRecorder, UniTask<CommonResult>> CancelTransform = async (input, buffSource, buffTarget, recorder) =>
        {
            if (buffTarget.TrainerID == BattleMgr.Instance.LocalPlayer.PlayerInfo.playerID)
            {
                _ = BattleMgr.Instance.BattleScenePanelTwoPlayerUI.selfPokemonInfo.SetPokemonImg(buffTarget.ImageKey);
                BattleMgr.Instance.BattleScenePanelTwoPlayerUI.selfPokemonInfo.SetAttributeText(BattleMgr.Instance.PlayerLogGet<AttributesTemplate>(buffTarget.RuntimeID + "_ATTRIBUTE").Name);
            }
            else
            {
                _ = BattleMgr.Instance.BattleScenePanelTwoPlayerUI.opPokemonInfo.SetPokemonImg(buffTarget.ImageKey);
                BattleMgr.Instance.BattleScenePanelTwoPlayerUI.opPokemonInfo.SetAttributeText(BattleMgr.Instance.PlayerLogGet<AttributesTemplate>(buffTarget.RuntimeID + "_ATTRIBUTE").Name);
            }

            PokemonBasicInfo info = PokemonMgr.Instance.GetPokemonByID(132);

            buffTarget.Attack = info.Attack;
            buffTarget.Defence = info.Defence;
            buffTarget.SpecialAttack = info.SpecialAttack;
            buffTarget.SpecialDefence = info.SpecialDefence;
            buffTarget.Speed = info.Speed;
            buffTarget.HpMax = info.HpMax;
            buffTarget.ChangeHp(0);

            buffTarget.SkillList = info.SkillList;

            buffTarget.Attribute.RemoveAttribute(buffTarget);
            buffTarget.Attribute = BattleMgr.Instance.PlayerLogGet<AttributesTemplate>(buffTarget.RuntimeID + "_ATTRIBUTE");
            await buffTarget.Attribute.InitAttribute(buffTarget);

            buffTarget.Type = info.Type;
            buffTarget.ChangeAllStatus(BattleMgr.Instance.PlayerLogGet<int[]>(buffTarget.RuntimeID + "_STATUS_CHANGE"));

            buffTarget.Pps = BattleMgr.Instance.PlayerLogGet<List<int>>(buffTarget.RuntimeID + "_SKILL_PP_LIST");

            BattleMgr.Instance.PlayerLogDelete(buffTarget.RuntimeID + "_ATTRIBUTE");
            BattleMgr.Instance.PlayerLogDelete(buffTarget.RuntimeID + "_STATUS_CHANGE");
            BattleMgr.Instance.PlayerLogDelete(buffTarget.RuntimeID + "_SKILL_PP_LIST");
            return input;
        };

        public static readonly Func<CommonResult, Pokemon, Pokemon, BuffRecorder, UniTask<CommonResult>> CannotBeTarget = async (input, _, _, _) =>
        {
            input.CanBeTargeted = false;
            await UniTask.Yield();
            return input;
        };
        
        public static readonly Func<CommonResult, Pokemon, Pokemon, BuffRecorder, UniTask<CommonResult>> AddDigAttack = async (input, _, buffTarget, _) =>
        {
            BattleMgr.Instance.LoadPokemonSkillDirectly(buffTarget, 10091);
            input.NeedCommandFromPokemon = false;
            await UniTask.Yield();
            return input;
        };
        
        public static readonly Func<CommonResult, Pokemon, Pokemon, BuffRecorder, UniTask<CommonResult>> AttackSkillOnly = async (input, _, buffTarget, recorder) =>
        {
            SkillTemplate template = input.STemplate;
            if (template.SkillType == SkillType.Status)
            {
                input.CanMove = false;
                input.Message = buffTarget.Name + recorder.Template.Message;
            }
            await UniTask.Yield();
            return input;
        };

        public static readonly Func<CommonResult, Pokemon, Pokemon, BuffRecorder, UniTask<CommonResult>> ForbiddenSkill = async (input, _, buffTarget, recorder) =>
        {
            if (recorder.ForbiddenSkill.SkillID == input.STemplate.SkillID)
            {
                input.CanMove = false;
                input.Message = buffTarget.Name + recorder.Template.Message;
            }

            await UniTask.Yield();
            return input;
        };

        public static readonly Func<CommonResult, Pokemon, Pokemon, BuffRecorder, UniTask<CommonResult>> DamageUpWhenHpDown = async (input, _, buffTarget, recorder) =>
        {
            if (input.STemplate.Type != recorder.Template.Type)
            {
                return input;
            }
            if (buffTarget.GetHp() < (buffTarget.HpMax * 1 / 3f))
            {
                input.Damage = (int)(1.5f * input.Damage);
            }

            await UniTask.Yield();
            return input;
        };
        
        public static readonly Func<CommonResult, Pokemon, Pokemon, BuffRecorder, UniTask<CommonResult>> WeatherStatChange = async (input, _, buffTarget, recorder) =>
        {
            switch (input.TargetWeather)
            {
                case Weather.HarshSunlight:
                case Weather.ExtremelyHarshSunlight:
                case Weather.StrongSunLight:
                    switch (recorder.Template.ChangeStat)
                    {
                        case PokemonStat.SpecialAttack:
                            buffTarget.SpecialAttack = (int)(buffTarget.SpecialAttack * 1.5f);
                            break;
                        case PokemonStat.Speed:
                            buffTarget.Speed = (int)(buffTarget.SpecialAttack * 2f);
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
        
        public static readonly Func<CommonResult, Pokemon, Pokemon, BuffRecorder, UniTask<CommonResult>> SolarPowerSetHp = async (input, _, buffTarget, _) =>
        {
            if (BattleMgr.Instance.GetWeather() == Weather.HarshSunlight || BattleMgr.Instance.GetWeather() == Weather.ExtremelyHarshSunlight || BattleMgr.Instance.GetWeather() == Weather.StrongSunLight)
            {
                BattleMgr.Instance.SetCommandText(buffTarget.TrainerID + " " + buffTarget.Name+"'s Hp decrease 1/8 because of solar power attribute");
                await DamageMaxHpByPercentage(null, buffTarget, 1 / 8f, null);
            }

            return input;
        };
        
        public static readonly Func<CommonResult, Pokemon, Pokemon, BuffRecorder, UniTask<CommonResult>> RecoverStatAtWeatherEnd = async (input, _, buffTarget, recorder) =>
        {
            switch (BattleMgr.Instance.GetWeather())
            {
                case Weather.HarshSunlight:
                case Weather.ExtremelyHarshSunlight:
                case Weather.StrongSunLight:
                    switch (recorder.Template.ChangeStat)
                    {
                        case PokemonStat.Speed:
                            buffTarget.Speed = (int)(buffTarget.Speed - buffTarget.Speed * 0.5f);
                            BattleMgr.Instance.UpdateSkillPriority();
                            break;
                        case PokemonStat.SpecialAttack:
                            buffTarget.SpecialAttack = (int)(buffTarget.SpecialAttack - buffTarget.SpecialAttack * 2/3f);
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

        public static readonly Func<CommonResult, Pokemon, Pokemon, BuffRecorder, UniTask<CommonResult>> StaticBuff = async (input, _, buffTarget, _) =>
        {
            if (input.STemplate == null)
            {
                return input;
            }

            // here we make it simpler, every physical is touch skill
            if (input.STemplate.SkillType == SkillType.Physical)
            {
                SkillTemplate temp = new SkillTemplate
                {
                    SpecialEffectProb = 0.3f
                };
                await TryAddParalysis(buffTarget, input.SkillSource, temp);
            }

            return input;
        };

        public static readonly Func<BeforeDamageApplyResult, Pokemon, Pokemon, BuffRecorder, UniTask<BeforeDamageApplyResult>> LightningRod = async (input, _, buffTarget, _) =>
        {
            if (input.Priority > (int)DamageApplyPriority.LightningRod)
            {
                return input;
            }

            if (input.Template.Type != PokemonType.Electric)
            {
                return input;
            }

            input.ShouldSuccess = false;
            input.Priority = (int)DamageApplyPriority.LightningRod;
            input.Message = "No Effect, " + buffTarget.Name + "'s special attack increase by 2";
            buffTarget.SetStatusChange(PokemonStat.SpecialAttack, 2);
            await UniTask.Yield();
            return input;
        };

        public static readonly Func<CommonResult, Pokemon, Pokemon, BuffRecorder, UniTask<CommonResult>> Limber = async (input, _, buffTarget, _) =>
        {
            if (input.BuffKey == 8)
            {
                input.CanAddBuff = false;
            }
            if (input.BuffKey == 9)
            {
                BattleMgr.Instance.SetCommandText(buffTarget.Name + " won't get paralysis because of limber");
                await UniTask.Delay(BattleMgr.Instance.AwaitTime);
                input.CanAddBuff = false;
            }

            return input;
        };

        public static readonly Func<CommonResult, Pokemon, Pokemon, BuffRecorder, UniTask<CommonResult>> ImposterDebut = async (input, _, buffTarget, _) =>
        {
            BattleMgr.Instance.LoadPokemonSkillDirectlyImm(buffTarget, 144);
            await UniTask.Yield();
            return input;
        };

        public static readonly Func<CommonResult, Pokemon, Pokemon, BuffRecorder, UniTask<CommonResult>> SheerForce = async (input, _, buffTarget, _) =>
        {
            if (input.STemplate.ProcedureFunctions.Length <= 1)
            {
                return input;
            }

            bool hasDamageSkill = false;
            foreach (var function in input.STemplate.ProcedureFunctions)
            {
                if (function == TryApplyDamage)
                {
                    hasDamageSkill = true;
                }
            }

            if (!hasDamageSkill)
            {
                return input;
            }

            BattleMgr.Instance.SetCommandText(buffTarget.Name + "'s sheer force activated");
            await UniTask.Delay(BattleMgr.Instance.AwaitTime);

            foreach (var target in input.TargetsList)
            {
                foreach (var func in input.STemplate.ProcedureFunctions)
                {
                    if (func == TryApplyDamage)
                    {
                        var template = SkillTemplate.Copy(input.STemplate);
                        template.Power = (int)(template.Power * 1.3f);
                        bool hit = await HitAccuracyCalculate(buffTarget, target, input.STemplate);
                        int damage;
                        if (!hit)
                        {
                            BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText("But it doesn't hit " + target.Name);
                            await UniTask.Delay(BattleMgr.Instance.AwaitTime);
                        }

                        var success = await CanApplyDamage(buffTarget, target, input.STemplate);
                        if (success)
                        {
                            var result = await DamageCalculate(buffTarget, target, input.STemplate);
                            PrintSkillEffectResult(result.Item1);
                            damage = -result.Item2;
                            target.ChangeHp(damage); // sheer force doesn't trigger post-damage effect: https://bulbapedia.bulbagarden.net/wiki/Sheer_Force_(Ability)
                            await UniTask.Delay(BattleMgr.Instance.AwaitTime);
                        }
                        else
                        {
                            PrintSkillEffectResult(SkillEffect.NotEffective);
                            await UniTask.Delay(BattleMgr.Instance.AwaitTime);
                            damage = 0;
                        }

                        BattleMgr.Instance.BattleStack[^1].Damages ??= new Dictionary<Pokemon, int>();
                        BattleMgr.Instance.BattleStack[^1].Damages.Add(target, damage);
                        break;
                    }
                    else
                    {
                        await func(buffTarget, target, input.STemplate);
                    }
                }
            }

            input.CanMove = false;
            return input;
        };

        public static readonly Func<CommonResult, Pokemon, Pokemon, BuffRecorder, UniTask<CommonResult>> CursedBody = async (input, _, buffTarget, recorder) =>
        {
            if (ProbTrigger(recorder.Template.SpecialEffectProb))
            {
                BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(buffTarget.Name + "'s cursed body activated");
                await UniTask.Delay(BattleMgr.Instance.AwaitTime);
                BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetCommandText(input.SkillSource.Name + " can not use " + input.STemplate.Name);
                await UniTask.Delay(BattleMgr.Instance.AwaitTime);
                var buffRecorder = await BuffMgr.Instance.AddBuff(buffTarget, input.SkillSource, 15);
                buffRecorder.EffectLastRound = 3;
                buffRecorder.ForbiddenSkill = input.STemplate;
            }

            return input;
        };
    }
}