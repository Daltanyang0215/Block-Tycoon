using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
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
        if (data.MissionCondition.MissionType == MissionType.None)
            _missionName.StringReference.SetReference("Mission", _data.MissionInfo);
        else
            _missionName.StringReference.SetReference("Mission", _data.MissionCondition.MissionType.ToString());
        TitleNameUpdate();

        _moneyText.text = data.RewardMoney.ToString();

        Vector2 size = GetComponent<RectTransform>().sizeDelta;
        size.y = 75 * _data.MissionCondition.ConditionPairs.Count;
        size.y = size.y < 75f ? 75f : size.y;
        size.y += 100;
        GetComponent<RectTransform>().sizeDelta = size;

        #region ui 내 리스트 업데이트

        foreach (ItemPair pair in _data.MissionCondition.ConditionPairs)
        {
            ReportElement element = Instantiate(_priceElement, _materialTransfrom);

            switch (_data.MissionCondition.MissionType)
            {
                case MissionType.HasItem:
                case MissionType.SubItem:
                    ItemData item = MainGameDataSo.Instance.ItemDatas[pair.ItemID];
                    element.Init(item.ItemSprite, item.ItemName);
                    element.SetColor(item.ItemColor);
                    break;
                case MissionType.HasHexa:
                case MissionType.UnlockHexa:
                    HexaElementDataSO hexa = MainGameDataSo.Instance.HexaDatas[pair.ItemID];
                    element.Init(hexa.HexaIcon, hexa.name, "Hexa");
                    element.SetColor(hexa.BottomHexaColor);
                    break;
                default:
                    break;
            }

            element.UpdateItemCount(pair.Amount);
        }
        #endregion

        if (_data.ConditionID != 0) gameObject.SetActive(false);
        MissionUpdata();

        _receiptButton.onClick.AddListener(() =>
        {
            MainGameManager.Instance.MissionComplite[_data.MissionID] = true;
            MainGameManager.Instance.AddMoney(_data.RewardMoney);
            if (_data.MissionCondition.MissionType == MissionType.SubItem)
            {
                foreach (ItemPair pair in _data.MissionCondition.ConditionPairs)
                {
                    MainGameManager.Instance.AddItem(pair.ItemID, -pair.Amount);
                }
            }
            gameObject.SetActive(false);
            MissionUpdata();
        });
    }

    public void TitleNameUpdate()
    {
        string itemname = "";
        switch (_data.MissionCondition.MissionType)
        {
            case MissionType.HasItem:
            case MissionType.SubItem:
                itemname = MainUIManager.Instance.GetLocalString("Item",
                    MainGameDataSo.Instance.ItemDatas[_data.MissionCondition.ConditionPairs[0].ItemID].ItemName);
                break;
            case MissionType.HasHexa:
            case MissionType.UnlockHexa:
                itemname = MainUIManager.Instance.GetLocalString("Hexa",
                    MainGameDataSo.Instance.HexaDatas[_data.MissionCondition.ConditionPairs[0].ItemID].name);
                break;
            default:
                break;
        }
        (_missionName.StringReference["Item"] as StringVariable).Value = itemname;
        if (_data.MissionCondition.ConditionPairs.Count > 0)
            (_missionName.StringReference["ItemCount"] as IntVariable).Value = _data.MissionCondition.ConditionPairs[0].Amount;
        (_missionName.StringReference["IsLoop"] as BoolVariable).Value = _data.IsLoop;

    }

    public void MissionUpdata()
    {
        // 미션 목록이 꺼져있으면서
        // 선행 미션을 클리어 했는데
        // 미션을 클리어 하지 못했거나 반복 미션이면,
        // 목록을 활성화
        int conditionid = _data.ConditionID == -1 ? _data.MissionID - 1 : _data.ConditionID;
        if (!gameObject.activeSelf &&
            MainGameManager.Instance.MissionComplite.TryGetValue(conditionid, out bool check) &&
            check &&
            (!MainGameManager.Instance.MissionComplite[_data.MissionID] || _data.IsLoop))
        {
            gameObject.SetActive(true);
        }

        _receiptButton.interactable = _data.MissionCondition.CheckCondition();
    }
}
