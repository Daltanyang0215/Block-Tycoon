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

    private Dictionary<ItemType, int> _hasItems;
    public int GetItemCount(ItemType type) => _hasItems[type];
    public void AddItem(ItemType type, int addValue)
    {
        _hasItems[type] += addValue;
        UIUpdateAction?.Invoke();
    }

    public System.Action UIUpdateAction;

    private void Start()
    {
        _hasItems = new Dictionary<ItemType, int>();
        foreach (ItemType type in System.Enum.GetValues(typeof(ItemType)))
        {
            _hasItems.Add(type, 0);
        }
    }

    public void OnGameExit()
    {
        Application.Quit();
    }
}