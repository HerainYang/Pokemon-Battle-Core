using System;
using Cysharp.Threading.Tasks;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;
using UnityEngine;

namespace TalesOfRadiance.Scripts.Battle.BattleComponents.BattleLogics
{
    public static partial class BattleLogic
    {
        public static readonly Func<SkillResult, ABattleEntity, RuntimeHero, SkillTemplate, UniTask<SkillResult>> NormalAttack = async (input, entity, arg3, arg4) =>
        {
            Debug.LogWarning(((RuntimeHero)entity).Template.Name + " trying to attack " + input.TargetHeroes[0].Template.Name);
            await UniTask.Delay(1000);
            return input;
        };
    }
}