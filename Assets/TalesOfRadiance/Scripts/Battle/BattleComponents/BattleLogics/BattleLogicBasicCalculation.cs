using System;
using CoreScripts.BattleComponents;
using Cysharp.Threading.Tasks;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;
using TalesOfRadiance.Scripts.Battle.Constant;
using TalesOfRadiance.Scripts.Battle.Managers;


namespace TalesOfRadiance.Scripts.Battle.BattleComponents.BattleLogics
{
    public static partial class BattleLogic
    {
        private static SkillResult ApplyNormalDamage(float attackDamageRough, RuntimeHero target)
        {
            float battleDefense = GetBattleDefense(target);
            float defenseCoe = ((battleDefense) / (battleDefense + 598 + 2 * 255));
            float damage = attackDamageRough * (1 - defenseCoe);

            return new SkillResult()
            {
                Damage = (int)-damage
            };
        }

        private static float GetBattleDefense(RuntimeHero hero)
        {
            float battleDefense = hero.Properties.Defence;
            return battleDefense;
        }


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

        private static async UniTask TryAddBuff(RuntimeHero sourceEntity, RuntimeHero targetEntity, int index)
        {
            var buffTemplate = ConfigManager.Instance.GetBuffTemplateByID(index);
            var buffRecorders = BuffMgr.Instance.GetBuffListByTargetAndBuffID(targetEntity, index);
            if (buffTemplate.HaveBuffNumberLimit)
            {
                var buffCount = buffRecorders.Count;
                if (buffCount > buffTemplate.BuffNumberLimit)
                    return;
            }

            if (buffTemplate.MultiLayerBuff)
            {
                if (buffRecorders.Count != 0)
                {
                    if (buffRecorders[0].BuffLayerCount > buffTemplate.BuffNumberLimit)
                    {
                        return;
                    }

                    buffRecorders[0].BuffLayerCount += 1;
                    return;
                }
            }

            await BuffMgr.Instance.AddBuff(sourceEntity, targetEntity, index);
        }

        private static async UniTask<bool> TrySetTargetHpActive(IBattleEntity source, IBattleEntity target, SkillTemplate template, int damage, bool isDamage = true)
        {
            var sourceEntity = (RuntimeHero)source;
            var targetEntity = (RuntimeHero)target;
            SkillResult tempSkillResult = null;
            if (isDamage)
            {
                tempSkillResult = (SkillResult)await BuffMgr.Instance.ExecuteBuff(Constant.Constant.BuffEventKey.BeforeApplyDamageActive, new SkillResult()
                {
                    Damage = damage,
                    SkillSource = source,
                    SkillTarget = target,
                    SkillTemplate = template,
                    CurrentDamageDonePriority = template.DamageDonePriority
                }, targetEntity);
                tempSkillResult = (SkillResult)await BuffMgr.Instance.ExecuteBuff(Constant.Constant.BuffEventKey.BeforeDamageActive, new SkillResult()
                {
                    Damage = tempSkillResult.Damage,
                    SkillSource = source,
                    SkillTarget = target,
                    SkillTemplate = template,
                    CurrentDamageDonePriority = template.DamageDonePriority
                }, targetEntity);
                if (!tempSkillResult.DamageShouldBeDone)
                {
                    return false;
                }
            }
            else
            {
                tempSkillResult = (SkillResult)await BuffMgr.Instance.ExecuteBuff(Constant.Constant.BuffEventKey.BeforeApplyDamageActive, new SkillResult()
                {
                    Damage = damage,
                    SkillSource = source,
                    SkillTarget = target,
                    SkillTemplate = template,
                    CurrentDamageDonePriority = template.DamageDonePriority
                }, targetEntity);
                tempSkillResult = (SkillResult)await BuffMgr.Instance.ExecuteBuff(Constant.Constant.BuffEventKey.BeforeHealActive, new SkillResult()
                {
                    Damage = tempSkillResult.Damage,
                    SkillSource = source,
                    SkillTarget = target,
                    SkillTemplate = template,
                    CurrentDamageDonePriority = template.DamageDonePriority
                }, targetEntity);
                if (!tempSkillResult.HealShouldBeDone)
                {
                    return false;
                }
            }

            sourceEntity.Anchor.ShowEffectText(template.Name);
            await targetEntity.SetTargetHp(tempSkillResult.Damage);

            if (isDamage)
            {
                await BuffMgr.Instance.ExecuteBuff(Constant.Constant.BuffEventKey.AfterDamageActive, new SkillResult()
                {
                    Damage = tempSkillResult.Damage,
                    SkillSource = source,
                    SkillTarget = target,
                    SkillTemplate = template,
                }, targetEntity);
                await BuffMgr.Instance.ExecuteBuff(Constant.Constant.BuffEventKey.AfterApplyDamageActive, new SkillResult()
                {
                    Damage = tempSkillResult.Damage,
                    SkillSource = source,
                    SkillTarget = target,
                    SkillTemplate = template,
                }, sourceEntity);
            }
            else
            {
                await BuffMgr.Instance.ExecuteBuff(Constant.Constant.BuffEventKey.AfterHealActive, new SkillResult()
                {
                    Damage = tempSkillResult.Damage,
                    SkillSource = source,
                    SkillTarget = target,
                    SkillTemplate = template,
                }, targetEntity);
                await BuffMgr.Instance.ExecuteBuff(Constant.Constant.BuffEventKey.AfterApplyHealActive, new SkillResult()
                {
                    Damage = tempSkillResult.Damage,
                    SkillSource = source,
                    SkillTarget = target,
                    SkillTemplate = template,
                }, sourceEntity);
            }

            return true;
        }

