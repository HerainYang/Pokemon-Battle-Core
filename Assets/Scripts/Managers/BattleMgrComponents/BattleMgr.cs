using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using Enum;
using Managers.BattleMgrComponents.BattlePlayables;
using Managers.BattleMgrComponents.BattlePlayables.Skills;
using Managers.BattleMgrComponents.BattlePlayables.Stages;
using Managers.BattleMgrComponents.BattlePlayer;
using Managers.BattleMgrComponents.PokemonLogic;
using Managers.BattleMgrComponents.PokemonLogic.BuffResults;
using PokemonDemo;
using PokemonLogic;
using PokemonLogic.BuffResults;
using PokemonLogic.PokemonData;
using UI;
using UnityEngine;
using Terrain = Enum.Terrain;

namespace Managers.BattleMgrComponents
{
    public class BattleMgr
    {
        private static BattleMgr _instance;

        public Dictionary<string, ABattlePlayer> PlayerInGame;

        public BattleScenePanelTwoPlayer BattleScenePanelTwoPlayerUI;

        public Pokemon[] OnStagePokemon;
        public ABattlePlayer[] PokemonStageIndex2PlayerMapping;

        private int _roundCount;

        private int _maxAllowPokemonOnStage;

        public ABattlePlayer LocalPlayer;

        public readonly int AwaitTime = 500;

        private Weather _curWeather;

        private Terrain _curTerrain;
        
        private BattleRound _curBattleRound;

        private Dictionary<string, object> _playerLog;

        public List<BattleStackItem> BattleStack;

        public Dictionary<PokemonEssentialSystemManager, bool> SystemLoadingDic;

        //For editor only
#if UNITY_EDITOR
        public BattleRound GetCurBattleRound()
        {
            return _curBattleRound;
        }

        public void SetBattleRoundStatus(BattleRoundStatus status)
        {
            _curBattleRound.Status = status;
        }
#endif
        
        
        //playerlog

        public void PlayerLogStore<T>(string key, T value)
        {
            if (_playerLog.ContainsKey(key))
            {
                Debug.LogError("Key: " + key + "already exists");
                return;
            }
            _playerLog.Add(key, value);
        }

        public T PlayerLogGet<T>(string key)
        {
            if (!_playerLog.ContainsKey(key))
            {
                Debug.LogError("Key: " + key + " doesn't exists");
                return default(T);
            }

            return (T)_playerLog[key];
        }

        public void PlayerLogDelete(string key)
        {
            if (!_playerLog.ContainsKey(key))
            {
                Debug.LogError("Key: " + key + " doesn't exists");
                return;
            }

            _playerLog.Remove(key);
        }

        // basic 
        public static BattleMgr Instance
        {
            get { return _instance ??= new BattleMgr(); }
        }

        private BattleMgr()
        {
            PlayerInGame = new Dictionary<string, BattlePlayer.ABattlePlayer>();
            _roundCount = 0;
        }

        public void InitData(BasicPlayerInfo[] playerInfos, int maxAllowPokemonOnstage)
        {
            _maxAllowPokemonOnStage = maxAllowPokemonOnstage;
            foreach (var playerInfo in playerInfos)
            {
                PlayerInGame.Add(playerInfo.playerID, PlayerDispatcher.InitPlayer(playerInfo));
            }

            OnStagePokemon = new Pokemon[_maxAllowPokemonOnStage * PlayerInGame.Count];
            PokemonStageIndex2PlayerMapping = new ABattlePlayer[_maxAllowPokemonOnStage * PlayerInGame.Count];
            _playerLog = new Dictionary<string, object>();
            BattleStack = new List<BattleStackItem>();
            SystemLoadingDic = new Dictionary<PokemonEssentialSystemManager, bool>();
            foreach (PokemonEssentialSystemManager system in System.Enum.GetValues(typeof(PokemonEssentialSystemManager)))
            {
                SystemLoadingDic.Add(system, false);
            }
            Debug.Log("[BattleMgr] There are " + _maxAllowPokemonOnStage * PlayerInGame.Count + " pokemons allow to be on the battlefield");

            LocalPlayer = PlayerInGame[Constant.PlayerInfo.LocalPlayerID];
            int stageIndex = 0;
            foreach (var pair in PlayerInGame)
            {
                ABattlePlayer player = pair.Value;
                for (int i = 0; i < _maxAllowPokemonOnStage; i++)
                {
                    PokemonStageIndex2PlayerMapping[stageIndex] = player;
                    stageIndex++;
                }
                // load pokemons
                foreach (var pokemonID in player.PlayerInfo.pokemonIDs)
                {
                    player.Pokemons.Add(new Pokemon(PokemonMgr.Instance.GetPokemonByID(pokemonID), player.PlayerInfo.playerID));
                }
                // load items
                foreach (var itemID in player.PlayerInfo.items)
                {
                    // give 10 item for each kind of item by default
                    player.Items.Add(new Item(PokemonMgr.Instance.GetItemTemplateByID(itemID)), 10);
                }
            }

            //load essential manager
            var pokemonMgr = PokemonMgr.Instance;

            EventMgr.Instance.AddListener<Pokemon>(Constant.EventKey.PokemonFaint, PokemonFaint);
            EventMgr.Instance.AddListener<Pokemon, CommonSkillTemplate, CommonResult, PlayablePriority>(Constant.EventKey.RequestLoadPokemonSkill, LoadPokemonSkill);
            EventMgr.Instance.AddListener<Pokemon, int>(Constant.EventKey.RequestSentPokemonOnStage, RequestSentPokemonOnStage);
        }

