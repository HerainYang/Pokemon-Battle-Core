using Managers.BattleMgrComponents;
using Managers.BattleMgrComponents.BattlePlayables;
using PokemonDemo.Scripts.BattleMgrComponents;
using PokemonDemo.Scripts.Enum;
using PokemonDemo.Scripts.Managers;
using PokemonDemo.Scripts.PokemonLogic.BuffResults;
using PokemonDemo.Scripts.PokemonLogic.PokemonData;

namespace PokemonDemo.Scripts.BattlePlayables.Stages
{
    public class BpDebut : APokemonBattlePlayable
    {
        private BasicPlayerInfo _info;
        private Pokemon _pokemonToBeOnStage;
        private int _onStagePosition;

#if UNITY_EDITOR
        public Pokemon GetPokemonInstance()
        {
            return _pokemonToBeOnStage;
        }
#endif

        public BpDebut(BasicPlayerInfo info, Pokemon pokemonToBeOnStage, int onStagePosition) : base((int)PlayablePriority.Debut)
        {
            _info = info;
            _pokemonToBeOnStage = pokemonToBeOnStage;
            _onStagePosition = onStagePosition;
        }
        
        public override async void Execute()
        {
            Pokemon pokemonBasicInfoInstance = _pokemonToBeOnStage;
            pokemonBasicInfoInstance.OnStage = true;
            BattleMgr.Instance.BattleScenePanelTwoPlayerUI.GetPokemonBattleInfo(_onStagePosition).SetPokemonInfo(pokemonBasicInfoInstance);

            await pokemonBasicInfoInstance.Attribute.InitAttribute(pokemonBasicInfoInstance);
            BattleMgr.Instance.OnStagePokemon[_onStagePosition] = pokemonBasicInfoInstance;
            PokemonCommonResult result = new PokemonCommonResult();
            result.DebutPokemon = pokemonBasicInfoInstance;
            result.TargetWeather = BattleMgr.Instance.GetWeather();
            await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.AfterDebut, result, pokemonBasicInfoInstance);
            await BattleMgr.Instance.SetCommandText(_info.name + " send " + pokemonBasicInfoInstance.GetName());
            OnDestroy();
        }

        protected override void OnDestroy()
        {
            BattleMgr.Instance.BattlePlayableEnd();
        }
    }
}