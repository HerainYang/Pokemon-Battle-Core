using CoreScripts.BattlePlayables;
using TalesOfRadiance.Scripts.Battle.BattleComponents;
using TalesOfRadiance.Scripts.Battle.Managers;
using UnityEngine;
using Types = TalesOfRadiance.Scripts.Battle.Constant.Types;

namespace TalesOfRadiance.Scripts.Battle.BattlePlayables
{
    public class BpMove : ABattlePlayable
    {
        private ABattleEntity _entity;
        public BpMove(ABattleEntity entity) : base((int)Types.PlayablePriority.Skill)
        {
            _entity = entity;
        }

        public override async void Execute()
        {
            var template = _entity.MakeBattleDecision();
            var playable = await template.SendLoadSkillRequest(_entity);
            BattleMgr.Instance.TransferControlToPendingPlayable(playable);
        }

        protected override void OnDestroy()
        {
            Debug.LogError("It should reach here");
        }
    }
}