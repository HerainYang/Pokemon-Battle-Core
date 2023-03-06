using System;
using System.Collections.Generic;
using System.Linq;
using CoreScripts.BattleComponents;
using Cysharp.Threading.Tasks;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;
using TalesOfRadiance.Scripts.Battle.Managers;
using UnityEngine;

namespace TalesOfRadiance.Scripts.Battle.BattleComponents.BattleLogics
{
    public partial class BattleLogic
    {
        public static readonly Func<List<Tuple<IBattleEntity, ASkillResult>>, ASkillResult, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> CheckIfCallDefaultPlayableDestroyFunction = async (procedureResults, input, source, template) =>
        {
            var skillResult = ((SkillResult)input);
            bool callDestroyFunction = skillResult.CallDefaultPlayableDestroyFunction;
            foreach (var tuple in procedureResults.Where(tuple => ((SkillResult)tuple.Item2).CallDefaultPlayableDestroyFunction == false))
            {
                
                callDestroyFunction = false;
            }

            skillResult.CallDefaultPlayableDestroyFunction = callDestroyFunction;

            await UniTask.Yield();
            
            return input;
        };
        
        public static readonly Func<List<Tuple<IBattleEntity, ASkillResult>>, ASkillResult, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> MergeStealBuffIDs = async (procedureResults, input, source, template) =>
        {
            var skillResult = ((SkillResult)input);
            foreach (var tuple in procedureResults)
            {
                SkillResult tempResult = (SkillResult)tuple.Item2;
                skillResult.StealBuffIDs.AddRange(tempResult.StealBuffIDs);
            }

            await UniTask.Yield();
            
            return input;
        };
        
        public static readonly Func<List<Tuple<IBattleEntity, ASkillResult>>, ASkillResult, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> CallBackExecuteNextSkillPlayableWithInput = async (procedureResults, input, source, template) =>
        {

            var skillTemplate = (SkillTemplate)template;
            var sourceEntity = (RuntimeHero)source;
            var skillResult = (SkillResult)input;
            var nextSkillTemplate = ConfigManager.Instance.GetSkillTemplateByID(skillTemplate.NextSkillID);
            var playable = await nextSkillTemplate.SendLoadSkillRequest(sourceEntity, skillResult.Copy());
            
            skillResult.CallDefaultPlayableDestroyFunction = false;
            BattleMgr.Instance.TransferControlToPendingPlayable(playable);

            return input;
        };
    }
}