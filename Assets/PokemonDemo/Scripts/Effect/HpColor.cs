using UnityEngine;
using UnityEngine.UI;

namespace PokemonDemo.Scripts.Effect
{
    public class HpColor : MonoBehaviour
    {
        private Color _min = Color.green;
        private Color _max = Color.red;

        private Color _def;

        private float _percentage;

        private RectTransform _transform;

        private Image _image;

        private float _maxLength;
        // Start is called before the first frame update
        void Start()
        {
            _def = _max - _min;
            _transform = this.GetComponent<RectTransform>();
            _image = GetComponent<Image>();
            _maxLength = transform.parent.GetComponent<RectTransform>().rect.width;
        }

        // Update is called once per frame
        void Update()
        {
            _percentage = (_maxLength - _transform.rect.width) / _maxLength;
            _image.color = _min + _percentage * _def;
        }
    }
}
