using System.Collections.Generic;
using CoreScripts.BattlePlayables;
using Cysharp.Threading.Tasks;
using Managers;
using TalesOfRadiance.Scripts.Battle.Constant;
using TalesOfRadiance.Scripts.Battle.Managers;
using TalesOfRadiance.Scripts.Character;

namespace TalesOfRadiance.Scripts.Battle.BattlePlayables
{
    public class BpCommand : ABattlePlayable
    {
        private HashSet<CharacterTeam> _teamCount;

        public BpCommand() : base((int)Types.PlayablePriority.CommandStage)
        {
        }

        public override void Execute()
        {
            _teamCount = new HashSet<CharacterTeam>();
            foreach (var team in BattleMgr.Instance.OnStageTeam)
            {
                team.LoadBattlePlayables();
            }
            OnDestroy();
        }

        protected override void OnDestroy()
        {
            BattleMgr.Instance.BattlePlayableEnd();
        }
    }
}