        private static async UniTask<bool> TrySetTargetHpPassive(IBattleEntity source, IBattleEntity target, SkillTemplate template, int damage, bool isDamage = true)
        {
            var sourceEntity = (RuntimeHero)source;
            var targetEntity = (RuntimeHero)target;
            SkillResult tempSkillResult = null;
            if (isDamage)
            {
                tempSkillResult = (SkillResult)await BuffMgr.Instance.ExecuteBuff(Constant.Constant.BuffEventKey.BeforeApplyDamagePassive, new SkillResult()
                {
                    Damage = damage,
                    SkillSource = source,
                    SkillTarget = target,
                    SkillTemplate = template,
                    CurrentDamageDonePriority = template.DamageDonePriority
                }, targetEntity);
                tempSkillResult = (SkillResult)await BuffMgr.Instance.ExecuteBuff(Constant.Constant.BuffEventKey.BeforeDamagePassive, new SkillResult()
                {
                    Damage = tempSkillResult.Damage,
                    SkillSource = source,
                    SkillTarget = target,
                    SkillTemplate = template,
                    CurrentDamageDonePriority = template.DamageDonePriority
                }, targetEntity);
                if (!tempSkillResult.DamageShouldBeDone)
                {
                    return false;
                }
            }
            else
            {
                tempSkillResult = (SkillResult)await BuffMgr.Instance.ExecuteBuff(Constant.Constant.BuffEventKey.BeforeApplyDamagePassive, new SkillResult()
                {
                    Damage = damage,
                    SkillSource = source,
                    SkillTarget = target,
                    SkillTemplate = template,
                    CurrentDamageDonePriority = template.DamageDonePriority
                }, targetEntity);
                tempSkillResult = (SkillResult)await BuffMgr.Instance.ExecuteBuff(Constant.Constant.BuffEventKey.BeforeHealPassive, new SkillResult()
                {
                    Damage = tempSkillResult.Damage,
                    SkillSource = source,
                    SkillTarget = target,
                    SkillTemplate = template,
                    CurrentDamageDonePriority = template.DamageDonePriority
                }, targetEntity);
                if (!tempSkillResult.HealShouldBeDone)
                {
                    return false;
                }
            }

            sourceEntity.Anchor.ShowEffectText(template.Name);
            await targetEntity.SetTargetHp(tempSkillResult.Damage);

            if (isDamage)
            {
                await BuffMgr.Instance.ExecuteBuff(Constant.Constant.BuffEventKey.AfterDamagePassive, new SkillResult()
                {
                    Damage = tempSkillResult.Damage,
                    SkillSource = source,
                    SkillTarget = target,
                    SkillTemplate = template,
                }, targetEntity);
                await BuffMgr.Instance.ExecuteBuff(Constant.Constant.BuffEventKey.AfterApplyDamagePassive, new SkillResult()
                {
                    Damage = tempSkillResult.Damage,
                    SkillSource = source,
                    SkillTarget = target,
                    SkillTemplate = template,
                }, sourceEntity);
            }
            else
            {
                await BuffMgr.Instance.ExecuteBuff(Constant.Constant.BuffEventKey.AfterHealPassive, new SkillResult()
                {
                    Damage = tempSkillResult.Damage,
                    SkillSource = source,
                    SkillTarget = target,
                    SkillTemplate = template,
                }, targetEntity);
                await BuffMgr.Instance.ExecuteBuff(Constant.Constant.BuffEventKey.AfterApplyHealPassive, new SkillResult()
                {
                    Damage = tempSkillResult.Damage,
                    SkillSource = source,
                    SkillTarget = target,
                    SkillTemplate = template,
                }, sourceEntity);
            }

            return true;
        }


        private static Types.Position GetPosition(int positionIndex)
        {
            switch (positionIndex)
            {
                case 0:
                case 3:
                case 6:
                    return Types.Position.Front;
                case 1:
                case 4:
                case 7:
                    return Types.Position.Mid;
                case 2:
                case 5:
                case 8:
                    return Types.Position.Mid;
            }

            throw new Exception("It should be impossible to reach here!");
        }
    }
}