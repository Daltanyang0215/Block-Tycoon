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
    [SerializeField] private Button _priceButton;

    [SerializeField] private ReportElement _priceElement;

    public void Init(HexaElementDataSO data)
    {
        _data = data;

        _hexaImage.sprite = _data.HexaIcon;
        _hexaName.StringReference.SetReference("Hexa", _data.name);
        Vector2 size = GetComponent<RectTransform>().sizeDelta;
        size.y = 75 * _data.BuyPrice.Count;
        size.y = size.y < 150f ? 150 : size.y;
        GetComponent<RectTransform>().sizeDelta = size;
        foreach (ItemPair pair in _data.BuyPrice)
        {
            ReportElement element = Instantiate(_priceElement, _priceTransfrom);
            ItemData item = MainGameDataSo.Instance.ItemDatas[pair.ItemID];
            element.Init(item.ItemSprite, item.ItemName);
            element.UpdateItemCount(pair.Amount);
            _priceButton.interactable = false;
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

        foreach (ItemPair pair in _data.BuyPrice)
        {
            if (MainGameManager.Instance.GetItemCount(pair.ItemID) < pair.Amount)
            {
                canPrice = false;
            }
        }
        _priceButton.interactable = canPrice;
    }
}
