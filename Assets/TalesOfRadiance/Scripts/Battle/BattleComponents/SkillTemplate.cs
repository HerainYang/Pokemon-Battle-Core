using System;
using CoreScripts.BattlePlayables;
using Cysharp.Threading.Tasks;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;
using TalesOfRadiance.Scripts.Battle.BattlePlayables;

namespace TalesOfRadiance.Scripts.Battle.BattleComponents
{
    public class SkillTemplate
    {
        public int SkillID;
        public string SkillName;
        public Func<SkillResult, ABattleEntity, RuntimeHero, SkillTemplate, UniTask<SkillResult>>[] ProcedureFunctions;
        public Func<SkillResult, ABattleEntity, SkillTemplate, UniTask<SkillResult>>[] OnLoadRequest;
        
        public int InitCd;
        public int Cd;
        
        // template attributes
        public float DamageIncreaseRate;

        public SkillTemplate(int skillID, string skillName, Func<SkillResult, ABattleEntity, RuntimeHero, SkillTemplate, UniTask<SkillResult>>[] procedureFunctions, Func<SkillResult, ABattleEntity, SkillTemplate, UniTask<SkillResult>>[] onLoadRequest = null)
        {
            SkillID = skillID;
            SkillName = skillName;
            ProcedureFunctions = procedureFunctions;
            OnLoadRequest = onLoadRequest;
        }
        
        public async UniTask<ABattlePlayable> SendLoadSkillRequest(ABattleEntity sourceEntity, SkillResult input = null)
        {
            if (input == null)
                input = new SkillResult();
            input.SkillID = SkillID;
            if (OnLoadRequest == null)
            {
                input = await BattleLogics.BattleLogic.SelectOneRandomEnemy(input, sourceEntity, this);
                if(input == null)
                    return null;
            }
            else
            {
                foreach (var func in OnLoadRequest)
                {
                    var result = await func(input, sourceEntity, this);
                    if(result == null)
                        return null;
                    input = result;
                }
            }

            return new BpSkill(sourceEntity, this, input);
        }
    }
}