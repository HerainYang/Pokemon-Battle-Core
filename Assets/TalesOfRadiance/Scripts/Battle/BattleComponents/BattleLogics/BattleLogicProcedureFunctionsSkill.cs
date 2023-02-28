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
                SkillTarget = target
            }, targetEntity);
            if (!tempSkillResult.DamageShouldBeDone)
            {
                return null;
            }
            if (!tempSkillResult.DamageShouldBeDone)
            {
                return null;
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
            await UniTask.Delay(1000);
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
            Debug.Log("Buff Added");
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
                SkillTarget = target
            }, targetEntity);
            if (!tempSkillResult.DamageShouldBeDone)
            {
                return null;
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
                    continue;
                }

                foreach (var index in template.AddBuffIndex[i])
                {
                    await BuffMgr.Instance.AddBuff(sourceEntity, targetEntity, index);
                }
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

            BattleMgr.Instance.BorrowControlToPendingPlayable(skillResult.SelfPlayable, bpDebut);
            var handler = EventMgr.Instance.ListenTo("BATTLE_PLAYABLE_RETURN_OWNER_SHIP_" + skillResult.SelfPlayable.GetHashCode());
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
            
            Debug.LogWarning(targetEntity.Template.Name);
            var buff = await BuffMgr.Instance.GetOnePositiveBuffByTarget(targetEntity);
            Debug.LogWarning(buff.Name);
            
            return input;
        };
    }
}