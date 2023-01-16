using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Enum;
using Managers;
using Managers.BattleMgrComponents;
using Managers.BattleMgrComponents.BattlePlayables.Skills;
using Managers.BattleMgrComponents.BattlePlayer;
using Managers.BattleMgrComponents.PokemonLogic;
using PokemonLogic;
using UI.BattleUIComponent;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace UI
{
    public class BattleScenePanelTwoPlayer : MonoBehaviour
    {
        [SerializeField] private Text commandText;

        [Header("Buttons")]
        [SerializeField] private Button[] btns;

        [SerializeField]
        private Text[] btnTexts;

        [Header("Pokemon Info")] 
        // public PokemonBattleInfo selfPokemonInfo;
        // public PokemonBattleInfo opPokemonInfo;

        public Transform selfPokemonHorizontalLayout;
        public Transform opPokemonHorizontalLayout;

        public Dictionary<int, PokemonBattleInfo> PokemonBattleInfoDic;

        [Header("Command Mask")] [SerializeField]
        private GameObject commandMask;

        [Header("Self Player Info")] [SerializeField]
        private Transform selfInfoBar;

        [SerializeField] private Text selfNameTag;

        [Header("Opponent Player Info")] [SerializeField]
        private Transform opInfoBar;

        [SerializeField] private Text opNameTag;

        [SerializeField] private ScrollRect historyViewVerticalBar;
        [SerializeField] private Transform historyCommandArea;
        private GameObject _commandTextPrefab;

        //UI
        private List<Grayout> _selfBalls;
        private List<Grayout> _opBalls;
        [SerializeField] private GameObject _pokemonSelectPanel;

        //Round Data
        private ABattlePlayer _curPlayer;
        private Pokemon _curPokemon;

        private CommandRequestType _requestType;


        private async void Awake()
        {
            BattleMgr.Instance.BattleScenePanelTwoPlayerUI = this;

            _selfBalls = new List<Grayout>();
            _opBalls = new List<Grayout>();

            for (int i = 0; i < btnTexts.Length; i++)
            {
                var i1 = i;
                btns[i].onClick.AddListener(delegate { SendLoadRequest(i1); });
            }

            PokemonBattleInfoDic = new Dictionary<int, PokemonBattleInfo>();

            // generate pokemon battle information display area for every possible pokemon standing area
            var handle = Addressables.LoadAssetAsync<GameObject>("PokemonBattleInfo");
            await handle;
            if (handle.Result == null)
            {
                Debug.LogError("Cannot Load PokemonBattleInfo");
            }
            else
            {
                for (int i = 0;  i< BattleMgr.Instance.PokemonStageIndex2PlayerMapping.Length;i++)
                {
                    var player = BattleMgr.Instance.PokemonStageIndex2PlayerMapping[i];
                    GameObject o;
                    o = Instantiate(handle.Result, player == BattleMgr.Instance.LocalPlayer ? selfPokemonHorizontalLayout : opPokemonHorizontalLayout);
                    PokemonBattleInfoDic.Add(i, o.GetComponent<PokemonBattleInfo>());
                }
            }

            BattleMgr.Instance.SystemLoadingDic[PokemonEssentialSystemManager.PokemonBattlePanel] = true;
        }

        private void OnEnable()
        {
            EventMgr.Instance.AddListener<ABattlePlayer, Pokemon>(Constant.EventKey.BattleCommandSent, ShowCommandMask);
        }

        private void OnDisable()
        {
            EventMgr.Instance.RemoveListener<ABattlePlayer, Pokemon>(Constant.EventKey.BattleCommandSent, ShowCommandMask);
        }

        private void OnDestroy()
        {
            foreach (var btn in btns)
            {
                btn.onClick.RemoveAllListeners();
            }
        }

        public PokemonBattleInfo GetPokemonBattleInfo(int pokemonStageIndex)
        {
            return PokemonBattleInfoDic[pokemonStageIndex];
        }

        private void HideCommandMask()
        {
            commandMask.SetActive(false);
        }

        private void ShowCommandMask(ABattlePlayer player, Pokemon pokemon)
        {
            if (_curPlayer == player && Equals(_curPokemon, pokemon))
            {
                commandMask.SetActive(true);
            }
        }

        public void StartCommandStage(ABattlePlayer player, Pokemon pokemon)
        {
            Debug.LogWarning("player turn " + pokemon.Name);
            _curPlayer = player;
            _curPokemon = pokemon;
            HideCommandMask();
            DefaultBs();
        }

        public void StartForcePokemonSelect(ABattlePlayer player, int onStagePosition)
        {
            _curPlayer = player;
            _curPokemon = null;
            ShowSelectPanel(onStagePosition, true, CommandRequestType.Pokemons);
        }

        public async UniTask SetCommandText(string text)
        {
            commandText.text = text;
            if (_commandTextPrefab == null)
            {
                var handler = Addressables.LoadAssetAsync<GameObject>("HistoryCommandText");
                await handler;
                _commandTextPrefab = handler.Result;
            }

            var temp = Instantiate(_commandTextPrefab, historyCommandArea);
            temp.GetComponent<Text>().text = text;

            await UniTask.Delay(BattleMgr.Instance.AwaitTime);
            Canvas.ForceUpdateCanvases();

            historyViewVerticalBar.normalizedPosition = new Vector2(0, 0);
        }

        //Button Setup (BS)
        private void DefaultBs()
        {
            btnTexts[0].text = "Skills";
            btnTexts[1].text = "Pokemons";
            btnTexts[2].text = "Items";
            btnTexts[3].text = "Run";
            btnTexts[4].text = "Catch";
            for (int index = 0; index < btns.Length; index++)
            {
                btns[index].interactable = true;
            }
            _requestType = CommandRequestType.Default;
        }

        private void SkillsBs()
        {
            int[] skills = _curPokemon.GetSkills();
            int index = 0;
            for (; index < skills.Length; index++)
            {
                btns[index].interactable = _curPokemon.CanUseSkillByIndex(index);
                btnTexts[index].text = PokemonMgr.Instance.GetSkillTemplateByID(skills[index]).Name + " [" + _curPokemon.Pps[index] + "/" + PokemonMgr.Instance.GetSkillTemplateByID(skills[index]).PowerPoint + "]";
            }

            while (index < 4)
            {
                btns[index++].interactable = false;
            }
            btnTexts[4].text = "Back";
            _requestType = CommandRequestType.Skills;
        }

        private void ShowSelectPanel(int onStagePosition, bool forceChange, CommandRequestType requestType, int skillIndex = 0)
        {
            _pokemonSelectPanel.SetActive(true);
            switch (requestType)
            {
                case CommandRequestType.Pokemons:
                    _pokemonSelectPanel.GetComponent<PokemonSelectPanel>().Init(onStagePosition, forceChange, requestType, _curPlayer.Pokemons);
                    break;
                case CommandRequestType.Skills:
                    List<Pokemon> OnstageNotNullPokemon = BattleMgr.Instance.GetOnStageAlivePokemon();

                    _pokemonSelectPanel.GetComponent<PokemonSelectPanel>().Init(onStagePosition, forceChange, requestType, OnstageNotNullPokemon, skillIndex);
                    break;
                default:
                    throw new NotImplementedException("impossible to reach here");
            }
        }

        private async void SendLoadRequest(int index)
        {
            if (_requestType == CommandRequestType.Default)
            {
                if (index == 0)
                {
                    SkillsBs();
                }
                else if (index == 1)
                {
                    ShowSelectPanel(BattleMgr.Instance.GetPokemonOnstagePosition(_curPokemon), false, CommandRequestType.Pokemons);
                }
            }
            else if (_requestType == CommandRequestType.Skills)
            {
                if (index == 4)
                {
                    DefaultBs();
                    return;
                }

                DefaultBs();
                int[] targets = await BattleMgr.Instance.TryAutoGetTarget(_curPokemon, PokemonMgr.Instance.GetSkillTemplateByID(_curPokemon.GetSkills()[index]).TargetType);
                if (targets == null) // need manually select
                {
                    ShowSelectPanel(BattleMgr.Instance.GetPokemonOnstagePosition(_curPokemon), false, CommandRequestType.Skills, index);
                }
                else
                {
                    EventMgr.Instance.Dispatch(Constant.EventKey.RequestLoadPokemonSkill, _curPokemon, index, targets);
                }
            }
        }

        public async void SetPlayerInfo()
        {
            var selfPlayer = BattleMgr.Instance.LocalPlayer.PlayerInfo;
            BasicPlayerInfo opponent = null;
            foreach (var pair in BattleMgr.Instance.PlayerInGame)
            {
                if (pair.Value != BattleMgr.Instance.LocalPlayer)
                {
                    opponent = pair.Value.PlayerInfo;
                }
            }

            if (opponent == null)
            {
                throw new Exception("Opponent not found!");
            }
            
            selfNameTag.text = selfPlayer.name;
            opNameTag.text = opponent.name;
            var handler = Addressables.LoadAssetAsync<GameObject>("BallPrefab");
            await handler;

            //self balls
            for (int i = 0; i < selfPlayer.pokemonIDs.Length; i++)
            {
                var temp = Instantiate(handler.Result, selfInfoBar);
                _selfBalls.Add(temp.GetComponent<Grayout>());
            }

            //op balls
            for (int i = 0; i < opponent.pokemonIDs.Length; i++)
            {
                var temp = Instantiate(handler.Result, opInfoBar);
                _opBalls.Add(temp.GetComponent<Grayout>());
            }
        }

        public void SetPokeBallStatus(ABattlePlayer player, Pokemon pokemon, int position)
        {
            if (player == BattleMgr.Instance.LocalPlayer)
            {
                _selfBalls[position].SetStatus(pokemon.IsFaint);
            }
            else
            {
                _opBalls[position].SetStatus(pokemon.IsFaint);
            }
        }
    }
}