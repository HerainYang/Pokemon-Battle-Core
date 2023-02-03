// Each battle run represent one round in the battle, including your turn, your opposite's turn

using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreScripts.Managers;
using Enum;
using Managers.BattleMgrComponents;
using Managers.BattleMgrComponents.BattlePlayables;
using Managers.BattleMgrComponents.BattlePlayables.Skills;
using Managers.BattleMgrComponents.BattlePlayables.Stages;
using PokemonLogic.PokemonData;
using UnityEngine;

namespace CoreScripts.BattlePlayables
{
    public class BattleRound
    {
        private List<ABattlePlayable> _battlePlayables;
        private List<ABattlePlayable> _remainingPlayables;
        public BattleRoundStatus Status;
        private readonly int _roundCount;
        private readonly ABattleMgr _battleMgr;
        
#if UNITY_EDITOR
        public List<ABattlePlayable> GetPlayables()
        {
            return _battlePlayables;
        }
        
        public List<ABattlePlayable> GetRemainingPlayables()
        {
            return _remainingPlayables;
        }
#endif

        public BattleRound(int roundCount, ABattleMgr battleMgr)
        {
            _battlePlayables = new List<ABattlePlayable>();
            _remainingPlayables = new List<ABattlePlayable>();
            Status = BattleRoundStatus.Prepare;
            _battleMgr = battleMgr;
            _battleMgr.UpdateRoundCount();
            _roundCount = roundCount;
        }

        public void OnDestroy()
        {
            _battlePlayables = null;
            _remainingPlayables = null;
            Status = BattleRoundStatus.Dead;
            Debug.Log("[BattleRound] OnDestroy: Round " + _roundCount);
        }

        public void AddBattlePlayables(ABattlePlayable playable)
        {
            _remainingPlayables.Add(playable);
        }

        public int CancelSkillByPSourcePokemonAndSkillId(Pokemon pokemon, int skillId)
        {
            int i = _remainingPlayables.RemoveAll(playable => playable is RunTimeSkillBase && ((RunTimeSkillBase)playable).Template.ID == skillId && Equals(((RunTimeSkillBase)playable).Source, pokemon));
            Debug.Log(GetProcessChart());
            return i;
        }

        public bool IsLastSkill()
        {
            foreach (var playable in _remainingPlayables)
            {
                if (playable is RunTimeSkillBase)
                    return false;
            }

            return true;
        }

        // to continue running the battle round, change the status and call this function
        // to run stage one by one, just call this function when you need to process
        public void ExecuteBattleStage()
        {
            if (_remainingPlayables.Count == 0)
            {
                _battleMgr.EndOfCurRound();
                return;
            }

            ABattlePlayable next = _remainingPlayables[0];
            foreach (var playable in _remainingPlayables)
            {
                if (playable.Compare(next))
                {
                    next = playable;
                }
            }

            _remainingPlayables.Remove(next);
            _battlePlayables.Add(next);

            Debug.Log("[BattleRound] Executing " + _battlePlayables.Last());
            Debug.Log(GetProcessChart());

            _battlePlayables.Last().Execute();
        }

        public ABattlePlayable GetCurrentPlayable()
        {
            return _battlePlayables.Last();
        }
        
        
        private string GetProcessChart()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(_roundCount + " [BattleRound] playing: " + _battlePlayables.Count + "\n");
            for (int i = 0; i < _battlePlayables.Count; i++)
            {
                stringBuilder.Append(_battlePlayables[i] + "\n");
            }

            stringBuilder.Append("*****************************************\n");

            foreach (var playable in _remainingPlayables)
            {
                stringBuilder.Append(playable + "\n");
            }

            return stringBuilder.ToString();
        }

        // this will be call when a pokemon dead
        public void RemoveRunTimeSkillPlayables(Pokemon source)
        {
            if (_remainingPlayables.Count == 0)
            {
                return;
            }

            for (int i = _remainingPlayables.Count - 1; i >= 0; i--)
            {
                if (_remainingPlayables[i] is RunTimeSkillBase)
                {
                    if (((RunTimeSkillBase)_remainingPlayables[i]).Source.Equals(source))
                    {
                        _remainingPlayables.RemoveAt(i);
                    }
                }
            }
        }

        public void TransferControlToPendingPlayable(ABattlePlayable targetPlayable)
        {
            if (_remainingPlayables.Contains(targetPlayable))
            {
                _remainingPlayables.Remove(targetPlayable);
            }

            _battlePlayables.Add(targetPlayable);
            _battlePlayables.Last().Execute();
        }

        public void UpdateSkillPriority()
        {
            foreach (var playable in _remainingPlayables)
            {
                if (playable is RunTimeSkillBase && playable.Priority < (int)PlayablePriority.SkillImm)
                {
                    playable.Priority = ((RunTimeSkillBase)playable).Source.Speed;
                }
            }
        }
    }
}