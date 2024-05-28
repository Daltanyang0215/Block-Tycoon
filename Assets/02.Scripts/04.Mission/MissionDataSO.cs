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
                    // Ŭ���� ������ �������̸� �ش� �������� Ȯ�� ���ڸ��ٸ� false
                    if (ConditionPair.ItemID != 0 &&
                        MainGameManager.Instance.GetItemCount(ConditionPair.ItemID) < ConditionPair.Amount)
                    {
                        result = false;
                    }
                    break;
                case MissionType.HasHexa:
                    // Ŭ���� ������ ����� �ش� ���̵��� ����� Ȯ�� ���ڸ��ٸ� false
                    if (ConditionPair.ItemID != 0 &&
                        HexaGridManager.Instance.GetHexaCount(ConditionPair.ItemID) < ConditionPair.Amount)
                    {
                        result = false;
                    }
                    break;
                case MissionType.UnlockHexa:
                    // Ŭ���� ������ ����̸� �ش� ���̵��� ����� �ȶ��Ǿ����� Ȯ��
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
