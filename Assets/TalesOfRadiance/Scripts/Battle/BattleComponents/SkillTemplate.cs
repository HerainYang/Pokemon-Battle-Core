using System;
using CoreScripts.BattleComponents;
using CoreScripts.BattlePlayables;
using Cysharp.Threading.Tasks;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;
using TalesOfRadiance.Scripts.Battle.BattlePlayables;
using UnityEngine;
using Types = TalesOfRadiance.Scripts.Battle.Constant.Types;

namespace TalesOfRadiance.Scripts.Battle.BattleComponents
{
    public class SkillTemplate : ASkillTemplate
    {
        public int InitCd;
        public int Cd;
        
        // template attributes
        public float DamageIncreaseRate;
        public Types.SkillType SkillType;
        
        // buff
        public readonly Delegate Callback;
        public int BuffLastRound;
        public Types.BuffType BuffType;

        public SkillTemplate(int skillID, string skillName, Types.SkillType skillType, Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>>[] procedureFunctions, Func<ASkillResult, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>>[] onLoadRequest = null)
        {
            ID = skillID;
            Name = skillName;
            SkillType = skillType;
            ProcedureFunctions = procedureFunctions;
            OnLoadRequest = onLoadRequest;
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
                input = (SkillResult)await BattleLogics.BattleLogic.SelectOneRandomEnemy(input, sourceEntity, this);
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