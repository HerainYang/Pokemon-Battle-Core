using System.Collections;
using System.Collections.Generic;
using Managers;
using Managers.BattleMgrComponents.BattlePlayer;
using Managers.BattleMgrComponents.PokemonLogic;
using UnityEngine;
using UnityEngine.UI;

public class PokemonSelectPanel : MonoBehaviour
{
    [SerializeField] private Button backBtn;
    [SerializeField] private PokemonPanelInfoItem[] items;

    private void Awake()
    {
        backBtn.onClick.AddListener(delegate { UIWindowsManager.Instance.HideUIWindow("PokemonSelectPanel"); });
    }

    public void Init(ABattlePlayer curPlayer, Pokemon curPokemon, int targetPosition, bool forceChange)
    {
        var pokemons = curPlayer.Pokemons;
        backBtn.gameObject.SetActive(!forceChange);
        for (int i = 0; i < pokemons.Count; i++)
        {
            _ = UIHelper.SetImageSprite(pokemons[i].ImageKey, items[i].GetImg(), false);
            items[i].Init(i, targetPosition, curPlayer, curPokemon, forceChange);
        }
    }
}