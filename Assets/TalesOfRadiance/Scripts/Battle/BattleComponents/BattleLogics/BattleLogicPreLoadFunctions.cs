using System;
using CoreScripts.BattleComponents;
using Cysharp.Threading.Tasks;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;
using TalesOfRadiance.Scripts.Battle.Managers;
using TalesOfRadiance.Scripts.Character;
using UnityEngine;

namespace TalesOfRadiance.Scripts.Battle.BattleComponents.BattleLogics
{
    public static partial class BattleLogic
    {
        private static CharacterTeam GetEnemyTeam(ATORBattleEntity hero)
        {
            foreach (var team in BattleMgr.Instance.OnStageTeam)
            {
                if (!hero.Team.Equals(team))
                {
                    return team;
                }
            }

            Debug.LogError("Enemy team should not be null");
            return null;
        }

        public static Func<ASkillResult, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> SelectOneRandomEnemy = async (input, hero, arg3) =>
        {
            var runtimeHero = (ATORBattleEntity)hero;
            var preLoadInput = (SkillResult)input;
            
            var enemyTeam = GetEnemyTeam(runtimeHero);
            foreach (var potentialTarget in enemyTeam.Heroes)
            {
                if (potentialTarget.Properties.IsAlive)
                {
                    preLoadInput.TargetHeroes.Add(potentialTarget);
                    break;
                }
            }


            await UniTask.Yield();

            return input;
        };

        public static Func<ASkillResult, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> SelectAppendFront = async (input, hero, arg3) =>
        {
            var runtimeHero = (ATORBattleEntity)hero;
            var preLoadInput = (SkillResult)input;
            
            var enemyTeam = GetEnemyTeam(runtimeHero);
            if (enemyTeam.GetHeroByIndex(0) != null)
            {
                preLoadInput.TargetHeroes.Add(enemyTeam.GetHeroByIndex(0));
            }
            if (enemyTeam.GetHeroByIndex(3) != null)
            {
                preLoadInput.TargetHeroes.Add(enemyTeam.GetHeroByIndex(3));
            }
            if (enemyTeam.GetHeroByIndex(6) != null)
            {
                Debug.LogWarning("Select 6");
                preLoadInput.TargetHeroes.Add(enemyTeam.GetHeroByIndex(6));
            }
            await UniTask.Yield();

            return input;
        };

        public static Func<ASkillResult, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> SelectAppendMid = async (input, hero, arg3) =>
        {
            var runtimeHero = (ATORBattleEntity)hero;
            var preLoadInput = (SkillResult)input;
            
            var enemyTeam = GetEnemyTeam(runtimeHero);
            if (enemyTeam.GetHeroByIndex(1) != null)
            {
                preLoadInput.TargetHeroes.Add(enemyTeam.GetHeroByIndex(1));
            }
            if (enemyTeam.GetHeroByIndex(4) != null)
            {
                preLoadInput.TargetHeroes.Add(enemyTeam.GetHeroByIndex(4));
            }
            if (enemyTeam.GetHeroByIndex(7) != null)
            {
                preLoadInput.TargetHeroes.Add(enemyTeam.GetHeroByIndex(7));
            }
            await UniTask.Yield();

            return input;
        };

        public static Func<ASkillResult, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> SelectAppendBack = async (input, hero, arg3) =>
        {
            var runtimeHero = (ATORBattleEntity)hero;
            var preLoadInput = (SkillResult)input;
            
            var enemyTeam = GetEnemyTeam(runtimeHero);
            if (enemyTeam.GetHeroByIndex(2) != null)
            {
                preLoadInput.TargetHeroes.Add(enemyTeam.GetHeroByIndex(2));
            }

            if (enemyTeam.GetHeroByIndex(5) != null)
            {
                preLoadInput.TargetHeroes.Add(enemyTeam.GetHeroByIndex(5));
            }

            if (enemyTeam.GetHeroByIndex(8) != null)
            {
                preLoadInput.TargetHeroes.Add(enemyTeam.GetHeroByIndex(8));
            }

            await UniTask.Yield();

            return input;
        };
    }
}