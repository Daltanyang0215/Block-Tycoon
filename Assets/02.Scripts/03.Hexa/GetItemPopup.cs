using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetItemPopup : MonoBehaviour
{
    private float _disolveTimer;
    private SpriteRenderer _itemSprite;

    private void Awake()
    {
        _itemSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    public void Init(Sprite sprite)
    {
        _itemSprite.sprite = sprite;
        transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = sprite; ;
    }

    private void Update()
    {
        _disolveTimer += Time.deltaTime;

        transform.Translate(Vector2.up * 0.5f * Time.deltaTime);
        if (_disolveTimer > 1.15f)
        {
            _disolveTimer = 0;
            gameObject.SetActive(false);
        }
    }
}
