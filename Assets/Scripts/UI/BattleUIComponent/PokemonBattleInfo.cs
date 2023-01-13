using System;
using Cysharp.Threading.Tasks;
using Managers;
using Managers.BattleMgrComponents;
using Managers.BattleMgrComponents.PokemonLogic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.BattleUIComponent
{
    public class PokemonBattleInfo : MonoBehaviour
    {
        [SerializeField] private Image pokemonImg;
        public Text pokemonName;
        public Text pokemonAttribute;
        public RectTransform hp;

        private Pokemon _curPokemon;
        
        private Color _min = Color.green;
        private Color _max = Color.red;
        private Color _def;
        private float _hpPercentage;
        private RectTransform _hpTransform;
        private RectTransform _pokemonImgTransform;

        private Image _hpImage;
        // Start is called before the first frame update
        void Start()
        {
            _def = _max - _min;
            _hpTransform = hp.GetComponent<RectTransform>();
            _hpImage = hp.GetComponent<Image>();
            _pokemonImgTransform = pokemonImg.GetComponent<RectTransform>();
        }

        // Update is called once per frame
        void Update()
        {
            _hpPercentage = (400 - _hpTransform.rect.width) / 400;
            _hpImage.color = _min + _hpPercentage * _def;
        }

        private void Awake()
        {
            EventMgr.Instance.AddListener<int, Pokemon>(Constant.EventKey.HpChange, SetHpAnimate);
        }

        private void OnDestroy()
        {
            EventMgr.Instance.RemoveListener<int, Pokemon>(Constant.EventKey.HpChange, SetHpAnimate);
        }

        public void SetPokemonInfo(Pokemon pokemon)
        {
            _curPokemon = pokemon;
            UIHelper.SetImageSprite(_curPokemon.ImageKey, pokemonImg, true).ContinueWith((() =>
            {
                pokemonImg.enabled = true;
                PokemonDebutAnimate();
            }));
            pokemonName.text = _curPokemon.Name;
            pokemonAttribute.text = _curPokemon.Attribute.Name;
            SetHpAnimate(pokemon.GetHp(), pokemon);
        }

        public async UniTask SetPokemonImg(string imgKey)
        {
            await UIHelper.SetImageSprite(imgKey, pokemonImg, true).ContinueWith((() =>
            {
                PokemonDebutAnimate();
            }));
        }
        
        public void SetPokemonImgActive(bool active)
        {
            pokemonImg.enabled = active;
        } 

        public void UnSetPokemonInfo()
        {
            _curPokemon = null;
            pokemonName.text = "";
            pokemonAttribute.text = "";
            pokemonImg.enabled = false;
        }

        public void SetAttributeText(string text)
        {
            pokemonAttribute.text = text;
        }
    
        private void SetHpAnimate(int hpTo, Pokemon target)
        {
            // float targetLength = ((float)(hpTo) / target.GetHpMax()) * 400;
            // hp.sizeDelta = new Vector2(targetLength, hp.rect.height);
            if (_curPokemon != null && target.RuntimeID != _curPokemon.RuntimeID)
            {
                return;
            }
            UniTask.Void((async () =>
            {
                float targetLength = ((float)(hpTo) / target.GetHpMax()) * 400;
                var t = targetLength < hp.rect.width ? -1f : 1f;
                var diff = Math.Abs(hp.rect.width - targetLength);
                int delayTime = (int) ((BattleMgr.Instance.AwaitTime - 200) / (diff / 1));
                Rect hpRect = hp.rect;
                while (Math.Abs(hp.rect.width - targetLength) > 1)
                {
                    var length = hp.rect.width + t > 400 ? 400 : hp.rect.width + t;
                    length = length < 0 ? 0 : length;
                    hp.sizeDelta = new Vector2(length, hpRect.height);
                    await UniTask.Delay(delayTime);
                }
            }));
        }

        private void PokemonDebutAnimate()
        {
            UniTask.Void((async () =>
            {
                float targetLength = 100;
                float curLength = 0;
                var t = 5f;
                
                int delayTime = (int) ((float)(BattleMgr.Instance.AwaitTime - 200) / 20);
                while (Math.Abs(_pokemonImgTransform.rect.width - targetLength) > 5)
                {
                    curLength += t;
                    var length = curLength;
                    _pokemonImgTransform.sizeDelta = new Vector2(length, length);
                    await UniTask.Delay(delayTime);
                }
            }));
        }
    }
}
