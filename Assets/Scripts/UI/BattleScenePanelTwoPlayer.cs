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

        [Header("Pokemon Info")] public PokemonBattleInfo selfPokemonInfo;
        public PokemonBattleInfo opPokemonInfo;

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

        //Round Data
        private ABattlePlayer _curPlayer;
        private Pokemon _curPokemon;

        private CommandRequestType _requestType;


        private void Awake()
        {
            BattleMgr.Instance.BattleScenePanelTwoPlayerUI = this;
            BattleMgr.Instance.StartFirstRound();

            _selfBalls = new List<Grayout>();
            _opBalls = new List<Grayout>();

            for (int i = 0; i < btnTexts.Length; i++)
            {
                var i1 = i;
                btns[i].onClick.AddListener(delegate { SendLoadRequest(i1); });
            }
            
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
            _curPlayer = player;
            _curPokemon = pokemon;
            HideCommandMask();
            DefaultBs();
        }

        public void StartForcePokemonSelect(ABattlePlayer player, int onStagePosition)
        {
            _curPlayer = player;
            _curPokemon = null;
            ShowSelectPanel(onStagePosition, true);
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

        private void ShowSelectPanel(int onStagePosition, bool forceChange)
        {
            UIWindowsManager.Instance.ShowUIWindowAsync("PokemonSelectPanel").ContinueWith((o =>
            {
                o.GetComponent<PokemonSelectPanel>().Init(_curPlayer, _curPokemon, onStagePosition, forceChange);
            }));
        }

        private void SendLoadRequest(int index)
        {
            if (_requestType == CommandRequestType.Default)
            {
                if (index == 0)
                {
                    SkillsBs();
                }
                else if (index == 1)
                {
                    ShowSelectPanel(BattleMgr.Instance.GetPokemonOnstagePosition(_curPokemon), false);
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
                EventMgr.Instance.Dispatch(Constant.EventKey.RequestLoadPokemonSkill, _curPokemon, index);
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