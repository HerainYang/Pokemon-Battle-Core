using System;
using System.Collections.Generic;
using Enum;
using Managers;
using Managers.BattleMgrComponents;
using Managers.BattleMgrComponents.BattlePlayables.Skills;
using Managers.BattleMgrComponents.BattlePlayer;
using Managers.BattleMgrComponents.PokemonLogic;
using PokemonLogic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.BattleUIComponent
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
        private int _skillIndex;

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
            switch (_commandRequestType)
            {
                case CommandRequestType.Pokemons:
                    EventMgr.Instance.Dispatch(Constant.EventKey.RequestSentPokemonOnStage, _selectPokemon[0], _targetPosition);
                    BackBehavior();
                    break;
                default:
                    List<int> indices = new List<int>();
                    foreach (var pokemon in _selectPokemon)
                    {
                        indices.Add(BattleMgr.Instance.GetPokemonOnstagePosition(pokemon));
                    }
                    EventMgr.Instance.Dispatch(Constant.EventKey.RequestLoadPokemonSkill, BattleMgr.Instance.OnStagePokemon[_targetPosition], _skillIndex, indices.ToArray());
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
        }

        public void Init(int targetPosition, bool forceChange, CommandRequestType commandRequestType, List<Pokemon> pokemonOptionList, int skillIndex = 0)
        {
            backBtn.gameObject.SetActive(!forceChange);
            _commandRequestType = commandRequestType;
            _targetPosition = targetPosition;
            _skillIndex = skillIndex;

            switch (commandRequestType)
            {
                case CommandRequestType.Pokemons:
                {
                    layoutGroupL.GetComponent<Image>().color = transparentColor;
                    layoutGroupR.GetComponent<Image>().color = transparentColor;
                    _requireNumberOfPokemon = 1;
                    for (int i = 0; i < pokemonOptionList.Count; i++)
                    {
                        var item = Instantiate(itemPrefab, i % 2 == 0 ? layoutGroupL : layoutGroupR).GetComponent<PokemonPanelInfoItem>();
                        item.Init(pokemonOptionList[i], targetPosition);
                        _ = UIHelper.SetImageSprite(pokemonOptionList[i].ImageKey, item.GetImg(), false);
                        item.HotArea.onClick.AddListener(() => ItemOnClick(item));
                    }

                    break;
                }
                
                case CommandRequestType.Skills:
                {
                    layoutGroupL.GetComponent<Image>().color = playerGroupColor;
                    layoutGroupR.GetComponent<Image>().color = enemyGroupColor;
                    var curPokemon = BattleMgr.Instance.OnStagePokemon[_targetPosition];
                    CommonSkillTemplate template = PokemonMgr.Instance.GetSkillTemplateByID(curPokemon.GetSkills()[skillIndex]);
                    for (int i = 0; i < pokemonOptionList.Count; i++)
                    {
                        var item = Instantiate(itemPrefab, pokemonOptionList[i].TrainerID == BattleMgr.Instance.LocalPlayer.PlayerInfo.playerID ? layoutGroupL : layoutGroupR).GetComponent<PokemonPanelInfoItem>();
                        item.Init(pokemonOptionList[i], targetPosition);
                        _ = UIHelper.SetImageSprite(pokemonOptionList[i].ImageKey, item.GetImg(), false);
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