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
    [Tooltip("부스팅 최대 게이지 , 지속시간")]
    [SerializeField] private float _productBoosterMaxValue;
    public float ProductBoosterMaxValue => 
        _productBoosterMaxValue +
        (MainGameManager.Instance.Upgrades[0][0] > 0 ? HexaDatas[0].UpgradePairs[0].Prices[MainGameManager.Instance.Upgrades[0][0] - 1].Value : 0);
    [Tooltip("부스팅 1회 충전량")]
    [SerializeField] private float _productAddBoostingValue;
    public float ProductAddBoostingValue =>
        _productAddBoostingValue + 
        (MainGameManager.Instance.Upgrades[0][1] > 0 ? HexaDatas[0].UpgradePairs[1].Prices[MainGameManager.Instance.Upgrades[0][1] - 1].Value : 0);
    [Tooltip("부스팅 최대일시 속도 배율")]
    [SerializeField] private float _productBoosterMaxMut;
    public float ProductBoosterMaxMut =>
        _productBoosterMaxMut + 
        (MainGameManager.Instance.Upgrades[0][2]>0 ? HexaDatas[0].UpgradePairs[2].Prices[MainGameManager.Instance.Upgrades[0][2]-1].Value*0.01f:0);
    [Tooltip("부스팅 중 속도 배율")]
    [SerializeField] private float _productBoostingMut;
    public float ProductBoostingMut =>
        _productBoostingMut +
        (MainGameManager.Instance.Upgrades[0][3] > 0 ? HexaDatas[0].UpgradePairs[3].Prices[MainGameManager.Instance.Upgrades[0][3] - 1].Value*0.01f : 0);
   

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

    public List<ItemData> GetCanProductItemList(HexaElementDataSO hexaData)
    {
        if (hexaData == null) return null;

        List<ItemData> result = new List<ItemData>();

        foreach (ItemData item in ItemDatas.Values)
        {
            if (hexaData.HexaType == item.ProduceRecipe.CanProduceHexa &&
                hexaData.HexaTier >= item.ProduceRecipe.CanProduceHexaTiar)
            {
                result.Add(item);
            }
        }
        return result;
    }

}
[System.Serializable]
public class ItemData
{
    [field: SerializeField] public string ItemName { get; private set; }
    [field: SerializeField] public int ItemID { get; private set; }
    [field: SerializeField] public int ItemPrice { get; private set; }
    [field: SerializeField] public Sprite ItemSprite { get; private set; }
    [field: SerializeField] public Color ItemColor { get; private set; }
    [field: SerializeField] public ProduceRecipe ProduceRecipe { get; private set; }
}