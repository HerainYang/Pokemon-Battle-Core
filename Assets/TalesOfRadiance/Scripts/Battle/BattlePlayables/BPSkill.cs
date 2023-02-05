using CoreScripts.BattlePlayables;
using Cysharp.Threading.Tasks;
using Enum;
using Managers.BattleMgrComponents;
using Managers.BattleMgrComponents.PokemonLogic;
using TalesOfRadiance.Scripts.Battle.BattleComponents;
using UnityEngine;
using BattleMgr = TalesOfRadiance.Scripts.Battle.Managers.BattleMgr;
using Types = TalesOfRadiance.Scripts.Battle.Constant.Types;

namespace TalesOfRadiance.Scripts.Battle.BattlePlayables
{
    public class BpSkill : ABattlePlayable
    {
        private SkillTemplate _skillTemplate;
        private SkillResult _skillResult;
        public BpSkill(ABattleEntity entity, SkillTemplate template, SkillResult preloadData) : base((int)Types.PlayablePriority.None)
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

            foreach (var target in _skillResult.TargetHeroes)
            {
                for (int i = 0; i < _skillTemplate.ProcedureFunctions.Length; i++)
                {
                    _skillResult = await _skillTemplate.ProcedureFunctions[i](_skillResult, Source, target, _skillTemplate);
                    if (_skillResult == null)
                    {
                        break;
                    }
                }
            }
        }
    }
}