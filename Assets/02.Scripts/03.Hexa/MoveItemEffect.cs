using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveItemEffect : MonoBehaviour
{
    private SpriteRenderer _sprite;

    private Vector2 _startPos;
    private Vector2 _endPos;
    private float _timer;
    private float _timerMax;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _timerMax = MainGameDataSo.Instance.MoveItemEffectTime;
    }

    public void Init(Vector2 startPos, Vector2 endPos, Color color)
    {
        _startPos = startPos;
        _endPos = endPos;
        _sprite.color = color;
        _timer = 0;
        transform.position = startPos;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        transform.position = Vector2.Lerp(_startPos, _endPos, (_timer / _timerMax));
        transform.localScale = Vector2.one * (.25f - .2f * Mathf.Abs(0.5f - (_timer / _timerMax)));
        if (_timer > _timerMax)
        {
            gameObject.SetActive(false);
        }
    }
}
