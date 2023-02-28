using System;
using System.Collections.Generic;
using System.Linq;
using CoreScripts.BattlePlayables;
using CoreScripts.Managers;
using Cysharp.Threading.Tasks;
using Managers;
using Managers.BattleMgrComponents;
using PokemonDemo.Scripts.BattlePlayables.Skills;
using PokemonDemo.Scripts.BattlePlayables.Stages;
using PokemonDemo.Scripts.BattlePlayer;
using PokemonDemo.Scripts.Enum;
using PokemonDemo.Scripts.Managers;
using PokemonDemo.Scripts.PokemonLogic;
using PokemonDemo.Scripts.PokemonLogic.BuffResults;
using PokemonDemo.Scripts.PokemonLogic.PokemonData;
using PokemonDemo.Scripts.UI;
using UnityEngine;
using Terrain = PokemonDemo.Scripts.Enum.Terrain;
using Types = CoreScripts.Constant.Types;

namespace PokemonDemo.Scripts.BattleMgrComponents
{
    public class BattleMgr : ABattleMgr
    {
        private static BattleMgr _instance;

        public readonly Dictionary<string, APokemonBattlePlayer> PlayerInGame;

        public BattleScenePanelTwoPlayer BattleScenePanelTwoPlayerUI;

        public Pokemon[] OnStagePokemon;
        public APokemonBattlePlayer[] PokemonStageIndex2PlayerMapping;

        private int _maxAllowPokemonOnStage;

        public APokemonBattlePlayer LocalPlayer;

        public readonly int AwaitTime = 500;

        private Weather _curWeather;

        private Terrain _curTerrain;

        private Dictionary<string, object> _playerLog;

        public List<BattleStackItem> BattleStack;

        public Dictionary<PokemonEssentialSystemManager, bool> SystemLoadingDic;


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
            PlayerInGame = new Dictionary<string, APokemonBattlePlayer>();
        }

        public void InitData(BasicPlayerInfo[] playerInfos, int maxAllowPokemonOnstage)
        {
            _maxAllowPokemonOnStage = maxAllowPokemonOnstage;
            foreach (var playerInfo in playerInfos)
            {
                PlayerInGame.Add(playerInfo.playerID, PlayerDispatcher.InitPlayer(playerInfo));
            }

            OnStagePokemon = new Pokemon[_maxAllowPokemonOnStage * PlayerInGame.Count];
            PokemonStageIndex2PlayerMapping = new APokemonBattlePlayer[_maxAllowPokemonOnStage * PlayerInGame.Count];
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
                APokemonBattlePlayer player = pair.Value;
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
            EventMgr.Instance.AddListener<Pokemon, CommonSkillTemplate, PokemonCommonResult, PlayablePriority>(Constant.EventKey.RequestLoadPokemonSkill, LoadPokemonSkill);
            EventMgr.Instance.AddListener<Pokemon, int>(Constant.EventKey.RequestSentPokemonOnStage, RequestSentPokemonOnStage);
        }

        public void OnBattleEnd()
        {
            EventMgr.Instance.RemoveListener<Pokemon>(Constant.EventKey.PokemonFaint, PokemonFaint);
            EventMgr.Instance.RemoveListener<Pokemon, CommonSkillTemplate, PokemonCommonResult, PlayablePriority>(Constant.EventKey.RequestLoadPokemonSkill, LoadPokemonSkill);
            EventMgr.Instance.RemoveListener<Pokemon, int>(Constant.EventKey.RequestSentPokemonOnStage, RequestSentPokemonOnStage);
            CurBattleRound = null;
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

        public int GetPokemonOnstagePosition(Pokemon pokemon)
        {
            for (int i = 0; i < OnStagePokemon.Length; i++)
            {
                if (OnStagePokemon[i] != null && Equals(pokemon, OnStagePokemon[i]))
                    return i;
            }

            return -1; // it isn't on the stage
        }

        public int CountOnstagePokemonByPlayer(APokemonBattlePlayer player)
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
            return CurBattleRound.GetCurrentPlayable();
        }

