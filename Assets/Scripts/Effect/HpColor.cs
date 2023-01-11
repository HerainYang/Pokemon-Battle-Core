using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpColor : MonoBehaviour
{
    private Color _min = Color.green;
    private Color _max = Color.red;

    private Color _def;

    private float _percentage;

    private RectTransform _transform;

    private Image _image;
    // Start is called before the first frame update
    void Start()
    {
        _def = _max - _min;
        _transform = this.GetComponent<RectTransform>();
        _image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        _percentage = (400 - _transform.rect.width) / 400;
        _image.color = _min + _percentage * _def;
    }
}
