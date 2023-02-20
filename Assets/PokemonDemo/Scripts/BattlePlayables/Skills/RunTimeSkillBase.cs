using System;
using System.Collections.Generic;
using CoreScripts.BattleComponents;
using Cysharp.Threading.Tasks;
using Managers.BattleMgrComponents;
using Managers.BattleMgrComponents.BattlePlayables;
using PokemonDemo.Scripts.BattleMgrComponents;
using PokemonDemo.Scripts.Enum;
using PokemonDemo.Scripts.Managers;
using PokemonDemo.Scripts.PokemonLogic.BuffResults;
using PokemonDemo.Scripts.PokemonLogic.PokemonData;

namespace PokemonDemo.Scripts.BattlePlayables.Skills
{
    public class RunTimeSkillBase : APokemonBattlePlayable
    {
        public new readonly Pokemon PokemonSource;
        public readonly CommonSkillTemplate Template;
        private readonly List<Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>>> _procedure;

        public CommonResult RuntimeParam;

        // two kinds of skill, target on pokemon, target on position, most of skills target on position, items target on pokemon
        public RunTimeSkillBase(CommonSkillTemplate template, Pokemon pokemonSource, CommonResult preLoadResult, int priority = (int)PlayablePriority.None) : base(priority == (int)PlayablePriority.None ? BattleLogic.GetPokemonSpeed(pokemonSource) : priority)
        {
            _procedure = new List<Func<ASkillResult, IBattleEntity, IBattleEntity, ASkillTemplate, UniTask<ASkillResult>>>();
            Template = template;
            PokemonSource = pokemonSource;
            Source = PokemonSource;
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
            item.Source = PokemonSource;
            item.Template = Template;
            BattleMgr.Instance.BattleStack.Add(item);

            if (RuntimeParam.RunTimeSkillBaseIsItem) 
                await BattleMgr.Instance.SetCommandText(BattleMgr.Instance.PlayerInGame[PokemonSource.TrainerID].PlayerInfo.name + " use " + Template.Name);
            else if (PokemonSource.OnStage && !PokemonSource.IsFaint) 
                await BattleMgr.Instance.SetCommandText(BattleMgr.Instance.PlayerInGame[PokemonSource.TrainerID].PlayerInfo.name + " asks " + PokemonSource.GetName() + " to use " + Template.Name);
            
            if (Available == false)
            {
                if (PokemonSource.OnStage && !PokemonSource.IsFaint)
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
                RuntimeParam.TargetsByPokemons = await BattleLogic.FindTarget(RuntimeParam.TargetsByIndices, PokemonSource);
            }
            
            if (!RuntimeParam.RunTimeSkillBaseIsItem)
            {
                RuntimeParam.CanMove = true;
                RuntimeParam.Priority = (int)MovePriority.Normal;
                RuntimeParam.STemplate = Template;
                RuntimeParam = (CommonResult)await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.BeforeMove, RuntimeParam, PokemonSource);
            
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
                    RuntimeParam = (CommonResult)await _procedure[i](RuntimeParam, PokemonSource, target, Template);
                    if (RuntimeParam == null)
                    {
                        break;
                    }
                }
            }
        }
    }
}