        public override async void StartFirstRound()
        {
            await CheckManagerReady();
            BattleScenePanelTwoPlayerUI.SetPlayerInfo();
            CurBattleRound = new BattleRound(RoundCount, this);

            for (int i = 0; i < OnStagePokemon.Length; i++)
            {
                RequestSentPokemonOnStage(null, i);
            }

            CurBattleRound.Status = Types.BattleRoundStatus.Running;
            CurBattleRound.ExecuteBattleStage();
        }
        
        public override async void EndOfCurRound()
        {
            if (UpdatedRoundCount != RoundCount)
            {
                await BuffMgr.Instance.Update();
                UpdatedRoundCount = RoundCount;
            }

            if (CurBattleRound.GetRemainingPlayables().Count != 0)
            {
                CurBattleRound.ExecuteBattleStage();
            }

            await SetCommandText(" ");
            LoadNextBattleRound();
        }

        protected override async void LoadNextBattleRound()
        {
            Debug.Log("[BattleMgr] Start new round: " + RoundCount);
            var temp = new BattleRound(RoundCount, this);
            temp.Status = CurBattleRound.Status;
            CurBattleRound.OnDestroy();
            
            CurBattleRound = temp;
            CurBattleRound.AddBattlePlayables(new BpCommandStage());
            CurBattleRound.AddBattlePlayables(new BpEndOfRound());
            CurBattleRound.AddBattlePlayables(new BpForceAddPokemon());
            CurBattleRound.AddBattlePlayables(new BpHeartBeat());
            var result = new PokemonCommonResult();
            await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.StartOfRound, result);
            if (CurBattleRound.Status == Types.BattleRoundStatus.Running)
                CurBattleRound.ExecuteBattleStage();
        }
        
        public void RemoveSkillPlayablesBySource(Pokemon pokemon)
        {
            // CurBattleRound.RemoveRunTimeSkillPlayablesPokemonDemo(pokemon);
            CurBattleRound.RemoveRunTimeSkill(pokemon);
        }
        
        public int CancelSkill(Pokemon source, int skillId)
        {
            return CurBattleRound.CancelSkillByPSourcePokemonAndSkillId(source, skillId);
        }

        public async void LoadPokemonSkill(Pokemon pokemon, CommonSkillTemplate template, PokemonCommonResult preLoadResult, PlayablePriority priority = PlayablePriority.None)
        {
            var result = new PokemonCommonResult();
            result.SkillID = template.ID;
            result = (PokemonCommonResult)await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.BeforeLoadPokemonSkill, result, pokemon);

            if (!template.IsItem)
            {
                pokemon.ConsumePpBySkillID(result.SkillID);
            }

            var playable = new RunTimeSkillBase(template, pokemon, preLoadResult, (int)priority)
            {
                Available = result.CanLoadSkill
            };
            CurBattleRound.AddBattlePlayables(playable);
            
            EventMgr.Instance.Dispatch(Constant.EventKey.BattleCommandSent, PlayerInGame[pokemon.TrainerID], pokemon);
        }

        public bool IsLastSkill()
        {
            return CurBattleRound.IsLastSkill();
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
            CurBattleRound.UpdateSkillPriority();
        }

        // pokemon
        private void PokemonFaint(Pokemon pokemon)
        {
            CurBattleRound.AddBattlePlayables(new BpPokemonFaint(pokemon));
        }


        public async UniTask<bool> SetWeather(Weather type)
        {
            if (type == _curWeather)
            {
                await SetCommandText("But fail to change the weather!");
                return false;
            }

            PokemonCommonResult result = new PokemonCommonResult();
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
            await BuffMgr.Instance.ExecuteBuff(Constant.BuffExecutionTimeKey.BeforeWithdraw, new PokemonCommonResult(), pokemon);
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
                CurBattleRound.AddBattlePlayables(new BpWithdrawPokemon(onStagePosition));
            }

            if (pokemonTobeOnStage == null)
            {
                pokemonTobeOnStage = player.GetFirstPokemonCanSent();
                if (pokemonTobeOnStage == null)
                    return;
            }

            pokemonTobeOnStage.OnStage = true; // so it cannot be automatically select to be sent on stage
            CurBattleRound.AddBattlePlayables(new BpDebut(player.PlayerInfo, pokemonTobeOnStage, onStagePosition));
            
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