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

    private Dictionary<int, int> _hasItems;
    public List<bool> UnlockList { get; private set; }
    public int GetItemCount(int type) => _hasItems[type];
    public void AddItem(int itemid, int addValue)
    {
        _hasItems[itemid] += addValue;
        UIUpdateAction?.Invoke();
    }

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
        _hasItems = new Dictionary<int, int>();
        for (int i = 0; i < saveData.HasItemCode.Count; i++)
        {
            _hasItems.Add(saveData.HasItemCode[i], saveData.HasItemCount[i]);
        }

        if(saveData.UnlockList?.Count == 0)
        {
            UnlockList = new List<bool>();
            for (int i = 0; i < MainGameDataSo.Instance.HexaDatas.Count; i++)
            {
                UnlockList.Add(false);
            }
        }
        else
        {
            UnlockList = saveData.UnlockList;
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