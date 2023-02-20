using System.Collections.Generic;
using PokemonDemo.Scripts.BattleMgrComponents;
using PokemonDemo.Scripts.BattlePlayables.Skills;
using PokemonDemo.Scripts.Enum;
using PokemonDemo.Scripts.Managers;
using PokemonDemo.Scripts.PokemonLogic.PokemonData;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace PokemonDemo.Scripts.UI.BattleUIComponent
{
    public class PokemonSelectPanel : MonoBehaviour
    {
        [SerializeField] private Button backBtn;
        [SerializeField] private Button confirmBtn;
        [SerializeField] private Transform layoutGroupL;
        [SerializeField] private Transform layoutGroupR;

        [SerializeField] private GameObject itemPrefab;

        [SerializeField] private Color transparentColor;
        [SerializeField] private Color playerGroupColor;
        [SerializeField] private Color enemyGroupColor;

        private CommandRequestType _commandRequestType;

        private List<Pokemon> _selectPokemon;
        private int _requireNumberOfPokemon = 1;
        private int _targetPosition;

        private void AddToSelectGroup(Pokemon pokemon)
        {
            _selectPokemon.Add(pokemon);
            confirmBtn.interactable = _selectPokemon.Count == _requireNumberOfPokemon;
        }
        
        private void RemoveFromSelectGroup(Pokemon pokemon)
        {
            _selectPokemon.Remove(pokemon);
            confirmBtn.interactable = _selectPokemon.Count == _requireNumberOfPokemon;
        }

        private void ItemOnClick(PokemonPanelInfoItem item)
        {
            if (item.Selected)
            {
                RemoveFromSelectGroup(item.ThisPokemon);
                item.SetColor(false);
            }
            else
            {
                AddToSelectGroup(item.ThisPokemon);
                item.SetColor(true);
            }

            item.Selected = !item.Selected;
        }

        private void OnEnable()
        {
            backBtn.onClick.AddListener(BackBehavior);
            confirmBtn.onClick.AddListener(ConfirmBehavior);
            _selectPokemon = new List<Pokemon>();
        }

        private void OnDisable()
        {
            backBtn.onClick.RemoveAllListeners();
            confirmBtn.onClick.RemoveAllListeners();
        }

        private void ConfirmBehavior()
        {
            List<int> indices;
            switch (_commandRequestType)
            {
                case CommandRequestType.Pokemons:
                    // EventMgr.Instance.Dispatch(Constant.EventKey.RequestSentPokemonOnStage, _selectPokemon[0], _targetPosition);
                    EventMgr.Instance.Dispatch(Constant.UIEventKey.ClosePokemonSelectWindow, new []{BattleMgr.Instance.PlayerInGame[_selectPokemon[0].TrainerID].Pokemons.IndexOf(_selectPokemon[0])});
                    BackBehavior();
                    break;
                case CommandRequestType.Skills:
                case CommandRequestType.Items:
                    indices = new List<int>();
                    foreach (var pokemon in _selectPokemon)
                    {
                        indices.Add(BattleMgr.Instance.GetPokemonOnstagePosition(pokemon));
                    }
                    // EventMgr.Instance.Dispatch(Constant.EventKey.RequestLoadPokemonSkill, BattleMgr.Instance.OnStagePokemon[_targetPosition], _skillIndex, indices.ToArray());
                    EventMgr.Instance.Dispatch(Constant.UIEventKey.ClosePokemonSelectWindow, indices.ToArray());
                    BackBehavior();
                    break;
            }
        }

        private void BackBehavior()
        {
            _selectPokemon = null;
            foreach (Transform child in layoutGroupL)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in layoutGroupR)
            {
                Destroy(child.gameObject);
            }
            gameObject.SetActive(false);
            EventMgr.Instance.Dispatch(Constant.UIEventKey.ClosePokemonSelectWindow, (int[]) null);
            UIWindowsManager.Instance.HideUIWindow("PokemonSelectPanel");
        }

        public void Init(int targetPosition, bool forceChange, CommandRequestType commandRequestType, List<Pokemon> pokemonOptionList, CommonSkillTemplate template = null)
        {
            backBtn.gameObject.SetActive(!forceChange);
            _commandRequestType = commandRequestType;
            _targetPosition = targetPosition;

            switch (commandRequestType)
            {
                case CommandRequestType.Items:
                {
                    // For simplification, only consider to own pokemon
                    layoutGroupL.GetComponent<Image>().color = transparentColor;
                    layoutGroupR.GetComponent<Image>().color = transparentColor;
                    for (int i = 0; i < pokemonOptionList.Count; i++)
                    {
                        var item = Instantiate(itemPrefab, i % 2 == 0 ? layoutGroupL : layoutGroupR).GetComponent<PokemonPanelInfoItem>();
                        item.Init(pokemonOptionList[i], targetPosition);
                        _ = UIHelper.SetImageSprite(pokemonOptionList[i].ImageKey, item.GetImg(), false);
                        item.HotArea.onClick.AddListener(() => ItemOnClick(item));
                    }

                    break;
                }
                case CommandRequestType.Pokemons:
                {
                    layoutGroupL.GetComponent<Image>().color = transparentColor;
                    layoutGroupR.GetComponent<Image>().color = transparentColor;
                    for (int i = 0; i < pokemonOptionList.Count; i++)
                    {
                        var item = Instantiate(itemPrefab, i % 2 == 0 ? layoutGroupL : layoutGroupR).GetComponent<PokemonPanelInfoItem>();
                        item.Init(pokemonOptionList[i], targetPosition);
                        _ = UIHelper.SetImageSprite(pokemonOptionList[i].ImageKey, item.GetImg(), false);
                        item.HotArea.interactable = (!pokemonOptionList[i].OnStage && !pokemonOptionList[i].IsFaint);
                        item.HotArea.onClick.AddListener(() => ItemOnClick(item));
                    }

                    break;
                }
                
                case CommandRequestType.Skills:
                {
                    layoutGroupL.GetComponent<Image>().color = playerGroupColor;
                    layoutGroupR.GetComponent<Image>().color = enemyGroupColor;
                    var curPokemon = BattleMgr.Instance.OnStagePokemon[_targetPosition];
                    for (int i = 0; i < pokemonOptionList.Count; i++)
                    {
                        var item = Instantiate(itemPrefab, pokemonOptionList[i].TrainerID == BattleMgr.Instance.LocalPlayer.PlayerInfo.playerID ? layoutGroupL : layoutGroupR).GetComponent<PokemonPanelInfoItem>();
                        item.Init(pokemonOptionList[i], targetPosition);
                        _ = UIHelper.SetImageSprite(pokemonOptionList[i].ImageKey, item.GetImg(), false);
                        Assert.IsNotNull(template);
                        if (template.TargetType == SkillTargetType.OneEnemy && pokemonOptionList[i].TrainerID == BattleMgr.Instance.LocalPlayer.PlayerInfo.playerID)
                        {
                            item.HotArea.interactable = false;
                        }
                        item.HotArea.onClick.AddListener(() => ItemOnClick(item));
                    }

                    break;
                }
            }
        }
    }
}