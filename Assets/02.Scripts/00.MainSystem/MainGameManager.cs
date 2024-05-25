using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    #region Singleton
    private static MainGameManager _instance;
    public static MainGameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("MainGameManager").GetComponent<MainGameManager>();
            }
            return _instance;
        }
    }
    #endregion

    [field: SerializeField] public MainGameDataSo GameDataSo { get; private set; }

    private readonly WaitForSeconds _autoSaveTime = new WaitForSeconds(300);

    #region Money
    [field: SerializeField] public int HasMoney { get; private set; }
    public void AddMoney(int money)
    {
        HasMoney += money;
        UIUpdateAction?.Invoke();
    }

    #endregion

    #region Item
    private Dictionary<int, int> _hasItems;
    public int GetItemCount(int type) => _hasItems[type];
    public void AddItem(int itemid, int addValue)
    {
        _hasItems[itemid] += addValue;
        UIUpdateAction?.Invoke();
    }
    #endregion

    public Dictionary<int, bool> MissionComplite { get; private set; }
    public Dictionary<int, bool> UnlockList { get; private set; }

    public Dictionary<int, List<int>> Upgrades { get; private set; }

    public System.Action UIUpdateAction;

    private void Awake()
    {
        GameDataSo.Init();

    }

    private void Start()
    {
        LoadData();
        MainUIManager.Instance.StartInit();
    }

    private void LoadData()
    {
        SaveData saveData = SaveSystem.Save;
        HasMoney = saveData.HasMoney;

        _hasItems = new Dictionary<int, int>();
        for (int i = 0; i < saveData.HasItemCode.Count; i++)
        {
            _hasItems.Add(saveData.HasItemCode[i], saveData.HasItemCount[i]);
        }

        UnlockList = new Dictionary<int, bool>();
        if (saveData.UnlockList?.Count == 0)
        {
            for (int i = 0; i < MainGameDataSo.Instance.HexaDatas.Count; i++)
            {
                UnlockList.Add(i, false);
            }
        }
        else
        {
            foreach (IntBoolPair data in saveData.UnlockList)
            {
                UnlockList.Add(data.index, data.value);
            }
        }
        MissionComplite = new Dictionary<int, bool>();
        if (saveData.MissionComplite?.Count == 0)
        {
            foreach (MissionDataSO data in MainGameDataSo.Instance.MissionDatas.Values)
            {
                MissionComplite.Add(data.MissionID, false);
            }
        }
        else
        {
            foreach (IntBoolPair data in saveData.MissionComplite)
            {
                MissionComplite.Add(data.index, data.value);
            }
        }

        Upgrades = new Dictionary<int, List<int>> ();
        if (saveData.HexaUpgradeData?.Count == 0)
        {
        
            foreach (HexaElementDataSO data in MainGameDataSo.Instance.HexaDatas)
            {
                List<int> add = new List<int>();
                for (int i = 0; i < data.UpgradePairs.Count; i++)
                {
                    add.Add(0);
                }
                Upgrades.Add(data.GetID, add);
            }
        }
        else
        {
            foreach (hexaUpgradeSaveData data in saveData.HexaUpgradeData)
            {
                Upgrades.Add(data.hexaindex, data.level);
            }
        }

    }

    private IEnumerator AutoSave()
    {
        while (true)
        {
            yield return _autoSaveTime;
            SaveData();
        }
    }



    [ContextMenu("DeleteData")]
    public void DeleteData() => SaveSystem.DeleteData();
    [ContextMenu("SaveData")]
    public void SaveData() => SaveSystem.SaveData();

    public void OnGameExit()
    {
        Application.Quit();
    }

    public void OnApplicationQuit()
    {
        SaveSystem.SaveData();
    }
}