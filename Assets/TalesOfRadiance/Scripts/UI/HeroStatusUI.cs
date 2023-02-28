using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace TalesOfRadiance.Scripts.UI
{
    public class HeroStatusUI : MonoBehaviour
    {
        [SerializeField] private Image hpRemain;
        [SerializeField] private Text textPrefab;
        [SerializeField] private Transform effectTextArea;

        private float _targetHpLengthPercentage = 1f;
        private float _hpMaxLength;
        private RectTransform _hpRemainRect;

        private void Awake()
        {
            _hpMaxLength = hpRemain.transform.parent.GetComponent<RectTransform>().sizeDelta.x;
            _hpRemainRect = hpRemain.GetComponent<RectTransform>();
        }

        public void ShowEffectText(string content, Color color)
        {
            UniTask.Void(async () =>
            {
                var text = Instantiate(textPrefab, effectTextArea);
                text.text = content;
                text.color = color;
                GameObject o;
                (o = text.gameObject).SetActive(true);
                var localPosition = o.transform.localPosition;
                while (localPosition.y < 0)
                {
                    localPosition = new Vector3(localPosition.x, localPosition.y + 1, localPosition.z);
                    text.gameObject.transform.localPosition = localPosition;
                    await UniTask.Delay(100);
                }

                Destroy(text);
            });
        }

        public void SetTargetHp(float percentage)
        {
            _targetHpLengthPercentage = percentage;
        }

        public void Update()
        {
            _hpRemainRect.sizeDelta = new Vector2(-(1 - _targetHpLengthPercentage) * _hpMaxLength, _hpRemainRect.sizeDelta.y);
        }
    }
}