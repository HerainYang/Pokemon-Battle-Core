using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Managers;
using Managers.BattleMgrComponents;
using PokemonDemo.Scripts.BattleMgrComponents;
using PokemonDemo.Scripts.BattlePlayables.Skills;
using PokemonDemo.Scripts.Enum;
using PokemonDemo.Scripts.Managers;
using PokemonDemo.Scripts.PokemonLogic;
using PokemonDemo.Scripts.PokemonLogic.PokemonData;

namespace PokemonDemo.Scripts.BattlePlayer
{
    public abstract class APokemonBattlePlayer
    {
        private readonly List<Pokemon> _commandRequests;
        private readonly List<int> _forceChangePokemonRequest;
        public readonly BasicPlayerInfo PlayerInfo;
        public readonly List<Pokemon> Pokemons;
        public Dictionary<Item, int> Items;

        public APokemonBattlePlayer(BasicPlayerInfo info)
        {
            _commandRequests = new List<Pokemon>();
            _forceChangePokemonRequest = new List<int>();
            Pokemons = new List<Pokemon>();
            Items = new Dictionary<Item, int>();
            PlayerInfo = info;
        }

        protected abstract void SendCommandRequest(Pokemon pokemon);
        protected abstract void SendPokemonForceAddRequest(int onStagePosition);
        public abstract void TestHeartBeat();

        public void ExecuteCommandStage()
        {
            
            if (_commandRequests.Count == 0)
            {
                return;
            }
            
            Pokemon pokemon = _commandRequests.First();
            _commandRequests.RemoveAt(0);
            SendCommandRequest(pokemon);
        }

        public void RemovePokemonFromRequestList(Pokemon pokemon)
        {
            _commandRequests.Remove(pokemon);
        }

        public void ExecuteAddPokemonStage()
        {
            if (_forceChangePokemonRequest.Count == 0)
            {
                return;
            }

            int onStagePosition = _forceChangePokemonRequest.First();
            _forceChangePokemonRequest.RemoveAt(0);
            SendPokemonForceAddRequest(onStagePosition);
        }

        public void AddCommandForPokemon(Pokemon pokemon)
        {
            _commandRequests.Add(pokemon);
        }

        public void AddPosition2ForceChangeList(int position)
        {
            _forceChangePokemonRequest.Add(position);
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

        public Pokemon GetFirstPokemonCanSent()
        {
            for (int i = 0; i < Pokemons.Count; i++)
            {
                if (!Pokemons[i].IsFaint && !Pokemons[i].OnStage)
                {
                    return Pokemons[i];
                }
            }
            
            return null;
        }

        public void UserItem(int itemID)
        {
            var listOfItem = new List<Item>(Items.Keys);
            foreach (var item in listOfItem.Where(item => item.ID == itemID))
            {
                if (Items[item] <= 0)
                {
                    throw new Exception("It should be impossible for item count to be less or equal to 0");
                }

                Items[item]--;
                return;
            }
            throw new Exception("It should be impossible to call this function while user does not have this item");
        }

        public virtual void SelectOnePokemonToSend(int onStagePosition, bool forceChange)
        {
            EventMgr.Instance.Dispatch(Constant.EventKey.RequestSentPokemonOnStage, GetFirstPokemonCanSent(), onStagePosition);
        }

        public virtual async UniTask<int[]> SelectIndicesTarget(CommonSkillTemplate template, Pokemon curPokemon)
        {
            int[] targets = null;
            switch (template.TargetType)
            {
                case SkillTargetType.OneEnemy:
                    targets = BattleMgr.Instance.TryAutoGetTarget(curPokemon, SkillTargetType.FirstAvailableEnemy);
                    break;
                default:
                    throw new NotImplementedException();
            }

            await UniTask.Yield();

            return targets;
        }
        
        public virtual async UniTask<List<PokemonRuntimeSkillData>> SelectSkillFromTarget(int skillInNeed, Pokemon curPokemon)
        {
            List<PokemonRuntimeSkillData> resultList = new List<PokemonRuntimeSkillData>();
            for (int i = 0; i < skillInNeed; i++)
            {
                if(i >= curPokemon.RuntimeSkillList.Count)
                    break;
                resultList.Add(curPokemon.RuntimeSkillList[i]);
            }

            await UniTask.Yield();

            return resultList;
        }
    }
}