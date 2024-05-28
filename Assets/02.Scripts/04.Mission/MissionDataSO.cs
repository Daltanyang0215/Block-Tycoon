using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionDataSO", menuName = "BlockTycoon/MissionDataSO")]
public class MissionDataSO : ScriptableObject
{
    [field: SerializeField] public int MissionID { get; private set; }
    [field: SerializeField] public string MissionInfo { get; private set; }
    [field: SerializeField] public int RewardMoney { get; private set; }
    [field: SerializeField] public int ConditionID { get; private set; } = -1;
    [field: SerializeField] public bool IsLoop { get; private set; }
    [field: SerializeField] public MissionCondition MissionCondition { get; private set; }
}
[System.Serializable]
public class MissionCondition
{
    [field: SerializeField] public MissionType MissionType { get; private set; }
    [field: SerializeField] public List<ItemPair> ConditionPairs { get; private set; }

    public bool CheckCondition()
    {
        bool result = true;

        foreach (ItemPair ConditionPair in ConditionPairs)
        {
            switch (MissionType)
            {
                case MissionType.None:
                    break;
                case MissionType.HasItem:
                case MissionType.SubItem:
                    // 클리어 조건이 아이템이면 해당 아이템을 확인 모자르다면 false
                    if (ConditionPair.ItemID != 0 &&
                        MainGameManager.Instance.GetItemCount(ConditionPair.ItemID) < ConditionPair.Amount)
                    {
                        result = false;
                    }
                    break;
                case MissionType.HasHexa:
                    // 클리어 조건이 헥사라면 해당 아이디의 블록을 확인 모자르다면 false
                    if (ConditionPair.ItemID != 0 &&
                        HexaGridManager.Instance.GetHexaCount(ConditionPair.ItemID) < ConditionPair.Amount)
                    {
                        result = false;
                    }
                    break;
                case MissionType.UnlockHexa:
                    // 클리어 조건이 언락이면 해당 아이디의 블록이 안락되었는지 확인
                    if (ConditionPair.ItemID != 0 && !MainGameManager.Instance.UnlockList[ConditionPair.ItemID])
                    {
                        result = false;
                    }
                    break;
                default:
                    break;
            }
        }

        return result;
    }
}
public enum MissionType
{
    None,
    HasItem,
    SubItem,
    HasHexa,
    UnlockHexa,
}
