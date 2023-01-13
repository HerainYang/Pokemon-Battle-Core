using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Managers;
using Managers.BattleMgrComponents;
using Managers.BattleMgrComponents.BattlePlayer;
using Managers.BattleMgrComponents.PokemonLogic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CanEditMultipleObjects]
public class PokemonPanelInfoItem : MonoBehaviour
{
    private int _index;
    private int _targetPosition;
    private ABattlePlayer _curPlayer;
    private Pokemon _curPokemon;
    private bool _forceChange;

    [SerializeField] private Color valid;
    [SerializeField] private Color faint;
    private Color _current = Color.yellow;

    [SerializeField] private Image bg;

    [SerializeField] private Image pokeImg;
    [SerializeField] private RectTransform hpRemain;
    private Image _hp;
    [SerializeField] private Text status;
    [SerializeField] private Button hotArea;

    private Color _max = Color.green;
    private Color _min = Color.red;
    private Color _def;

    private void OnEnable()
    {
        hotArea.onClick.AddListener(SentPokemonChangeRequest);
    }

    private void OnDisable()
    {
        hotArea.onClick.RemoveListener(SentPokemonChangeRequest);
    }

    private void Awake()
    {
        _def = _max - _min;
        _hp = hpRemain.GetComponent<Image>();
    }

    public void Init(int index, int targetPosition, ABattlePlayer curPlayer, Pokemon curPokemon, bool forceChange)
    {
        _index = index;
        _targetPosition = targetPosition;
        _curPlayer = curPlayer;
        _curPokemon = curPokemon;
        _forceChange = forceChange;
        bg.color = _curPlayer.Pokemons[index].IsFaint ? faint : valid;
        if (Equals(curPokemon, _curPlayer.Pokemons[index]))
        {
            bg.color = _current;
        }
        float percentage = ((float)_curPlayer.Pokemons[index].GetHp() / _curPlayer.Pokemons[index].HpMax);
        hpRemain.sizeDelta = new Vector2(percentage * 250, hpRemain.sizeDelta.y);
        _hp.color = _min + percentage * _def;
    }

    public Image GetImg()
    {
        return pokeImg;
    }

    private async void SentPokemonChangeRequest()
    {
        if (!_curPlayer.Pokemons[_index].IsFaint && !_curPlayer.Pokemons[_index].OnStage)
        {
            UIWindowsManager.Instance.HideUIWindow("PokemonSelectPanel");
            if (_curPokemon != null)
            {
                await BattleMgr.Instance.SetCommandText("Come back! " + _curPokemon.Name);
                await UniTask.Delay(BattleMgr.Instance.AwaitTime);
            }
            EventMgr.Instance.Dispatch(Constant.EventKey.RequestSentPokemonOnStage, _index, _targetPosition);
        }

    }
}