        public void OnBattleEnd()
        {
            EventMgr.Instance.RemoveListener<Pokemon>(Constant.EventKey.PokemonFaint, PokemonFaint);
            EventMgr.Instance.RemoveListener<Pokemon, CommonSkillTemplate, CommonResult, PlayablePriority>(Constant.EventKey.RequestLoadPokemonSkill, LoadPokemonSkill);
            EventMgr.Instance.RemoveListener<Pokemon, int>(Constant.EventKey.RequestSentPokemonOnStage, RequestSentPokemonOnStage);
            _curBattleRound = null;
        }
        
        public async UniTask SetCommandText(string text)
        {
            if (BattleScenePanelTwoPlayerUI == null)
                return;
            Debug.Log("[BattleMgr] Set command text: " + text);
            await BattleScenePanelTwoPlayerUI.SetCommandText(text);
        }

        private async UniTask CheckManagerReady()
        {
            foreach (PokemonEssentialSystemManager manager in System.Enum.GetValues(typeof(PokemonEssentialSystemManager)))
            {
                while (!SystemLoadingDic[manager])
                {
                    Debug.LogWarning(manager + " haven't got ready");
                    await UniTask.Delay(100);
                }
            }
        }
        
        // get set attributes

        public int GetMaxPokemonAllowOnStage()
        {
            return _maxAllowPokemonOnStage;
        }
        
        public void UpdateRoundCount()
        {
            _roundCount++;
        }

        public int GetRoundCount()
        {
            return _roundCount;
        }

        public int GetPokemonOnstagePosition(Pokemon pokemon)
        {
            for (int i = 0; i < OnStagePokemon.Length; i++)
            {
                if (OnStagePokemon[i] != null && Equals(pokemon, OnStagePokemon[i]))
                    return i;
            }

            return -1; // it isn't on the stage
        }

        public int CountOnstagePokemonByPlayer(ABattlePlayer player)
        {
            int count = 0;
            foreach (var pokemon in OnStagePokemon)
            {
                if (pokemon != null && pokemon.TrainerID == player.PlayerInfo.playerID)
                {
                    count++;
                }
            }

            return count;
        }
        
        public Weather GetWeather()
        {
            return _curWeather;
        }

        public List<Pokemon> GetOnStageAlivePokemon()
        {
            List<Pokemon> result = new List<Pokemon>();
            foreach (var pokemon in OnStagePokemon)
            {
                if (pokemon is { IsFaint: false })
                {
                    result.Add(pokemon);
                }
            }

            return result;
        }

        // playables

        public ABattlePlayable GetCurrentPlayable()
        {
            return _curBattleRound.GetCurrentPlayable();
        }

        public async void StartFirstRound()
        {
            await CheckManagerReady();
            BattleScenePanelTwoPlayerUI.SetPlayerInfo();
            _curBattleRound = new BattleRound(_roundCount);

            for (int i = 0; i < OnStagePokemon.Length; i++)
            {
                RequestSentPokemonOnStage(null, i);
            }

            _curBattleRound.Status = BattleRoundStatus.Running;
            _curBattleRound.ExecuteBattleStage();
        }
        
        public async void EndOfCurRound()
        {
            await BuffMgr.Instance.Update();

            await SetCommandText(" ");
            LoadNextBattleRound();
        }

        public async void LoadNextBattleRound()
        {
            var temp = new BattleRound(_roundCount);
            temp.Status = _curBattleRound.Status;
            temp.InitNormalRound();
            _curBattleRound.OnDestroy();
            _curBattleRound = temp;
            _curBattleRound.AddBattlePlayables(new BpEndOfRound());
            _curBattleRound.AddBattlePlayables(new BpForceAddPokemon());
            _curBattleRound.AddBattlePlayables(new BpHeartBeat());
            var result = new CommonResult();
            await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.StartOfRound, result);
            if (_curBattleRound.Status == BattleRoundStatus.Running)
                _curBattleRound.ExecuteBattleStage();
        }
        
