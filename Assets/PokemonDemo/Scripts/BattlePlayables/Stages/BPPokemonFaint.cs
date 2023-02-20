using System;
using Cysharp.Threading.Tasks;
using Managers.BattleMgrComponents;
using Managers.BattleMgrComponents.BattlePlayables;
using PokemonDemo.Scripts.BattleMgrComponents;
using PokemonDemo.Scripts.BattlePlayer;
using PokemonDemo.Scripts.Enum;
using PokemonDemo.Scripts.PokemonLogic.PokemonData;
using UnityEngine;

namespace PokemonDemo.Scripts.BattlePlayables.Stages
{
    public class BpPokemonFaint : APokemonBattlePlayable
    {
        private APokemonBattlePlayer _trainer;
        private Pokemon _curPokemon;
        public BpPokemonFaint(Pokemon pokemon) : base((int)PlayablePriority.Immediately)
        {
            _trainer = BattleMgr.Instance.PlayerInGame[pokemon.TrainerID];
            _curPokemon = pokemon;
        }

        public override async void Execute()
        {
            int pokemonStageIndex = Array.FindIndex(BattleMgr.Instance.OnStagePokemon, pokemon1 => (pokemon1 != null && _curPokemon.RuntimeID == pokemon1.RuntimeID));
            await BattleMgr.Instance.WithDrawPokemon(pokemonStageIndex);
            
            int pokemonInBagIndex = BattleMgr.Instance.PlayerInGame[_curPokemon.TrainerID].Pokemons.FindIndex(pokemon1 => pokemon1.RuntimeID == _curPokemon.RuntimeID);
            BattleMgr.Instance.BattleScenePanelTwoPlayerUI.SetPokeBallStatus(BattleMgr.Instance.PlayerInGame[_curPokemon.TrainerID], _curPokemon, pokemonInBagIndex);

            await BattleMgr.Instance.SetCommandText(_curPokemon.Name + " faint");
            await UniTask.Delay(BattleMgr.Instance.AwaitTime);
            if (_trainer.AllPokemonFaint())
            {
                Debug.LogError("Battle End");
            }
            OnDestroy();
        }

        protected override void OnDestroy()
        {
            BattleMgr.Instance.BattlePlayableEnd();
        }
    }
}