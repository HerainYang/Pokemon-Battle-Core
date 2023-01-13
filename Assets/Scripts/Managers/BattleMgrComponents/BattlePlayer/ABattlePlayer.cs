using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Managers.BattleMgrComponents.PokemonLogic;
using UnityEngine;

namespace Managers.BattleMgrComponents.BattlePlayer
{
    public abstract class ABattlePlayer
    {
        protected List<Pokemon> CommandRequests;
        protected List<int> ForceChangePokemonRequest;
        public BasicPlayerInfo PlayerInfo;
        public List<Pokemon> Pokemons;

        public ABattlePlayer(BasicPlayerInfo info)
        {
            CommandRequests = new List<Pokemon>();
            ForceChangePokemonRequest = new List<int>();
            Pokemons = new List<Pokemon>();
            PlayerInfo = info;
        }

        protected abstract void SendCommandRequest(Pokemon pokemon);
        protected abstract void SendPokemonForceAddRequest(int onStagePosition);
        public abstract void TestHeartBeat();

        public void ExecuteCommandStage()
        {
            if (CommandRequests.Count == 0)
            {
                return;
            }
            
            Pokemon pokemon = CommandRequests.First();
            CommandRequests.RemoveAt(0);
            SendCommandRequest(pokemon);
        }

        public void ExecuteAddPokemonStage()
        {
            if (ForceChangePokemonRequest.Count == 0)
            {
                return;
            }

            int onStagePosition = ForceChangePokemonRequest.First();
            ForceChangePokemonRequest.RemoveAt(0);
            SendPokemonForceAddRequest(onStagePosition);
        }

        public void AddCommandForPokemon(Pokemon pokemon)
        {
            CommandRequests.Add(pokemon);
        }

        public void AddPosition2ForceChangeList(int position)
        {
            ForceChangePokemonRequest.Add(position);
        }

        public bool AllPokemonFaint()
        {
            foreach (var pokemon in Pokemons)
            {
                if (!pokemon.IsFaint)
                {
                    return false;
                }
            }

            return true;
        }

        public int PokemonCanSent()
        {
            int count = 0;
            foreach (var pokemon in Pokemons)
            {
                if (!pokemon.IsFaint && !pokemon.OnStage)
                {
                    count++;
                }
            }

            return count;
        }

        public int GetFirstPokemonCanSentIndex()
        {
            for (int i = 0; i < Pokemons.Count; i++)
            {
                if (!Pokemons[i].IsFaint && !Pokemons[i].OnStage)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}