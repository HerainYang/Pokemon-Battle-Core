using System;
using CoreScripts.BattleComponents;
using Cysharp.Threading.Tasks;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;
using UnityEngine;

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
            Debug.LogWarning(sourceEntity.Template.Name + " trying to attack " + targetEntity.Template.Name + " and deal damage: " + damage);
            await targetEntity.SetTargetHp(damage);
            EffectMgr.Instance.RenderLineFromTo(sourceEntity.Anchor.transform.position, targetEntity.Anchor.transform.position);
            await UniTask.Delay(1000);
            skillResult.Damage = damage;
            return input;
        };

        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> CleanAllNegativeBuff = async (input, entity, target, skillTemplate) =>
        {
            await UniTask.Yield();
            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> HealWithExceedShield = async (input, entity, target, skillTemplate) =>
        {
            await UniTask.Yield();
            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> TryAddContinuousHealBuff = async (input, entity, target, skillTemplate) =>
        {
            await UniTask.Yield();
            return input;
        };
        
    }
}