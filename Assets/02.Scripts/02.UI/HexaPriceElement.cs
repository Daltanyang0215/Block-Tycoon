using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class HexaPriceElement : MonoBehaviour
{
    private HexaElementDataSO _data;

    [SerializeField] private Image _hexaImage;
    [SerializeField] private LocalizeStringEvent _hexaName;
    [SerializeField] private Transform _priceTransfrom;
    [SerializeField] private Transform _unlockTransfrom;
    [SerializeField] private Button _priceButton;
    [SerializeField] private Button _unlockButton;

    [SerializeField] private ReportElement _priceElement;

    public void Init(HexaElementDataSO data)
    {
        _data = data;

        _hexaImage.sprite = _data.HexaIcon;
        _hexaName.StringReference.SetReference("Hexa", _data.name);

        if (MainGameManager.Instance.UnlockList[_data.GetID] || _data.UnlockPrice == 0)
            UnlockHexa();
        else
        {
            //Vector2 size = GetComponent<RectTransform>().sizeDelta;
            //size.y = 75 * _data.UnlockPrice.Count;
            //size.y = size.y < 150f ? 150 : size.y;
            //GetComponent<RectTransform>().sizeDelta = size;
            //foreach (ItemPair pair in _data.UnlockPrice)
            {
                //ReportElement element = Instantiate(_priceElement, _priceTransfrom);
                //ItemData item = MainGameDataSo.Instance.ItemDatas[pair.ItemID];
                //element.Init(item.ItemSprite, item.ItemName);
                //element.SetColor(item.ItemColor);
                //element.UpdateItemCount(pair.Amount);
                //_unlockButton.interactable = false;

                ReportElement element = Instantiate(_priceElement, _priceTransfrom);
                //ItemData item = MainGameDataSo.Instance.ItemDatas[pair.ItemID];
                element.Init(MainGameDataSo.Instance.MoneyImage, "Money");
                //element.SetColor(item.ItemColor);
                element.UpdateItemCount(data.UnlockPrice);
                _unlockButton.interactable = false;
            }

            _unlockButton.onClick.AddListener(() =>
            {
                MainGameManager.Instance.AddMoney(-data.UnlockPrice);
            });
        }

        _priceButton.onClick.AddListener(() =>
        {
            MainUIManager.Instance.ShowReportAnimation(false);
        });
        _priceButton.onClick.AddListener(() =>
        {
            HexaGridManager.Instance.GridPreview.Init(_data);
            HexaGridManager.Instance.GridPreview.gameObject.SetActive(true);
        });
    }

    public void PriceUpdata()
    {
        bool canPrice = true;

        if (!MainGameManager.Instance.UnlockList[_data.GetID])
        {
                if (MainGameManager.Instance.HasMoney < _data.UnlockPrice)
                {
                    canPrice = false;
                }
            _unlockButton.interactable = canPrice;
            return;
        }
        else if (_unlockTransfrom.gameObject.activeSelf)
        {
            UnlockHexa();
        }


        //foreach (ItemPair pair in _data.BuyPrice)
        //{
        //    if (MainGameManager.Instance.GetItemCount(pair.ItemID) < pair.Amount)
        //    {
        //        canPrice = false;
        //    }
        //}
        if (MainGameManager.Instance.HasMoney < _data.BuyPrice)
        {
            canPrice = false;
        }
        _priceButton.interactable = canPrice;
    }

    public void UnlockHexa()
    {
        _unlockTransfrom.gameObject.SetActive(false);
        MainGameManager.Instance.UnlockList[_data.GetID] = true;

        for (int i = _priceTransfrom.childCount - 1; i >= 0; i--)
        {
            Destroy(_priceTransfrom.GetChild(0).gameObject);
        }
        PriceListUpdate();
    }


    private void PriceListUpdate()
    {
        //Vector2 size = GetComponent<RectTransform>().sizeDelta;
        //size.y = 75 * _data.BuyPrice.Count;
        //size.y = size.y < 150f ? 150 : size.y;
        //GetComponent<RectTransform>().sizeDelta = size;
        //foreach (ItemPair pair in _data.BuyPrice)
        {
            ReportElement element = Instantiate(_priceElement, _priceTransfrom);
            //ItemData item = MainGameDataSo.Instance.ItemDatas[0];
            //element.Init(item.ItemSprite, item.ItemName);
            element.Init(MainGameDataSo.Instance.MoneyImage, "Money");
            //element.SetColor(item.ItemColor);
            //element.UpdateItemCount(pair.Amount);
            element.UpdateItemCount(_data.BuyPrice);
            _priceButton.interactable = false;
        }
    }

}
