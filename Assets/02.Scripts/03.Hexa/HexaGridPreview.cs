using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexaGridPreview : MonoBehaviour
{
    private HexaGridManager _manager;
    private Transform _canBuild;
    private Transform _canNotBuild;

    private HexaElementDataSO _data;

    public void Init(HexaElementDataSO dataSO) => _data = dataSO;

    private void Awake()
    {
        _manager = HexaGridManager.Instance;
        _canBuild = transform.GetChild(0);
        _canNotBuild = transform.GetChild(1);
    }

    private void Update()
    {
        transform.position = _manager.MousePosToGridWorldPos(Input.mousePosition);

        bool isPlace = _manager.CheckPlaceGrid(_manager.MousePosToGridPos(Input.mousePosition));
        _canBuild.gameObject.SetActive(!isPlace);
        _canNotBuild.gameObject.SetActive(isPlace);

        if (Input.GetMouseButtonDown(0) && !isPlace)
        {
            _manager.SpawnHexaGird(_data, _manager.MousePosToGridPos(Input.mousePosition));
            foreach (ItemPair pair in _data.BuyPrice)
            {
                MainGameManager.Instance.AddItem(pair.ItemID, -pair.Amount);
            }
            gameObject.SetActive(false);
        }
        if (Input.GetMouseButtonDown(1))
        {
            gameObject.SetActive(false);
        }
    }
}
