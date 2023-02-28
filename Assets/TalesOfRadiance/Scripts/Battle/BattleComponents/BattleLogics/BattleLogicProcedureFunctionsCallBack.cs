using System;
using System.Collections.Generic;
using System.Linq;
using CoreScripts.BattleComponents;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TalesOfRadiance.Scripts.Battle.BattleComponents.BattleLogics
{
    public partial class BattleLogic
    {
        public static Func<List<Tuple<IBattleEntity, ASkillResult>>, ASkillResult, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>> CheckIfCallDefaultPlayableDestroyFunction = async (procedureResults, input, source, template) =>
        {
            var skillResult = ((SkillResult)input);
            bool callDestroyFunction = true;
            foreach (var tuple in procedureResults.Where(tuple => ((SkillResult)tuple.Item2).CallDefaultPlayableDestroyFunction == false))
            {
                callDestroyFunction = false;
            }

            skillResult.CallDefaultPlayableDestroyFunction = callDestroyFunction;
            return input;
        };
    }
}