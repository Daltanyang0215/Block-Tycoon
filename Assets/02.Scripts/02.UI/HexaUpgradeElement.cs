using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class HexaUpgradeElement : MonoBehaviour
{
    private HexaElementDataSO _data;

    [SerializeField] private Image _hexaImage;
    [SerializeField] private LocalizeStringEvent _hexaName;
    [SerializeField] private Transform _unlockTransfrom;
    [SerializeField] private Transform _priceTransfrom;
    [SerializeField] private Transform _upgradeTransfrom;
    [SerializeField] private Button _unlockButton;


    [SerializeField] private ReportElement _priceElement;
    [SerializeField] private UpgradeElement _upgradeElement;
    private List<UpgradeElement> _upgradeList = new List<UpgradeElement>();

    public void Init(HexaElementDataSO data)
    {
        _data = data;

        _hexaImage.sprite = _data.HexaIcon;
        _hexaName.StringReference.SetReference("Hexa", _data.name);

        if (MainGameManager.Instance.UnlockList[_data.GetID] || _data.UnlockPrice.Count == 0)
            UnlockHexa();
        else
        {
            Vector2 size = GetComponent<RectTransform>().sizeDelta;
            size.y = 75 * _data.UnlockPrice.Count;
            size.y = size.y < 150f ? 150 : size.y;
            GetComponent<RectTransform>().sizeDelta = size;
            foreach (ItemPair pair in _data.UnlockPrice)
            {
                ReportElement element = Instantiate(_priceElement, _priceTransfrom);
                ItemData item = MainGameDataSo.Instance.ItemDatas[pair.ItemID];
                element.Init(item.ItemSprite, item.ItemName);
                element.SetColor(item.ItemColor);
                element.UpdateItemCount(pair.Amount);
                _unlockButton.interactable = false;
            }

            _unlockButton.onClick.AddListener(() =>
            {
                foreach (ItemPair pair in _data.UnlockPrice)
                {
                    MainGameManager.Instance.AddItem(pair.ItemID, -pair.Amount);
                }
            });
        }

    }
    public void UpgradeUpdata()
    {
        bool canPrice = true;

        if (!MainGameManager.Instance.UnlockList[_data.GetID])
        {
            foreach (ItemPair pair in _data.UnlockPrice)
            {
                if (MainGameManager.Instance.GetItemCount(pair.ItemID) < pair.Amount)
                {
                    canPrice = false;
                }
            }
            _unlockButton.interactable = canPrice;
            return;
        }
        else
        {
            _unlockTransfrom.gameObject.SetActive(false);
        }

        foreach (UpgradeElement element in _upgradeList)
        {
            element.ElementUpdate();
        }
    }
    public void UnlockHexa()
    {
        _unlockTransfrom.gameObject.SetActive(false);
        MainGameManager.Instance.UnlockList[_data.GetID] = true;

        UpgradeListUpdata();
    }
    private void UpgradeListUpdata()
    {
        Vector2 size = GetComponent<RectTransform>().sizeDelta;
        size.y = 150 * _data.UpgradePairs.Count;
        size.y = size.y == 0 ? 150 : size.y;

        GetComponent<RectTransform>().sizeDelta = size;
        for (int i = 0; i < _data.UpgradePairs.Count; i++)
        {
            UpgradeElement element = Instantiate(_upgradeElement, _upgradeTransfrom);
            element.Init(_data.GetID, i, _data.UpgradePairs[i]);
            _upgradeList.Add(element);
        }
    }

}
