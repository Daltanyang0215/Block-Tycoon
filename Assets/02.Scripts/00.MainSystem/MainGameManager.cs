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

    [field:SerializeField] public MainGameDataSo GameDataSo { get; private set; }

    private Dictionary<int, int> _hasItems;
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
        _hasItems = new Dictionary<int, int>();
        foreach (ItemData data in MainGameDataSo.Instance.ItemDatas.Values)
        {
            _hasItems.Add(data.ItemID, 0);
        }
    }

    public void OnGameExit()
    {
        Application.Quit();
    }
}