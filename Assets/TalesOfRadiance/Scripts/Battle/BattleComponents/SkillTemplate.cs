using System;
using CoreScripts.BattleComponents;
using CoreScripts.BattlePlayables;
using Cysharp.Threading.Tasks;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;
using TalesOfRadiance.Scripts.Battle.BattlePlayables;
using TalesOfRadiance.Scripts.Battle.Constant;

namespace TalesOfRadiance.Scripts.Battle.BattleComponents
{
    public class SkillTemplate : ASkillTemplate
    {
        public int InitCd;
        public int Cd;
        
        // template attributes
        public float DamageIncreaseRate;
        
        // buff
        public readonly Delegate Callback;
        public float BuffLastRound;
        public Types.BuffType BuffType;

        public SkillTemplate(int skillID, string skillName, Func<SkillResult, ATORBattleEntity, RuntimeHero, SkillTemplate, UniTask<SkillResult>>[] procedureFunctions, Func<SkillResult, ATORBattleEntity, SkillTemplate, UniTask<SkillResult>>[] onLoadRequest = null)
        {
            ID = skillID;
            Name = skillName;
        }
        
        public SkillTemplate(int id, string name, int effectRound, Delegate callback, Func<IBattleEntity, IBattleEntity, ABuffRecorder, UniTask> onDestroyCallBack, string buffTriggerEvent)
        {
            BuffLastRound = effectRound;
            Callback = callback;
            BuffTriggerEvent = buffTriggerEvent;
            ID = id;
            Name = name;
            OnDestroyCallBack = onDestroyCallBack;
        }

        public async UniTask<ABattlePlayable> SendLoadSkillRequest(ATORBattleEntity sourceEntity, SkillResult input = null)
        {
            if (input == null)
                input = new SkillResult();
            input.SkillID = ID;
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
                    input = (SkillResult)result;
                }
            }

            return new BpSkill(sourceEntity, this, input);
        }
    }
}