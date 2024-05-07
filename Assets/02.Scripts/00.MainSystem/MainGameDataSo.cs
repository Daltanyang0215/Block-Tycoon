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

    [SerializeField] private List<Sprite> _itemSprites = new List<Sprite>();
    public Sprite GetItemSprite(ItemType type) => _itemSprites[(int)type];
}