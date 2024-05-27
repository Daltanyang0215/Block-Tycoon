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
    [field: SerializeField] public List<MissionCondition> MissionConditions { get; private set; }
}
[System.Serializable]
public class MissionCondition
{
    [field: SerializeField] public bool IsItem {  get; private set; }
    [field: SerializeField] public ItemPair ConditionPair { get; private set; }

    public bool CheckCondition()
    {
        bool result = true;

        if (IsItem)
        {
            // Ŭ���� ������ �������̸� �ش� �������� Ȯ�� ���ڸ��ٸ� false
            if (ConditionPair.ItemID != 0 &&
                MainGameManager.Instance.GetItemCount(ConditionPair.ItemID) < ConditionPair.Amount)
            {
                result = false;
            }
        }
        else
        {
            // Ŭ���� ������ �������� �ƴϸ� ������ ���̵��� ����� Ȯ�� ���ڸ��ٸ� false
            if (ConditionPair.ItemID != 0 &&
                HexaGridManager.Instance.GetHexaCount(ConditionPair.ItemID) < ConditionPair.Amount)
            {
                result = false;
            }
        }

        return result;
    }
}

