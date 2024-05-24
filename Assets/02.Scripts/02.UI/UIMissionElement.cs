using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class UIMissionElement : MonoBehaviour
{
    private MissionDataSO _data;

    [SerializeField] private LocalizeStringEvent _missionName;
    [SerializeField] private TMP_Text _moneyText;
    [SerializeField] private Transform _materialTransfrom;
    [SerializeField] private Button _receiptButton;

    [SerializeField] private ReportElement _priceElement;

    public void Init(MissionDataSO data)
    {
        _data = data;
        _missionName.StringReference.SetReference("Mission", _data.name);
        _moneyText.text = data.RewardMoney.ToString();

        Vector2 size = GetComponent<RectTransform>().sizeDelta;
        size.y = 75 * _data.MissionConditions.Count;
        size.y = size.y < 75f ? 75f : size.y;
        size.y += 100;
        GetComponent<RectTransform>().sizeDelta = size;

        foreach (MissionCondition pair in _data.MissionConditions)
        {
            ReportElement element = Instantiate(_priceElement, _materialTransfrom);
            ItemData item = MainGameDataSo.Instance.ItemDatas[pair.ConditionItem.ItemID];
            element.Init(item.ItemSprite, item.ItemName);
            element.SetColor(item.ItemColor);
            element.UpdateItemCount(pair.ConditionItem.Amount);
        }
        if (_data.ConditionID != -1) gameObject.SetActive(false);
        MissionUpdata();

        _receiptButton.onClick.AddListener(() =>
        {
            MainGameManager.Instance.MissionComplite[_data.MissionID] = true;
            MainGameManager.Instance.AddMoney(_data.RewardMoney);
            foreach (MissionCondition condition in _data.MissionConditions)
            {
                MainGameManager.Instance.AddItem(condition.ConditionItem.ItemID, -condition.ConditionItem.Amount);
            }
            gameObject.SetActive(false);
        });
    }

    public void MissionUpdata()
    {
        if (!gameObject.activeSelf &&
            !MainGameManager.Instance.MissionComplite[_data.MissionID] &&
            MainGameManager.Instance.MissionComplite.TryGetValue(_data.ConditionID, out bool check) &&
            check)
        {
            gameObject.SetActive(true);
        }

        bool canReceipt = true;

        foreach (MissionCondition condition in _data.MissionConditions)
        {
            if (!condition.CheckCondition())
            {
                canReceipt = false;
            }
        }

        _receiptButton.interactable = canReceipt;
    }
}
