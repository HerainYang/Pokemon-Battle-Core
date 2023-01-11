using System.Collections.Generic;
using Enum;
using Managers.BattleMgrComponents.BattlePlayer;
using UnityEngine;

namespace Managers.BattleMgrComponents.BattlePlayables.Stages
{
    public class BpHeartBeat : ABattlePlayable
    {
        private HashSet<string> _waitingList;
        public BpHeartBeat() : base((int)PlayablePriority.HeartBeat)
        {
            
        }

        public override void Execute()
        {
            EventMgr.Instance.AddListener<ABattlePlayer>(Constant.EventKey.HeartBeatSent, ReceiveHeartBeat);
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
            EventMgr.Instance.RemoveListener<ABattlePlayer>(Constant.EventKey.HeartBeatSent, ReceiveHeartBeat);
            BattleMgr.Instance.BattlePlayableEnd();
        }

        private void ReceiveHeartBeat(ABattlePlayer player)
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