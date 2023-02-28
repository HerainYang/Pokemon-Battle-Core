using CoreScripts.BattlePlayables;
using TalesOfRadiance.Scripts.Battle.BattleComponents;
using TalesOfRadiance.Scripts.Battle.Constant;
using TalesOfRadiance.Scripts.Battle.Managers;

namespace TalesOfRadiance.Scripts.Battle.BattlePlayables
{
    public class BpEndOfRound : ABattlePlayable
    {
        public BpEndOfRound() : base((int)Types.PlayablePriority.EndOfRound)
        {
        }

        public override async void Execute()
        {
            await BuffMgr.Instance.ExecuteBuff(Constant.Constant.BuffEventKey.AfterRound, new SkillResult());
            OnDestroy();
        }

        protected override void OnDestroy()
        {
            BattleMgr.Instance.BattlePlayableEnd();
        }
    }
}