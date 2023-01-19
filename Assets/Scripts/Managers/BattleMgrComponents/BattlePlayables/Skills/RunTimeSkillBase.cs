using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Enum;
using Managers.BattleMgrComponents.BattlePlayer;
using Managers.BattleMgrComponents.PokemonLogic;
using Managers.BattleMgrComponents.PokemonLogic.BuffResults;
using PokemonLogic;
using PokemonLogic.BuffResults;
using PokemonLogic.PokemonData;
using Unity.VisualScripting;
using UnityEngine;

namespace Managers.BattleMgrComponents.BattlePlayables.Skills
{
    public class RunTimeSkillBase : ABattlePlayable
    {
        public readonly Pokemon Source;
        public readonly CommonSkillTemplate Template;
        private readonly List<Func<CommonResult, Pokemon, Pokemon, CommonSkillTemplate, UniTask<CommonResult>>> _procedure;

        public CommonResult RuntimeParam;

        // two kinds of skill, target on pokemon, target on position, most of skills target on position, items target on pokemon
        public RunTimeSkillBase(CommonSkillTemplate template, Pokemon source, CommonResult preLoadResult, int priority = (int)PlayablePriority.None) : base(priority == (int)PlayablePriority.None ? BattleLogic.GetPokemonSpeed(source) : priority)
        {
            _procedure = new List<Func<CommonResult, Pokemon, Pokemon, CommonSkillTemplate, UniTask<CommonResult>>>();
            Template = template;
            Source = source;
            RuntimeParam = preLoadResult;
        }

#if UNITY_EDITOR
        public List<Pokemon> GetRunTimeTarget()
        {
            return RuntimeParam.TargetsByPokemons;
        }
#endif

        public override async void Execute()
        {
            var item = new BattleStackItem();
            item.Source = Source;
            item.Template = Template;
            BattleMgr.Instance.BattleStack.Add(item);

            if (RuntimeParam.RunTimeSkillBaseIsItem) 
                await BattleMgr.Instance.SetCommandText(BattleMgr.Instance.PlayerInGame[Source.TrainerID].PlayerInfo.name + " use " + Template.Name);
            else if (Source.OnStage && !Source.IsFaint) 
                await BattleMgr.Instance.SetCommandText(BattleMgr.Instance.PlayerInGame[Source.TrainerID].PlayerInfo.name + " asks " + Source.GetName() + " to use " + Template.Name);
            
            if (Available == false)
            {
                if (Source.OnStage && !Source.IsFaint)
                {
                    await BattleMgr.Instance.SetCommandText("But failed!");
                }
                OnDestroy();
                return;
            }
            if (Template.ProcedureFunctions == null)
                return;
            foreach (var func in Template.ProcedureFunctions)
            {
                _procedure.Add(func);
            }
            await ExecuteList();
            
            OnDestroy();
        }

        protected override void OnDestroy()
        {
            BattleMgr.Instance.BattlePlayableEnd();
        }

        private async UniTask ExecuteList()
        {
            if (RuntimeParam.TargetsByPokemons == null)
            {
                RuntimeParam.TargetsByPokemons = await BattleLogic.FindTarget(RuntimeParam.TargetsByIndices, Source);
            }
            
            if (!RuntimeParam.RunTimeSkillBaseIsItem)
            {
                RuntimeParam.CanMove = true;
                RuntimeParam.Priority = (int)MovePriority.Normal;
                RuntimeParam.STemplate = Template;
                RuntimeParam = await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.BeforeMove, RuntimeParam, Source);
            
                if (!RuntimeParam.CanMove)
                {
                    if (RuntimeParam.Message != null)
                    {
                        await BattleMgr.Instance.SetCommandText(RuntimeParam.Message);
                    }
                    return;
                }
            }
            

            if (RuntimeParam.TargetsByPokemons == null || RuntimeParam.TargetsByPokemons.Count == 0)
            {
                await BattleMgr.Instance.SetCommandText("But there is no target!");
                return;
            }

            BattleMgr.Instance.BattleStack[^1].Targets = RuntimeParam.TargetsByPokemons;

            foreach (var target in RuntimeParam.TargetsByPokemons)
            {
                for (int i = 0; i < _procedure.Count; i++)
                {
                    RuntimeParam = await _procedure[i](RuntimeParam, Source, target, Template);
                    if (RuntimeParam == null)
                    {
                        break;
                    }
                }
            }
        }
    }
}