        public void RemoveSkillPlayablesBySource(Pokemon pokemon)
        {
            _curBattleRound.RemoveRunTimeSkillPlayables(pokemon);
        }
        
        public int CancelSkill(Pokemon source, int skillId)
        {
            return _curBattleRound.CancelSkillByPSourcePokemonAndSkillId(source, skillId);
        }
        
        // should be called by battle playables to hand out the control
        public void BattlePlayableEnd()
        {
            Debug.Log("[BattleMgr] Current battle playable end");
            if (_curBattleRound.Status == BattleRoundStatus.Running)
                _curBattleRound.ExecuteBattleStage();
        }

        public async void LoadPokemonSkill(Pokemon pokemon, CommonSkillTemplate template, CommonResult preLoadResult, PlayablePriority priority = PlayablePriority.None)
        {
            var result = new CommonResult();
            result.SkillID = template.ID;
            result = await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.BeforeLoadPokemonSkill, result, pokemon);

            if (!template.IsItem)
            {
                pokemon.ConsumePpBySkillID(result.SkillID);
            }

            var playable = new RunTimeSkillBase(template, pokemon, preLoadResult, (int)priority)
            {
                Available = result.CanLoadSkill
            };
            _curBattleRound.AddBattlePlayables(playable);
            
            EventMgr.Instance.Dispatch(Constant.EventKey.BattleCommandSent, PlayerInGame[pokemon.TrainerID], pokemon);
        }

        public bool IsLastSkill()
        {
            return _curBattleRound.IsLastSkill();
        }

        public BattleStackItem GetPokemonLastSkillData(Pokemon pokemon)
        {
            for (int i = BattleStack.Count - 1; i >= 0; i--)
            {
                if (BattleStack[i].Source.Equals(pokemon))
                {
                    return BattleStack[i];
                }
            }

            return null;
        }

        public void UpdateSkillPriority()
        {
            _curBattleRound.UpdateSkillPriority();
        }

        // pokemon
        private void PokemonFaint(Pokemon pokemon)
        {
            _curBattleRound.AddBattlePlayables(new BpPokemonFaint(pokemon));
        }


        public async UniTask<bool> SetWeather(Weather type)
        {
            if (type == _curWeather)
            {
                await SetCommandText("But fail to change the weather!");
                return false;
            }

            CommonResult result = new CommonResult();
            result.TargetWeather = type;
            await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.OnWeatherChange, result);
                
            if (type == Weather.None)
            {
                switch (_curWeather)
                {
                    case Weather.HarshSunlight: 
                        await SetCommandText("The harsh sunlight faded");
                        break;
                    default:
                        throw new NotImplementedException();
                }
                _curWeather = type;
                await UniTask.Delay(AwaitTime);
                return true;
            }

            if (_curWeather != Weather.None)
            {
                BuffMgr.Instance.RemoveAllWeatherBuff();
            }
            
            _curWeather = type;
            switch (_curWeather)
            {
                case Weather.HarshSunlight:
                    await SetCommandText("The sunlight is harsh!");
                    break;;
                default:
                    throw new NotImplementedException();
            }
            await UniTask.Delay(AwaitTime);
            return true;
        }

        public async UniTask WithDrawPokemon(int pokemonStageIndex)
        {
            Pokemon pokemon = OnStagePokemon[pokemonStageIndex];
            await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.BeforeWithdraw, new CommonResult(), pokemon);
            RemoveSkillPlayablesBySource(pokemon);
            await BuffMgr.Instance.RemoveAllBuffByTarget(pokemon);
            pokemon.OnStage = false;
            BattleScenePanelTwoPlayerUI.GetPokemonBattleInfo(pokemonStageIndex).UnSetPokemonInfo();

            OnStagePokemon[pokemonStageIndex] = null;
        }

        private void RequestSentPokemonOnStage(Pokemon pokemonTobeOnStage, int onStagePosition)
        {
            var player = PokemonStageIndex2PlayerMapping[onStagePosition];
            var targetPokemon = OnStagePokemon[onStagePosition];
            if (OnStagePokemon[onStagePosition] != null)
            {
                _curBattleRound.AddBattlePlayables(new BpWithdrawPokemon(onStagePosition));
            }

            if (pokemonTobeOnStage == null)
            {
                pokemonTobeOnStage = player.GetFirstPokemonCanSent();
                if (pokemonTobeOnStage == null)
                    return;
            }

            pokemonTobeOnStage.OnStage = true; // so it cannot be automatically select to be sent on stage
            _curBattleRound.AddBattlePlayables(new BpDebut(player.PlayerInfo, pokemonTobeOnStage, onStagePosition));
            
            EventMgr.Instance.Dispatch(Constant.EventKey.BattleCommandSent, player, targetPokemon);
            EventMgr.Instance.Dispatch(Constant.EventKey.BattlePokemonForceChangeCommandSent, player);
        }
        public int[] TryAutoGetTarget(Pokemon source, SkillTargetType type)
        {
            System.Diagnostics.Debug.Assert(source != null);
            switch (type)
            {
                case SkillTargetType.Self:
                {
                    int selfIndex = GetPokemonOnstagePosition(source);
                    return new[] { selfIndex };
                }
                case SkillTargetType.OneEnemy:
                {
                    if (OnStagePokemon.Length == 2)
                    {
                        for (int i = 0; i < OnStagePokemon.Count(); i++)
                        {
                            if (OnStagePokemon[i] == null || !Equals(OnStagePokemon[i], source))
                                return new[] { i };
                        }

                        throw new Exception("it should be impossible to reach here");
                    }
                    else
                    {
                        return null;
                    }
                }
                case SkillTargetType.All:
                {
                    return Enumerable.Range(0, OnStagePokemon.Length).ToArray();
                }
                case SkillTargetType.FirstAvailableEnemy:
                {
                    for (int i = 0; i < OnStagePokemon.Length; i++)
                    {
                        if (OnStagePokemon[i] != null && PlayerInGame[OnStagePokemon[i].TrainerID].PlayerInfo.teamID != PlayerInGame[source.TrainerID].PlayerInfo.teamID)
                        {
                            return new[] { i };
                        }
                    }
                    for (int i = 0; i < PokemonStageIndex2PlayerMapping.Length; i++)
                    {
                        if (PokemonStageIndex2PlayerMapping[i].PlayerInfo.teamID != LocalPlayer.PlayerInfo.teamID)
                        {
                            return new[] { i };
                        }
                    }

                    throw new Exception("Impossible to reach here!");
                }
                case SkillTargetType.FirstEnemy:
                {
                    for (int i = 0; i < PokemonStageIndex2PlayerMapping.Length; i++)
                    {
                        if (PokemonStageIndex2PlayerMapping[i].PlayerInfo.teamID != LocalPlayer.PlayerInfo.teamID)
                        {
                            return new[] { i };
                        }
                    }

                    throw new Exception("Impossible to reach here!");
                }
                case SkillTargetType.FirstAvailableTeammate:
                {
                    for (int i = 0; i < OnStagePokemon.Length; i++)
                    {
                        if (OnStagePokemon[i] != null && PlayerInGame[OnStagePokemon[i].TrainerID].PlayerInfo.teamID == PlayerInGame[source.TrainerID].PlayerInfo.teamID)
                        {
                            return new[] { i };
                        }
                    }
                    for (int i = 0; i < PokemonStageIndex2PlayerMapping.Length; i++)
                    {
                        if (PokemonStageIndex2PlayerMapping[i].PlayerInfo.teamID != LocalPlayer.PlayerInfo.teamID)
                        {
                            return new[] { i };
                        }
                    }

                    throw new Exception("Impossible to reach here!");
                }
                case SkillTargetType.FirstTeammate:
                {
                    for (int i = 0; i < PokemonStageIndex2PlayerMapping.Length; i++)
                    {
                        if (PokemonStageIndex2PlayerMapping[i].PlayerInfo.teamID == LocalPlayer.PlayerInfo.teamID)
                        {
                            return new[] { i };
                        }
                    }

                    throw new Exception("Impossible to reach here!");
                }
                case SkillTargetType.AllEnemy:
                {
                    List<int> enemyList = new List<int>();
                    for (int i = 0; i < OnStagePokemon.Count(); i++)
                    {
                        if (OnStagePokemon[i] == null || (!Equals(OnStagePokemon[i], source) && PlayerInGame[OnStagePokemon[i].TrainerID].PlayerInfo.teamID != PlayerInGame[source.TrainerID].PlayerInfo.teamID))
                            enemyList.Add(i);
                    }

                    return enemyList.ToArray();
                }
                case SkillTargetType.OneTeammate:
                    return null;
                case SkillTargetType.AllTeammate:
                {
                    List<int> enemyList = new List<int>();
                    for (int i = 0; i < OnStagePokemon.Count(); i++)
                    {
                        if (OnStagePokemon[i] == null || (!Equals(OnStagePokemon[i], source) && PlayerInGame[OnStagePokemon[i].TrainerID].PlayerInfo.teamID == PlayerInGame[source.TrainerID].PlayerInfo.teamID))
                            enemyList.Add(i);
                    }

                    return enemyList.ToArray();
                }
                case SkillTargetType.AllExceptSelf:
                    break;
                case SkillTargetType.None:
                    break;
            }

            throw new Exception("it should be impossible to reach here for now");
        }
    }
}