using System.Collections.Generic;
using CoreScripts.BattlePlayables;
using Cysharp.Threading.Tasks;
using Managers.BattleMgrComponents;
using TalesOfRadiance.Scripts.Battle.BattleComponents;
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
        public BpSkill(ATORBattleEntity entity, SkillTemplate template, SkillResult preloadData) : base((int)Types.PlayablePriority.None)
        {
            Source = entity;
            _skillTemplate = template;
            _skillResult = preloadData;
        }

        public override async void Execute()
        {
            if (Available == false)
            {
                OnDestroy();
                return;
            }
            if (_skillTemplate.ProcedureFunctions == null)
                return;
            await ExecuteList();
            
            OnDestroy();
        }

        protected override void OnDestroy()
        {
            BattleMgr.Instance.BattlePlayableEnd();
        }
        
        private async UniTask ExecuteList()
        {
            if (_skillResult.TargetHeroes == null)
            {
                Debug.LogError("No target");
                return;
            }


            List<UniTask> waitingList = new List<UniTask>();
            foreach (var target in _skillResult.TargetHeroes)
            {
                waitingList.Add(ExecuteToOneTarget(target));
            }

            await waitingList;
        }

        private async UniTask ExecuteToOneTarget(RuntimeHero target)
        {
            for (int i = 0; i < _skillTemplate.ProcedureFunctions.Length; i++)
            {
                _skillResult = (SkillResult)await _skillTemplate.ProcedureFunctions[i](_skillResult, ((ATORBattleEntity)Source), target, _skillTemplate);
                if (_skillResult == null)
                {
                    break;
                }
            }
        }
    }
}