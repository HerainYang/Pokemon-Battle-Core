using System;
using Cysharp.Threading.Tasks;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;
using TalesOfRadiance.Scripts.Battle.Managers;
using TalesOfRadiance.Scripts.Character;
using UnityEngine;

namespace TalesOfRadiance.Scripts.Battle.BattleComponents.BattleLogics
{
    public static partial class BattleLogic
    {
        public static Func<SkillResult, ABattleEntity, SkillTemplate, UniTask<SkillResult>> SelectOneRandomEnemy = async (input, hero, arg3) =>
        {
            CharacterTeam enemyTeam = null;
            foreach (var team in BattleMgr.Instance.OnStageTeam)
            {
                if (!hero.Team.Equals(team))
                {
                    enemyTeam = team;
                    break;
                }
            }

            if (enemyTeam == null)
            {
                Debug.Log("Enemy team should not be null");
                return null;
            }

            foreach (var potentialTarget in enemyTeam.Heroes)
            {
                if (potentialTarget.Properties.IsAlive)
                {
                    input.TargetHeroes.Add(potentialTarget);
                    break;
                }
            }

            

            await UniTask.Yield();
            
            return input;
        };
    }
}