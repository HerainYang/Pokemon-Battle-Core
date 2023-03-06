using System;
using CoreScripts.BattleComponents;
using Cysharp.Threading.Tasks;
using PokemonDemo.Scripts.Managers;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;
using TalesOfRadiance.Scripts.Battle.BattlePlayables;
using TalesOfRadiance.Scripts.Battle.Managers;
using UnityEngine;
using BuffMgr = TalesOfRadiance.Scripts.Battle.Managers.BuffMgr;

namespace TalesOfRadiance.Scripts.Battle.BattleComponents.BattleLogics
{
    public static partial class BattleLogic
    {
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> NormalAttack = async (input, entity, target, skillTemplate) =>
        {
            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;

            var damage = ApplyNormalDamage(sourceEntity.Properties.Attack * template.DamageIncreaseRate, targetEntity).Damage;
            SkillResult tempSkillResult = (SkillResult)await BuffMgr.Instance.ExecuteBuff(Constant.Constant.BuffEventKey.BeforeDamage, new SkillResult()
            {
                Damage = damage,
                SkillSource = entity,
                SkillTarget = target,
                CurrentDamageDonePriority = template.DamageDonePriority
            }, targetEntity);
            if (!tempSkillResult.DamageShouldBeDone)
            {
                skillResult.ContinueProcedureFunction = false;
                return input;
            }
            sourceEntity.Anchor.ShowEffectText(template.Name);
            await targetEntity.SetTargetHp(damage); 
            await BuffMgr.Instance.ExecuteBuff(Constant.Constant.BuffEventKey.AfterDamage, new SkillResult()
            {
                Damage = damage,
                SkillSource = entity,
                SkillTarget = target
            }, targetEntity);

            await BuffMgr.Instance.ExecuteBuff(Constant.Constant.BuffEventKey.AfterApplyDamage, new SkillResult()
            {
                Damage = damage,
                SkillSource = entity,
                SkillTarget = target
            }, sourceEntity);
            EffectMgr.Instance.RenderLineFromTo(sourceEntity.Anchor.transform.position, targetEntity.Anchor.transform.position);
            await UniTask.Delay(BattleMgr.Instance.AnimationAwaitTime);
            skillResult.Damage = damage;
            return input;
        };

        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> CleanAllNegativeBuff = async (input, entity, target, skillTemplate) =>
        {
            await BuffMgr.Instance.RemoveAllNegativeBuffByTarget(target);
            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> HealWithExceedShield = async (input, entity, target, skillTemplate) =>
        {
            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;
            int health = (int)(template.PercentageDamageRate * targetEntity.Properties.MaxHealth);
            await targetEntity.SetTargetHp(health, true);
            await BuffMgr.Instance.ExecuteBuff(Constant.Constant.BuffEventKey.AfterHeal, new SkillResult()
            {
                Damage = health,
                SkillSource = entity,
                SkillTarget = target
            }, targetEntity);
            return input;
        };
        
        
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> TryAddContinuousHealBuff = async (input, entity, target, skillTemplate) =>
        {
            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;
            await BuffMgr.Instance.AddBuff(sourceEntity, targetEntity, 4);
            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> TryAddBuffForTest = async (input, entity, target, skillTemplate) =>
        {
            await BuffMgr.Instance.AddBuff(entity, target, 0);
            return input;
        };
        
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> TryApplyPercentageDamage = async (input, entity, target, skillTemplate) =>
        {
            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;
            int damage = (int)(template.PercentageDamageRate * targetEntity.Properties.MaxHealth);
            SkillResult tempSkillResult = (SkillResult)await BuffMgr.Instance.ExecuteBuff(Constant.Constant.BuffEventKey.BeforeDamage, new SkillResult()
            {
                Damage = damage,
                SkillSource = entity,
                SkillTarget = target,
                CurrentDamageDonePriority = template.DamageDonePriority
            }, targetEntity);
            if (!tempSkillResult.DamageShouldBeDone)
            {
                skillResult.ContinueProcedureFunction = false;
                return input;
            }
            await targetEntity.SetTargetHp(-damage );
            await BuffMgr.Instance.ExecuteBuff(Constant.Constant.BuffEventKey.AfterDamage, new SkillResult()
            {
                Damage = damage,
                SkillSource = entity,
                SkillTarget = target
            }, targetEntity);
            await BuffMgr.Instance.ExecuteBuff(Constant.Constant.BuffEventKey.AfterApplyDamage, new SkillResult()
            {
                Damage = damage,
                SkillSource = entity,
                SkillTarget = target
            }, sourceEntity);
            skillResult.Damage = -damage;
            return input;
        };

        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> TryAddBuffInBuffList = async (input, entity, target, skillTemplate) =>
        {
            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;

            for (int i = 0; i < template.AddBuffPossibility.Length; i++)
            {
                if (!ProbTrigger(template.AddBuffPossibility[i]))
                {
                    skillResult.ContinueProcedureFunction = false;
                    continue;
                }

                foreach (var index in template.AddBuffIndex[i])
                {
                    var buffTemplate = ConfigManager.Instance.GetBuffTemplateByID(index);
                    if (buffTemplate.OnlyOneBuffShouldExist && BuffMgr.Instance.ExistActiveBuff(target, buffTemplate))
                    {
                        return input;
                    }
                    await BuffMgr.Instance.AddBuff(sourceEntity, targetEntity, index);
                }
            }

            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> TryAddBuffInInputStealBuffList = async (input, entity, target, skillTemplate) =>
        {
            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;
            

            foreach (var index in skillResult.StealBuffIDs)
            {
                await BuffMgr.Instance.AddBuff(sourceEntity, targetEntity, index);
            }
            
            return input;
        };

        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> TryBringBackToLife = async (input, entity, target, skillTemplate) =>
        {
            if (target == null)
                return input;

            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;

            BpDebut bpDebut = new BpDebut(targetEntity);

            var handler = EventMgr.Instance.ListenTo(CoreScripts.Constant.Constant.ListenToEvent.BattlePlayableReturnOwnerShip + skillResult.SelfPlayable.RuntimeID);
            BattleMgr.Instance.BorrowControlToPendingPlayable(skillResult.SelfPlayable, bpDebut);
            
            await UniTask.WaitUntil(() => handler.Complete);


            targetEntity.Properties.Hp = (int)(targetEntity.Properties.MaxHealth * 0.1);

            await UniTask.Delay(1000);

            return input;
        };

        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> ExecuteNextSkillPlayable = async (input, entity, target, skillTemplate) =>
        {
            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;

            var nextSkillTemplate = ConfigManager.Instance.GetSkillTemplateByID(template.NextSkillID);
            var playable = await nextSkillTemplate.SendLoadSkillRequest(sourceEntity);
            
            skillResult.CallDefaultPlayableDestroyFunction = false;
            BattleMgr.Instance.TransferControlToPendingPlayable(playable);

            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> TryHealByAttack = async (input, entity, target, skillTemplate) =>
        {
            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;
            int health = (int)(template.DamageIncreaseRate * sourceEntity.Properties.Attack);
            await targetEntity.SetTargetHp(health);
            await BuffMgr.Instance.ExecuteBuff(Constant.Constant.BuffEventKey.AfterHeal, new SkillResult()
            {
                Damage = health,
                SkillSource = entity,
                SkillTarget = target
            }, targetEntity);
            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> StealOneBuff = async (input, entity, target, skillTemplate) =>
        {
            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;
            
            EffectMgr.Instance.RenderLineFromTo(sourceEntity.Anchor.transform.position, targetEntity.Anchor.transform.position);
            var buff = await BuffMgr.Instance.RemoveOnePositiveBuffByTarget(targetEntity);
            if(buff != null)
            {
                skillResult.StealBuffIDs.Add(buff.ID);
            }
            
            await UniTask.Delay(BattleMgr.Instance.AnimationAwaitTime);
            
            
            
            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> LoopSkillNTime = async (input, entity, target, skillTemplate) =>
        {
            var skillResult = (SkillResult)input;
            var sourceEntity = (RuntimeHero)entity;
            var targetEntity = (RuntimeHero)target;
            var template = (SkillTemplate)skillTemplate;


            SkillTemplate loopSkillTemplate = ConfigManager.Instance.GetSkillTemplateByID(template.LoopSkillID);
            SkillResult prototype = new SkillResult();
            if (template.LoopLoadPreLoadData)
            {
                prototype = await loopSkillTemplate.GetPreloadData(sourceEntity);
            }
            else
            {
                prototype.TargetHeroes.Add(targetEntity);
            }
            
            for (int i = 0; i < template.LoopTime; i++)
            {
                SkillResult temp = prototype.Copy();
                BpSkill bpSkill = new BpSkill(sourceEntity, loopSkillTemplate, temp);
                
                var handler = EventMgr.Instance.ListenTo(CoreScripts.Constant.Constant.ListenToEvent.BattlePlayableReturnOwnerShip + skillResult.SelfPlayable.RuntimeID);
                BattleMgr.Instance.BorrowControlToPendingPlayable(skillResult.SelfPlayable, bpSkill);
                await UniTask.WaitUntil(() => handler.Complete);
            }
            
            
            
            return input;
        };
    }
}