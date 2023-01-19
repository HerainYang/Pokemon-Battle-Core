using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Enum;
using Managers.BattleMgrComponents.BattlePlayables.Skills;
using Managers.BattleMgrComponents.BattlePlayables.Stages;
using PokemonLogic;
using PokemonLogic.BuffResults;
using PokemonLogic.PokemonData;
using UI.BattleUIComponent;
using UnityEngine;

namespace Managers.BattleMgrComponents.BattlePlayer
{
    public class PlayerUser : ABattlePlayer
    {
        protected override void SendCommandRequest(Pokemon pokemon)
        {
            BattleMgr.Instance.BattleScenePanelTwoPlayerUI.StartCommandStage(this, pokemon);
        }

        protected override void SendPokemonForceAddRequest(int onStagePosition)
        {
            BattleMgr.Instance.BattleScenePanelTwoPlayerUI.StartForcePokemonSelect(this, onStagePosition);
        }

        public override void TestHeartBeat()
        {
            EventMgr.Instance.Dispatch(Constant.EventKey.HeartBeatSent, this);
        }

        public PlayerUser(BasicPlayerInfo info) : base(info)
        {
        }
        
        public override async void SelectOnePokemonToSend(int onStagePosition, bool forceChange)
        {
            _ = UIWindowsManager.Instance.ShowUIWindowAsync("PokemonSelectPanel").ContinueWith((o =>
            {
                o.GetComponent<PokemonSelectPanel>().Init(onStagePosition, forceChange, CommandRequestType.Pokemons, Pokemons);
            }));
            var handler = EventMgr.Instance.ListenTo<int[]>(Constant.UIEventKey.ClosePokemonSelectWindow);
            await UniTask.WaitUntil(() => handler.Complete);
            if (handler.Result == null)
            {
                return;
            }

            Pokemon pokemonToSent = Pokemons[handler.Result[0]];
            EventMgr.Instance.Dispatch(Constant.EventKey.RequestSentPokemonOnStage, pokemonToSent, onStagePosition);
        }

        public override async UniTask<int[]> SelectIndicesTarget(CommonSkillTemplate template, Pokemon curPokemon)
        {
            if (!(BattleMgr.Instance.GetCurrentPlayable() is BpCommandStage))
            {
                return await base.SelectIndicesTarget(template, curPokemon);
            }

            int onStagePosition = BattleMgr.Instance.GetPokemonOnstagePosition(curPokemon);
            _ = UIWindowsManager.Instance.ShowUIWindowAsync("PokemonSelectPanel").ContinueWith((o =>
            {
                if (template.IsItem)
                {
                    o.GetComponent<PokemonSelectPanel>().Init(onStagePosition, false, CommandRequestType.Items, BattleMgr.Instance.PlayerInGame[curPokemon.TrainerID].Pokemons);
                }
                else
                {
                    List<Pokemon> onstageNotNullPokemon = BattleMgr.Instance.GetOnStageAlivePokemon();
                    o.GetComponent<PokemonSelectPanel>().Init(onStagePosition, false, CommandRequestType.Skills, onstageNotNullPokemon, template);
                }
            }));
            var handler = EventMgr.Instance.ListenTo<int[]>(Constant.UIEventKey.ClosePokemonSelectWindow);
            await UniTask.WaitUntil(() => handler.Complete);
            return handler.Result;
        }

        public override async UniTask<List<PokemonRuntimeSkillData>> SelectSkillFromTarget(int skillInNeed, Pokemon curPokemon)
        {
            if (!(BattleMgr.Instance.GetCurrentPlayable() is BpCommandStage) || skillInNeed != 1) // only test select 1, don't want to write more code to support select 2 or 3, they will be basically the same as the implement of pokemon select panel
            {
                return await base.SelectSkillFromTarget(skillInNeed, curPokemon);
            }

            _ = UIWindowsManager.Instance.ShowUIWindowAsync("ItemSelectPanel").ContinueWith((o => { o.GetComponent<ItemSelectPanel>().ShowSkillList(curPokemon); }));

            var handler = EventMgr.Instance.ListenTo<List<PokemonRuntimeSkillData>>(Constant.UIEventKey.CloseSkillSelectWindow);
            await UniTask.WaitUntil(() => handler.Complete);

            return handler.Result;
        }
    }
}