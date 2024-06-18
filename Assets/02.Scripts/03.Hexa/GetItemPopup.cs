using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GetItemPopup : MonoBehaviour
{
    private float _disolveTimer;
    [SerializeField] private TMP_Text _priceText;


    public void Init(int price)
    {
        _priceText.text = price.ToString();
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
