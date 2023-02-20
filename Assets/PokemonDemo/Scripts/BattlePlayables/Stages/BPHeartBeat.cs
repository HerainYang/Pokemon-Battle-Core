using System.Collections.Generic;
using Managers;
using Managers.BattleMgrComponents;
using Managers.BattleMgrComponents.BattlePlayables;
using PokemonDemo.Scripts.BattleMgrComponents;
using PokemonDemo.Scripts.BattlePlayer;
using PokemonDemo.Scripts.Enum;
using PokemonDemo.Scripts.Managers;
using UnityEngine;

namespace PokemonDemo.Scripts.BattlePlayables.Stages
{
    public class BpHeartBeat : APokemonBattlePlayable
    {
        private HashSet<string> _waitingList;
        public BpHeartBeat() : base((int)PlayablePriority.HeartBeat)
        {
            
        }

        public override void Execute()
        {
            EventMgr.Instance.AddListener<APokemonBattlePlayer>(Constant.EventKey.HeartBeatSent, ReceiveHeartBeat);
            _waitingList = new HashSet<string>();
            foreach (var pair in BattleMgr.Instance.PlayerInGame)
            {
                var player = pair.Value;
                _waitingList.Add(player.PlayerInfo.playerID);
            }

            foreach (var player in BattleMgr.Instance.PlayerInGame)
            {
                player.Value.TestHeartBeat();
            }
        }

        protected override void OnDestroy()
        {
            _waitingList = null;
            EventMgr.Instance.RemoveListener<APokemonBattlePlayer>(Constant.EventKey.HeartBeatSent, ReceiveHeartBeat);
            BattleMgr.Instance.BattlePlayableEnd();
        }

        private void ReceiveHeartBeat(APokemonBattlePlayer player)
        {
            Debug.Log("[BPHeartBeat] Receive heartbeat from " + player.PlayerInfo.name);
            _waitingList.Remove(player.PlayerInfo.playerID);
            if (_waitingList.Count == 0)
            {
                OnDestroy();
            }
        }
    }
}