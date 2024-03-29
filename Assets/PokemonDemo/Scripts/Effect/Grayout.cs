using UnityEngine;
using UnityEngine.UI;

namespace PokemonDemo.Scripts.Effect
{
    public class Grayout : MonoBehaviour
    {
        public bool isGray;

        [SerializeField] private Texture2D normal;
        [SerializeField] private Texture2D gray;

        private RawImage _image;

        private void Awake()
        {
            _image = GetComponent<RawImage>();

            if (isGray)
            {
                TurnGray();
            }
            else
            {
                TurnNormal();
            }
        }

        public void SetStatus(bool isFaint)
        {
            if (isFaint)
            {
                TurnGray();
            }
            else
            {
                TurnNormal();
            }
        }

        private void TurnNormal()
        {
            _image.texture = normal;
            _image.enabled = true;
        }


        public void TurnGray()
        {
            _image.texture = gray;
            _image.enabled = true;
        }
    }
}
