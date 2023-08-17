using System;
using Cysharp.Threading.Tasks;
using TalesOfRadiance.Scripts.Battle.BattleComponents;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;
using TalesOfRadiance.Scripts.Battle.Managers;
using TalesOfRadiance.Scripts.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TalesOfRadiance.Scripts.Character
{
    public class CharacterAnchor : MonoBehaviour
    {
        private GameObject _characterModel;
        private GameObject _heroUI;
        private HeroStatusUI _heroStatusUI;
        public RuntimeHero Hero;
        public int position;
        public bool isActive = false;
        public async UniTask Init(int index, CharacterTeam team)
        {
            isActive = true;
            HeroTemplate template = ConfigManager.Instance.GetHeroTemplateByID(index);
            Hero = new RuntimeHero(template, team, this);
            var handler = Addressables.LoadAssetAsync<GameObject>(template.ModelKey);
            await handler;
            _characterModel = Instantiate(handler.Result, transform);
            _characterModel.transform.localPosition = new Vector3(0, 0, 0);
            _characterModel.transform.localRotation = new Quaternion(0, 0, 0, 0);
            _characterModel.transform.localScale = new Vector3(1, 1, -1);

            var uiHandler = Addressables.LoadAssetAsync<GameObject>("HeroUI");
            await uiHandler;
            _heroUI = Instantiate(uiHandler.Result, BattleMgr.Instance.CameraSpaceCanvas.transform);
            Vector3 onScreenPosition = BattleMgr.Instance.UICamera.WorldToScreenPoint(_characterModel.transform.position);
            _heroUI.transform.localPosition = new Vector3((onScreenPosition.x - Screen.width / 2), (onScreenPosition.y - Screen.height / 2), 0);

            _heroStatusUI = _heroUI.GetComponent<HeroStatusUI>();
        }

        public void SetTargetHp(float percentage)
        {
            _heroStatusUI.SetTargetHp(percentage);
        }

        public void ShowEffectText(string content, Color color = default(Color))
        {
            if (color == default(Color))
            {
                _heroStatusUI.ShowEffectText(content, Color.black);
            }
            else
            {
                _heroStatusUI.ShowEffectText(content, color);
            }
            
        }

        private void OnDisable()
        {
            if(_heroUI == null)
                return;
            _heroUI.SetActive(false);
        }

        private void OnEnable()
        {
            if (_heroUI != null)
            {
                _heroUI.SetActive(true);
            }
        }
    }
}
