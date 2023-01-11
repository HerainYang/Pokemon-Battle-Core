using Enum;
using Managers.BattleMgrComponents.PokemonLogic;
using Managers.BattleMgrComponents.PokemonLogic.BuffResults;

namespace Managers.BattleMgrComponents.BattlePlayables.Stages
{
    public class BpEndOfRound : ABattlePlayable
    {
        public BpEndOfRound() : base((int)PlayablePriority.EndOfRound)
        {
        }

        public override async void Execute()
        {
            var result = new CommonResult();
            _ = await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.EndOfRound, result);
            OnDestroy();
        }

        protected override void OnDestroy()
        {
            BattleMgr.Instance.BattlePlayableEnd();
        }
    }
}