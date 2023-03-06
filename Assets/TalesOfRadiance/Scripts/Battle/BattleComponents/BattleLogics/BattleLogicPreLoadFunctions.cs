using System;
using System.Collections.Generic;
using CoreScripts.BattleComponents;
using Cysharp.Threading.Tasks;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;
using TalesOfRadiance.Scripts.Battle.Managers;
using TalesOfRadiance.Scripts.Character;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TalesOfRadiance.Scripts.Battle.BattleComponents.BattleLogics
{
    public static partial class BattleLogic
    {
        private static CharacterTeam GetEnemyTeam(AtorBattleEntity hero)
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
        
        private static CharacterTeam GetSelfTeam(AtorBattleEntity hero)
        {
            return hero.Team;
        }

        public static readonly Func<ASkillResult, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> SelectRandomEnemy = async (input, hero, template) =>
        {
            var runtimeHero = (AtorBattleEntity)hero;
            var preLoadInput = (SkillResult)input;
            var skillTemplate = (SkillTemplate)template;
            
            var enemyTeam = GetEnemyTeam(runtimeHero);
            var potentialList = enemyTeam.Heroes.FindAll(o => o.Properties.IsAlive);
            for (int i = 0; i < skillTemplate.TargetCount; i++)
            {
                if(potentialList.Count == 0)
                    break;
                int index = Random.Range(0, potentialList.Count);
                preLoadInput.TargetHeroes.Add(potentialList[index]);
                potentialList.RemoveAt(index);
            }
            
            await UniTask.Yield();

            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> SelectRandomTeammateExceptSelf = async (input, hero, template) =>
        {
            var runtimeHero = (AtorBattleEntity)hero;
            var preLoadInput = (SkillResult)input;
            var skillTemplate = (SkillTemplate)template;
            
            var selfTeam = GetSelfTeam(runtimeHero);
            var potentialList = selfTeam.Heroes.FindAll(o => o.Properties.IsAlive);
            potentialList.Remove((RuntimeHero)hero);
            for (int i = 0; i < skillTemplate.TargetCount; i++)
            {
                if(potentialList.Count == 0)
                    break;
                int index = Random.Range(0, potentialList.Count);
                preLoadInput.TargetHeroes.Add(potentialList[index]);
                potentialList.RemoveAt(index);
            }
            
            await UniTask.Yield();

            return input;
        };

        public static readonly Func<ASkillResult, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> SelectAppendFront = async (input, hero, arg3) =>
        {
            var runtimeHero = (AtorBattleEntity)hero;
            var preLoadInput = (SkillResult)input;
            
            var enemyTeam = GetEnemyTeam(runtimeHero);
            
            if (enemyTeam.GetHeroByIndex(0) != null && enemyTeam.GetHeroByIndex(3) != null && enemyTeam.GetHeroByIndex(6) != null)
            {
                return await SelectAppendMid(input, hero, arg3);
            }
            
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
                preLoadInput.TargetHeroes.Add(enemyTeam.GetHeroByIndex(6));
            }
            await UniTask.Yield();

            return input;
        };

        public static Func<ASkillResult, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> SelectAppendMid = async (input, hero, arg3) =>
        {
            var runtimeHero = (AtorBattleEntity)hero;
            var preLoadInput = (SkillResult)input;
            
            var enemyTeam = GetEnemyTeam(runtimeHero);
            
            if (enemyTeam.GetHeroByIndex(1) != null && enemyTeam.GetHeroByIndex(4) != null && enemyTeam.GetHeroByIndex(7) != null)
            {
                return await SelectAppendBack(input, hero, arg3);
            }
            
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
            var runtimeHero = (AtorBattleEntity)hero;
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

        public static readonly Func<ASkillResult, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> SelectSelf = async (input, hero, arg3) =>
        {
            var preLoadInput = (SkillResult)input;
            preLoadInput.TargetHeroes.Add((RuntimeHero)hero);

            await UniTask.Yield();
            return input;
        };

        public static readonly Func<ASkillResult, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> SelectAllTeammate = async (input, hero, template) =>
        {
            var runtimeHero = (AtorBattleEntity)hero;
            var preLoadInput = (SkillResult)input;

            var selfTeam = GetSelfTeam(runtimeHero);
            foreach (var teammate in selfTeam.Heroes)
            {
                preLoadInput.TargetHeroes.Add(teammate);
            }
            
            await UniTask.Yield();

            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> SelectAllEnemy = async (input, hero, template) =>
        {
            var runtimeHero = (AtorBattleEntity)hero;
            var preLoadInput = (SkillResult)input;

            var selfTeam = GetEnemyTeam(runtimeHero);
            foreach (var teammate in selfTeam.Heroes)
            {
                preLoadInput.TargetHeroes.Add(teammate);
            }
            
            await UniTask.Yield();

            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> DeselectSelfFromPrevious = async (input, hero, arg3) =>
        {
            var runtimeHero = (AtorBattleEntity)hero;
            var preLoadInput = (SkillResult)input;

            if (preLoadInput.TargetHeroes == null || preLoadInput.TargetHeroes.Count == 0)
            {
                Debug.LogError("Target hero list shouldn't be empty");
                return input;
            }

            preLoadInput.TargetHeroes.Remove((RuntimeHero)hero);
            
            await UniTask.Yield();

            return input;
        };


        public static readonly Func<ASkillResult, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> SelectLowestHpFromPrevious = async (input, hero, template) =>
        {
            var preLoadInput = (SkillResult)input;
            var skillTemplate = (SkillTemplate)template;

            if (preLoadInput.TargetHeroes == null || preLoadInput.TargetHeroes.Count == 0)
            {
                Debug.LogError("Target hero list shouldn't be empty");
                return input;
            }

            List<RuntimeHero> targets = new List<RuntimeHero>();

            for (int i = 0; i < skillTemplate.TargetCount; i++)
            {
                if(preLoadInput.TargetHeroes.Count == 0)
                    break;

                RuntimeHero lowest = preLoadInput.TargetHeroes[0];
                foreach (var runtimeHero in preLoadInput.TargetHeroes)
                {
                    if (runtimeHero.Properties.Hp < lowest.Properties.Hp)
                    {
                        lowest = runtimeHero;
                    }
                }

                targets.Add(lowest);
                preLoadInput.TargetHeroes.Remove(lowest);
            }
            
            preLoadInput.TargetHeroes = targets;
            
            await UniTask.Yield();
            return input;
        };
        
        public static readonly Func<ASkillResult, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> SelectOneFaintTeammate = async (input, hero, arg3) =>
        {
            var runtimeHero = (AtorBattleEntity)hero;
            var preLoadInput = (SkillResult)input;

            var team = GetSelfTeam(runtimeHero);

            var potentialList = team.Heroes.FindAll(o => !o.Properties.IsAlive);
            if (potentialList.Count == 0)
            {
                preLoadInput.TargetHeroes.Add(null);
                return input;
            }
            int index = Random.Range(0, potentialList.Count);
            preLoadInput.TargetHeroes.Add(potentialList[index]);

            await UniTask.Yield();
            return input;
        };
    }
}