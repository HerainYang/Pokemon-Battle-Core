using PokemonDemo.Scripts.BattleMgrComponents;
using PokemonDemo.Scripts.PokemonLogic.PokemonData;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace PokemonDemo.Scripts.UI.BattleUIComponent
{
    public class PokemonPanelInfoItem : MonoBehaviour
    {
        public Pokemon ThisPokemon;
        private int _targetPosition;

        [SerializeField] private Color valid;
        [SerializeField] private Color faint;
        private Color _current = Color.yellow;
    
        private Color _selected = Color.cyan;

        [SerializeField] private Image bg;

        [SerializeField] private Image pokeImg;
        [SerializeField] private RectTransform hpRemain;
        private Image _hp;
        [SerializeField] private Text status;
        public Button HotArea;

        private Color _max = Color.green;
        private Color _min = Color.red;
        private Color _def;

        public bool Selected = false;

        private void Awake()
        {
            _def = _max - _min;
            _hp = hpRemain.GetComponent<Image>();
        }

        public void Init(Pokemon thisPokemon, int targetPosition)
        {
            ThisPokemon = thisPokemon;
            _targetPosition = targetPosition;
            SetColor(false);
            float percentage = ((float)ThisPokemon.GetHp() / ThisPokemon.HpMax);
            hpRemain.sizeDelta = new Vector2(percentage * 250, hpRemain.sizeDelta.y);
            _hp.color = _min + percentage * _def;
        }

        public void SetColor(bool selected)
        {
            if (selected)
            {
                bg.color = _selected;
                return;
            }
            bg.color = ThisPokemon.IsFaint ? faint : valid;
            if (ThisPokemon.OnStage && !ThisPokemon.IsFaint && ThisPokemon.TrainerID == BattleMgr.Instance.LocalPlayer.PlayerInfo.playerID && BattleMgr.Instance.OnStagePokemon[_targetPosition] != null && ThisPokemon.Equals(BattleMgr.Instance.OnStagePokemon[_targetPosition]))
            {
                bg.color = _current;
            }
        }

        public Image GetImg()
        {
            return pokeImg;
        }
    }
}