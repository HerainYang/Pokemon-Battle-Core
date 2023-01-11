using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Enum;
using Managers.BattleMgrComponents.BattlePlayer;
using Managers.BattleMgrComponents.PokemonLogic;
using UnityEngine;

namespace Managers.BattleMgrComponents.BattlePlayables.Stages
{
    public class BpForceAddPokemon : ABattlePlayable
    {
        private Dictionary<ABattlePlayer, int> _needChangePokemonList; // int here is the position
        public BpForceAddPokemon() : base((int)PlayablePriority.ForceAddPokemon)
        {
        }

        public override void Execute()
        {
            _needChangePokemonList = new Dictionary<ABattlePlayer, int>();
            EventMgr.Instance.AddListener<ABattlePlayer>(Constant.EventKey.BattlePokemonForceChangeCommandSent, ReceivePokemonChangeCommand);
            for (int i = 0; i < BattleMgr.Instance.OnStagePokemon.Length; i++)
            {
                if (BattleMgr.Instance.OnStagePokemon[i] == null)
                {
                    var player = BattleMgr.Instance.PokemonStageIndex2PlayerMapping[i];
                    if(player.PokemonCanSent() == 0)
                        continue;
                    if (_needChangePokemonList.ContainsKey(player) && _needChangePokemonList[player] >= player.PokemonCanSent()) // no more pokemon can be sent into battlefield
                    {
                        continue;
                    }
                    if(!_needChangePokemonList.ContainsKey(player))
                    {
                        _needChangePokemonList.Add(player, 0);
                    }

                    _needChangePokemonList[BattleMgr.Instance.PokemonStageIndex2PlayerMapping[i]]++;
                    player.AddPosition2ForceChangeList(i);
                }
            }

            if (_needChangePokemonList.Count == 0)
            {
                OnDestroy();
            }
            else
            {
                foreach (var pair in BattleMgr.Instance.PlayerInGame)
                {
                    var player = pair.Value;
                    
                    if (_needChangePokemonList.ContainsKey(player))
                    {
                        Debug.Log(player.PlayerInfo.name + " " + _needChangePokemonList[player]);
                        player.ExecuteAddPokemonStage();
                    }
                }
            }
        }

        protected override void OnDestroy()
        {
            _needChangePokemonList = null;
            EventMgr.Instance.RemoveListener<ABattlePlayer>(Constant.EventKey.BattlePokemonForceChangeCommandSent, ReceivePokemonChangeCommand);
            BattleMgr.Instance.BattlePlayableEnd();
        }
        
        private void ReceivePokemonChangeCommand(ABattlePlayer player)
        {
            Debug.Log("[BPForceAddPokemon] Receive pokemon change command from " + player.PlayerInfo.name);

            _needChangePokemonList[player]--;
            Debug.Log(_needChangePokemonList[player]);
            if (_needChangePokemonList[player] == 0)
            {
                _needChangePokemonList.Remove(player);
                if (_needChangePokemonList.Count == 0)
                {
                    OnDestroy();
                }
                return;
            }
            
            player.ExecuteAddPokemonStage();
        }
    }
}