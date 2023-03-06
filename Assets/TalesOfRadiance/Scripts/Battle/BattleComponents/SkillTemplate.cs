using System;
using System.Collections.Generic;
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

        public float PercentageDamageRate;
        public Types.DamageDonePriority DamageDonePriority;

        public int LoopTime;
        public int LoopSkillID;
        // if is true, it will use skill's preload function to load target and other data
        // if is false, it will use the current target
        public bool LoopLoadPreLoadData = true;
        public int NextSkillID;

        public int TargetCount = 1;
        
        //reason see BattleLogic.TryAddBuffInBuffList
        public int[][] AddBuffIndex;
        public float[] AddBuffPossibility;

        public int[] PassiveSkillBuffList;
        
        // buff
        public int BuffLastRound;
        public Types.BuffType BuffType;
        public float ValueChangeRate;
        public bool RecoverHp;

        public bool OnlyOneBuffShouldExist = false;

        public SkillTemplate(int skillID, string skillName, Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>>[] procedureFunctions, Func<ASkillResult, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>>[] onLoadRequest = null,  Func<List<Tuple<IBattleEntity, ASkillResult>>, ASkillResult, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>>[] onProcedureFunctionsEndCallBacks = null)
        {
            ID = skillID;
            Name = skillName;
            SkillType = Types.SkillType.Active;
            ProcedureFunctions = procedureFunctions;
            OnLoadRequest = onLoadRequest;
            OnProcedureFunctionsEndCallBacks = onProcedureFunctionsEndCallBacks; 
        }
        
        public SkillTemplate(int skillID, string skillName, int[] passiveSkillBuffList, Func<ASkillResult, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>>[] onLoadRequest = null)
        {
            ID = skillID;
            Name = skillName;
            SkillType = Types.SkillType.Passive;
            PassiveSkillBuffList = passiveSkillBuffList;
            OnLoadRequest = onLoadRequest;
        }
        
        public SkillTemplate(int id, string name, int effectRound, Types.BuffType buffType, Func<ASkillResult, IBattleEntity, IBattleEntity, ABuffRecorder, UniTask<ASkillResult>> callback, Func<IBattleEntity, IBattleEntity, ABuffRecorder, UniTask> onDestroyCallBacks, string buffTriggerEvent)
        {
            BuffLastRound = effectRound;
            BuffCallBacks = callback;
            BuffTriggerEvent = buffTriggerEvent;
            ID = id;
            Name = name;
            OnDestroyCallBacks = onDestroyCallBacks;
            BuffType = buffType;
        }

        public SkillTemplate()
        {
            
        }

        public async UniTask<ABattlePlayable> SendLoadSkillRequest(AtorBattleEntity sourceEntity, SkillResult input = null)
        {
            if (input == null)
                input = new SkillResult();
            input.SkillID = ID;
            if (OnLoadRequest == null)
            {
                input = (SkillResult)await BattleLogics.BattleLogic.SelectRandomEnemy(input, sourceEntity, this);
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

        public async UniTask<SkillResult> GetPreloadData(AtorBattleEntity sourceEntity, SkillResult input = null)
        {
            if (input == null)
                input = new SkillResult();
            input.SkillID = ID;
            if (OnLoadRequest == null)
            {
                input = (SkillResult)await BattleLogics.BattleLogic.SelectRandomEnemy(input, sourceEntity, this);
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

            return input;
        }
    }
}