using Managers.BattleMgrComponents;
using Managers.BattleMgrComponents.BattlePlayables;
using PokemonDemo.Scripts.BattleMgrComponents;
using PokemonDemo.Scripts.Enum;
using PokemonDemo.Scripts.Managers;
using PokemonDemo.Scripts.PokemonLogic.BuffResults;

namespace PokemonDemo.Scripts.BattlePlayables.Stages
{
    public class BpEndOfRound : APokemonBattlePlayable
    {
        public BpEndOfRound() : base((int)PlayablePriority.EndOfRound)
        {
        }

        public override async void Execute()
        {
            var result = new PokemonCommonResult();
            _ = await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.EndOfRound, result);
            OnDestroy();
        }

        protected override void OnDestroy()
        {
            BattleMgr.Instance.BattlePlayableEnd();
        }
    }
}