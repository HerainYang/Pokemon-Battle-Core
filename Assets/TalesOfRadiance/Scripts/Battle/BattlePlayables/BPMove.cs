using CoreScripts.BattleComponents;
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
        public BpMove(AtorBattleEntity entity) : base((int)Types.PlayablePriority.Skill)
        {
            Source = entity;
            if (Source is RuntimeHero hero)
            {
                Priority = hero.Template.Speed;
            }
        }
        
#if UNITY_EDITOR
        public IBattleEntity GetSource()
        {
            return Source;
        }
#endif

        public override async void Execute()
        {
            AtorBattleEntity source = ((AtorBattleEntity)Source);
            var template = source.MakeBattleDecision();
            var playable = await template.SendLoadSkillRequest(source);
            BattleMgr.Instance.TransferControlToPendingPlayable(playable);
        }

        protected override void OnDestroy()
        {
            Debug.LogError("It should reach here");
        }
    }
}