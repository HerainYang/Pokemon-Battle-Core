using CoreScripts.BattlePlayables;
using TalesOfRadiance.Scripts.Battle.BattleComponents;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;
using TalesOfRadiance.Scripts.Battle.Managers;
using UnityEngine;
using Types = TalesOfRadiance.Scripts.Battle.Constant.Types;

namespace TalesOfRadiance.Scripts.Battle.BattlePlayables
{
    public class BpMove : ABattlePlayable
    {
        public BpMove(ABattleEntity entity) : base((int)Types.PlayablePriority.Skill)
        {
            Source = entity;
            if (Source is RuntimeHero hero)
            {
                Priority = hero.Template.Speed;
            }
        }

        public override async void Execute()
        {
            var template = Source.MakeBattleDecision();
            var playable = await template.SendLoadSkillRequest(Source);
            BattleMgr.Instance.TransferControlToPendingPlayable(playable);
        }

        protected override void OnDestroy()
        {
            Debug.LogError("It should reach here");
        }
    }
}