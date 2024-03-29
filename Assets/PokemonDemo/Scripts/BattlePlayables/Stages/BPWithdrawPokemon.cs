using Managers.BattleMgrComponents;
using Managers.BattleMgrComponents.BattlePlayables;
using PokemonDemo.Scripts.BattleMgrComponents;
using PokemonDemo.Scripts.Enum;

namespace PokemonDemo.Scripts.BattlePlayables.Stages
{
    public class BpWithdrawPokemon : APokemonBattlePlayable
    {
        private readonly int _onStagePosition;
        public BpWithdrawPokemon(int onStagePosition) : base((int)PlayablePriority.WithDrawPokemon)
        {
            _onStagePosition = onStagePosition;
        }

        public override async void Execute()
        {
            await BattleMgr.Instance.SetCommandText("Come Back! " + BattleMgr.Instance.OnStagePokemon[_onStagePosition].Name);
            await BattleMgr.Instance.WithDrawPokemon(_onStagePosition);
            OnDestroy();
        }

        protected override void OnDestroy()
        {
            BattleMgr.Instance.BattlePlayableEnd();
        }
    }
}