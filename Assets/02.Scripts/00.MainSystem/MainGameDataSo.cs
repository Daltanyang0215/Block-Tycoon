using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "MainGameDataSo", menuName = "BlockTycoon/MainGameDataSo")]
public class MainGameDataSo : ScriptableObject
{
    #region Singleton

    private static MainGameDataSo _instance;
    public static MainGameDataSo Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("MainGameManager").GetComponent<MainGameManager>().GameDataSo;
            }
            return _instance;
        }
    }
    #endregion

    [field: Header("HexaData")]
    [field: SerializeField] public List<HexaElementDataSO> HexaDatas { get; private set; }

    [field: SerializeField] public int MatarialStorageCountMut { get; private set; }
    [field: SerializeField] public int ProductStorageCountMut { get; private set; }
    [Header("ItmeData")]
    [SerializeField] private List<ItemData> _itemDatas;
    public Dictionary<int, ItemData> ItemDatas { get; private set; }

    [Header("MissionData")]
    [SerializeField] private List<MissionDataSO> _missions;
    public Dictionary<int, MissionDataSO> MissionDatas { get; private set; }


    [field: Header("Other")]
    [field: SerializeField] public float MoveItemEffectTime { get; private set; }
    [field: SerializeField] public Sprite MoneyImage { get; private set; }
    [field: SerializeField] public Sprite HexaLoadImage { get; private set; }
    [field: SerializeField] public Sprite ProcessTimerImage { get; private set; }
    [field: SerializeField] public Sprite ProcessClickImage { get; private set; }

    public void Init()
    {
        ItemDatas = new Dictionary<int, ItemData>();
        foreach (var itemData in _itemDatas)
        {
            ItemDatas.Add(itemData.ItemID, itemData);
        }

        MissionDatas = new Dictionary<int, MissionDataSO>();
        foreach (var missionData in _missions)
        {
            MissionDatas.TryAdd(missionData.MissionID, missionData);
        }
    }


}
[System.Serializable]
public class ItemData
{
    [field: SerializeField] public string ItemName { get; private set; }
    [field: SerializeField] public int ItemID { get; private set; }
    [field: SerializeField] public int ItemPrice {  get; private set; }
    [field: SerializeField] public Sprite ItemSprite { get; private set; }
    [field: SerializeField] public Color ItemColor { get; private set; }

}