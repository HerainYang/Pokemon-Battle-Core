using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using Enum;
using Managers.BattleMgrComponents.BattlePlayer;
using Managers.BattleMgrComponents.PokemonLogic;
using Managers.BattleMgrComponents.PokemonLogic.BuffResults;
using PokemonDemo;
using PokemonLogic;
using PokemonLogic.BuffResults;
using PokemonLogic.PokemonData;
using UnityEngine;

namespace Managers.BattleMgrComponents.BattlePlayables.Stages
{
    public class BpCommandStage : ABattlePlayable
    {
        private HashSet<Tuple<ABattlePlayer, Guid>> _commandWaitList;
        private HashSet<Tuple<ABattlePlayer, Guid>> _receiveList;
        public BpCommandStage() : base((int)PlayablePriority.CommandStage)
        {
            
        }
        public override async void Execute()
        {
            _commandWaitList = new HashSet<Tuple<ABattlePlayer, Guid>>();
            _receiveList = new HashSet<Tuple<ABattlePlayer, Guid>>();
            EventMgr.Instance.AddListener<ABattlePlayer, Pokemon>(Constant.EventKey.BattleCommandSent, ReceiveCommand);
            foreach (var pokemon in BattleMgr.Instance.OnStagePokemon)
            {
                if (pokemon == null)
                {
                    continue;
                }

                
                ABattlePlayer player = BattleMgr.Instance.PlayerInGame[pokemon.TrainerID];
                player.AddCommandForPokemon(pokemon);
                _commandWaitList.Add(new Tuple<ABattlePlayer, Guid>(player, pokemon.RuntimeID));
            }
            
            await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.BeforeRequirePokemonCommand, new CommonResult());

            foreach (var player in BattleMgr.Instance.PlayerInGame)
            {
                player.Value.ExecuteCommandStage();
            }
        }

        private void ReceiveCommand(ABattlePlayer player, Pokemon pokemon)
        {
            // _commandWaitList.Remove(new Tuple<ABattlePlayer, Guid>(player, pokemon.RuntimeID));
            bool success = _receiveList.Add(new Tuple<ABattlePlayer, Guid>(player, pokemon.RuntimeID));
            Debug.Log("[BPCommandStage] Receive Command From " + player.PlayerInfo.name + " of pokemon " + pokemon.RuntimeID + ", remains " + (_commandWaitList.Count - _receiveList.Count) + " to wait!");
            if(!success)
                return;
            if (_commandWaitList.Count == _receiveList.Count)
            {
                OnDestroy();
                return;
            }
            
            player.ExecuteCommandStage();
        }

        protected override void OnDestroy()
        {
            _commandWaitList = null;
            _receiveList = null;
            EventMgr.Instance.RemoveListener<ABattlePlayer, Pokemon>(Constant.EventKey.BattleCommandSent, ReceiveCommand);
            BattleMgr.Instance.BattlePlayableEnd();
        }
    }
}