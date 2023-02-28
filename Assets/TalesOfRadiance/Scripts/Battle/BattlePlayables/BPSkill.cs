using System;
using System.Collections.Generic;
using System.Linq;
using CoreScripts.BattleComponents;
using CoreScripts.BattlePlayables;
using Cysharp.Threading.Tasks;
using Managers.BattleMgrComponents;
using TalesOfRadiance.Scripts.Battle.BattleComponents;
using TalesOfRadiance.Scripts.Battle.BattleComponents.BattleLogics;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;
using UnityEngine;
using BattleMgr = TalesOfRadiance.Scripts.Battle.Managers.BattleMgr;
using Types = TalesOfRadiance.Scripts.Battle.Constant.Types;

namespace TalesOfRadiance.Scripts.Battle.BattlePlayables
{
    public class BpSkill : ABattlePlayable
    {
        private SkillTemplate _skillTemplate;
        private SkillResult _skillResult;
        public BpSkill(AtorBattleEntity entity, SkillTemplate template, SkillResult preloadData) : base((int)Types.PlayablePriority.None)
        {
            Source = entity;
            _skillTemplate = template;
            _skillResult = preloadData;
            _skillResult.SelfPlayable = this;
        }
        
#if UNITY_EDITOR
        public SkillResult GetPreLoadData()
        {
            return _skillResult;
        }
        
        public SkillTemplate GetSkillTemplate()
        {
            return _skillTemplate;
        }
#endif

        public override async void Execute()
        {
            if (Available == false)
            {
                OnDestroy();
                return;
            }
            if (_skillTemplate.ProcedureFunctions == null)
                return;
            var callDefaultDestroy = await ExecuteList();

            if(callDefaultDestroy)
                OnDestroy();
        }

        protected override void OnDestroy()
        {
            BattleMgr.Instance.BattlePlayableEnd();
        }
        
        private async UniTask<bool> ExecuteList()
        {
            if (_skillResult.TargetHeroes == null)
            {
                Debug.LogError("No target");
                return true;
            }


            List<UniTask<ASkillResult>> waitingList = new List<UniTask<ASkillResult>>();
            foreach (var target in _skillResult.TargetHeroes)
            {
                waitingList.Add(ExecuteToOneTarget(target));
            }

            var results = await waitingList;

            List<Tuple<IBattleEntity, ASkillResult>> callBackParamList = new List<Tuple<IBattleEntity, ASkillResult>>();
            for (int i = 0; i < _skillResult.TargetHeroes.Count; i++)
            {
                callBackParamList.Add(new Tuple<IBattleEntity, ASkillResult>(_skillResult.TargetHeroes[i], results[i]));
            }

            SkillResult callBackResult = new SkillResult();
            if (_skillTemplate.OnProcedureFunctionsEndCallBacks != null)
            {
                foreach (var endCallBack in _skillTemplate.OnProcedureFunctionsEndCallBacks)
                {
                    callBackResult = (SkillResult)await endCallBack(callBackParamList, callBackResult, Source, _skillTemplate);
                }
            }

            callBackResult = (SkillResult)await BattleLogic.CheckIfCallDefaultPlayableDestroyFunction(callBackParamList, callBackResult, Source, _skillTemplate);


            return callBackResult.CallDefaultPlayableDestroyFunction;
        }

        private async UniTask<ASkillResult> ExecuteToOneTarget(RuntimeHero target)
        {
            var localResult = _skillResult.Copy();
            foreach (var procedureFunction in _skillTemplate.ProcedureFunctions)
            {
                localResult = (SkillResult)await procedureFunction(localResult, ((AtorBattleEntity)Source), target, _skillTemplate);
                if (!localResult.ContinueProcedureFunction)
                {
                    return localResult;
                }
            }

            return localResult;
        }
    }
}