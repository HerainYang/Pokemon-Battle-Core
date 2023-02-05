using System;
using Cysharp.Threading.Tasks;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;
using UnityEngine;

namespace TalesOfRadiance.Scripts.Battle.BattleComponents.BattleLogics
{
    public static partial class BattleLogic
    {
        public static readonly Func<SkillResult, ABattleEntity, RuntimeHero, SkillTemplate, UniTask<SkillResult>> NormalAttack = async (input, entity, target, skillTemplate) =>
        {
            if (entity is not RuntimeHero hero)
            {
                throw new Exception("this is function for hero type");
            }

            var damage = ApplyNormalDamage(hero.Properties.Attack * skillTemplate.DamageIncreaseRate, target).Damage;
            Debug.LogWarning(hero.Template.Name + " trying to attack " + target.Template.Name + " and deal damage: " + damage);
            await target.SetTargetHp(damage);
            EffectMgr.Instance.RenderLineFromTo(hero.Anchor.transform.position, input.TargetHeroes[0].Anchor.transform.position);
            await UniTask.Delay(1000);
            return input;
        };
    }
}