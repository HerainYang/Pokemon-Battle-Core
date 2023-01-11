using Cysharp.Threading.Tasks;
using Enum;
using Managers.BattleMgrComponents.PokemonLogic;
using Managers.BattleMgrComponents.PokemonLogic.BuffResults;
using UnityEngine;

namespace Managers.BattleMgrComponents.BattlePlayables.Stages
{
    public class BpDebut : ABattlePlayable
    {
        private BasicPlayerInfo _info;
        private int _pokemonIndex;
        private int _onStagePosition;

#if UNITY_EDITOR
        public Pokemon GetPokemonInstance()
        {
            return BattleMgr.Instance.PlayerInGame[_info.playerID].Pokemons[_pokemonIndex];
        }
#endif

        public BpDebut(BasicPlayerInfo info, int pokemonIndex, int onStagePosition) : base((int)PlayablePriority.Debut)
        {
            _info = info;
            _pokemonIndex = pokemonIndex;
            _onStagePosition = onStagePosition;
        }
        
        public override async void Execute()
        {
            Pokemon pokemonBasicInfoInstance = BattleMgr.Instance.PlayerInGame[_info.playerID].Pokemons[_pokemonIndex];
            pokemonBasicInfoInstance.OnStage = true;
            BattleMgr.Instance.SetCommandText(_info.name + " send " + pokemonBasicInfoInstance.GetName());
            if (_info.isAI)
            {
                BattleMgr.Instance.BattleScenePanelTwoPlayerUI.opPokemonInfo.SetPokemonInfo(pokemonBasicInfoInstance);
            }
            else
            {
                BattleMgr.Instance.BattleScenePanelTwoPlayerUI.selfPokemonInfo.SetPokemonInfo(pokemonBasicInfoInstance);
            }
            await pokemonBasicInfoInstance.Attribute.InitAttribute(pokemonBasicInfoInstance);
            await UniTask.Delay(BattleMgr.Instance.AwaitTime);
            BattleMgr.Instance.OnStagePokemon[_onStagePosition] = pokemonBasicInfoInstance;
            CommonResult result = new CommonResult();
            result.DebutPokemon = pokemonBasicInfoInstance;
            result.TargetWeather = BattleMgr.Instance.GetWeather();
            await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.AfterDebut, result, pokemonBasicInfoInstance);
            OnDestroy();
        }

        protected override void OnDestroy()
        {
            BattleMgr.Instance.BattlePlayableEnd();
        }
    }
}