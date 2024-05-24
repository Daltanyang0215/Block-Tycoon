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
    [field: SerializeField] public ItemPair ConditionItem { get; private set; }

    public bool CheckCondition()
    {
        if (ConditionItem.ItemID == 0 ||
            MainGameManager.Instance.GetItemCount(ConditionItem.ItemID) > ConditionItem.Amount)
        {
            return true;
        }
        return false;
    }
}

