using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexaGridElement : MonoBehaviour
{
    [SerializeField] private HexaElementDataSO _data;
    private HexaGridManager _manger;
    private Material _fillMaterial;

    private float _filltimer = 1;
    private float _fillAmount;
    private float _fillMut;

    public void Init(HexaElementDataSO newData)
    {
        _data = newData;

        _fillMut = _data.ProducTime == 0 ? 0 : 1 / _data.ProducTime;

        transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_TopColor", _data.TopHexaColor);
        transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_MiddleColor", _data.BottomHexaColor);
        transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetColor("_TopColor", _data.TopGaugeColor);
        transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetColor("_MiddleColor", _data.BottomGaugeColor);
        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = _data.HexaIcon;
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = _data.HexaIcon;
    }

    private void Start()
    {
        _manger = HexaGridManager.Instance;
        _fillMaterial = transform.Find("Gauge").GetComponent<SpriteRenderer>().materials[0];

        // TODO 나중에 지워야됨
        {
            _fillMut = _data.ProducTime == 0 ? 0 : 1 / _data.ProducTime;

            transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_TopColor", _data.TopHexaColor);
            transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_MiddleColor", _data.BottomHexaColor);
            transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetColor("_TopColor", _data.TopGaugeColor);
            transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetColor("_MiddleColor", _data.BottomGaugeColor);
            transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = _data.HexaIcon;
            transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = _data.HexaIcon;
        }
    }

    private void Update()
    {
        _fillAmount += Time.deltaTime * _fillMut;
        if (_fillAmount > _filltimer)
        {
            _fillAmount -= _filltimer;
            for (int i = 0; i < _data.itemPairs.Count; i++)
            {
                ProducItemPair item = _data.itemPairs[i];
                _manger.ShowAddItemPopup(transform.position + (i * 0.35f) * Vector3.up + 0.15f * Vector3.up, item.ProducItemType);
                MainGameManager.Instance.AddItem(item.ProducItemType, item.ProducAmount);
            }
        }
        _fillMaterial.SetFloat("_CutRange", _fillAmount / _filltimer);
    }

    private void OnMouseDown()
    {
        _fillAmount += _data.ProducClick;
    }

    private void OnMouseDrag()
    {
        transform.position = _manger.GetGridePos(this, Input.mousePosition, transform.position);
    }


}
