using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexaGridManager : MonoBehaviour
{
    #region Singleton

    private static HexaGridManager _instance;
    public static HexaGridManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("HexaGridManager").GetComponent<HexaGridManager>();
            }
            return _instance;
        }
    }
    #endregion

    private Camera _camera;
    private Grid _grid;

    [field: SerializeField] public HexaGridPreview GridPreview { get; private set; }

    [SerializeField] private GameObject _elementPrefab;
    private Dictionary<Vector3Int, IHexaGridElement> _gridPositions;
    public bool CheckPlaceGrid(Vector3Int pos) => _gridPositions.ContainsKey(pos) && !ReferenceEquals(_gridPositions[pos], null);

    private List<GetItemPopup> _itemsPopups;
    [SerializeField] private GetItemPopup _itemPopupPrefab;

    private readonly List<Vector3Int> _oddPos = new List<Vector3Int>
    {
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(1, 1, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(1, -1, 0),
        new Vector3Int(0, -1, 0)
    };
    private readonly List<Vector3Int> _evenPos = new List<Vector3Int>
    {
        new Vector3Int(-1, 0, 0),
        new Vector3Int(-1 , 1, 0),
        new Vector3Int(0 , 1, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int( 0 , -1, 0),
        new Vector3Int( -1, -1, 0)
    };

    private void Start()
    {
        _camera = Camera.main;
        _grid = GetComponent<Grid>();
        _gridPositions = new Dictionary<Vector3Int, IHexaGridElement>();
        _itemsPopups = new List<GetItemPopup>();
        for (int i = 0; i < 15; i++)
        {
            _itemsPopups.Add(Instantiate(_itemPopupPrefab, transform.GetChild(0)));
            _itemsPopups[i].gameObject.SetActive(false);
        }
        for (int i = 0;i < 6; i++)
        {
            SpawnHexaGird(MainGameDataSo.Instance.HexaDatas[i], Vector3Int.right * i);
        }
    }

    private void Update()
    {
        // 생산블록의 생산 시작(소비 먼저)
        foreach (IHexaGridElement hexa in _gridPositions.Values)
        {
            if (ReferenceEquals(hexa, null)) continue;
            if (hexa is HexaGridProduct)
                hexa.HexaUpdate();
        }
        // 운송블록의 재료이동
        foreach (IHexaGridElement hexa in _gridPositions.Values)
        {
            if (ReferenceEquals(hexa, null)) continue;
            if (hexa is HexaGridTransit)
                hexa.HexaUpdate();
        }
        // 생산블록의 재료 이동(소비 이후 재료 이동)
        foreach (IHexaGridElement hexa in _gridPositions.Values)
        {
            if (ReferenceEquals(hexa, null)) continue;
            if (hexa is HexaGridProduct grid)
                grid.GetMaterialToNear();
        }
        // 창고블록의 재료 이동(생산 블록 종료 후 나머지 창고 이동)
        foreach (IHexaGridElement hexa in _gridPositions.Values)
        {
            if (ReferenceEquals(hexa, null)) continue;
            if (hexa is HexaGridStorage)
                hexa.HexaUpdate();
        }
    }

    public void SpawnHexaGird(HexaElementDataSO data, Vector3Int pos)
    {
        GameObject grid = Instantiate(_elementPrefab, _grid.CellToWorld(pos), Quaternion.identity, transform).gameObject;

        switch (data.HexaType)
        {
            case HexaType.Storge:
                grid.AddComponent<HexaGridStorage>().Init(data);
                break;
            case HexaType.Move:
                grid.AddComponent<HexaGridTransit>().Init(data);
                break;
            default:
                grid.AddComponent<HexaGridProduct>().Init(data);
                break;
        }
        _gridPositions.Add(pos, grid.GetComponent<IHexaGridElement>());

        List<Vector3Int> posList = pos.y % 2 == 0 ? _evenPos : _oddPos;
        for (int i = 0; i < posList.Count; i++)
        {
            HexaNearUpData(pos + posList[i]);
        }
        HexaNearUpData(pos);
    }

    public void DestoryHexaGrid(GameObject hexa)
    {
        Vector3Int pos = _grid.WorldToCell(hexa.transform.position);

        _gridPositions.Remove(_grid.WorldToCell(pos));
        HexaNearRemove(_grid.WorldToCell(pos), hexa.GetComponent<IHexaGridElement>());

        List<Vector3Int> posList = pos.y % 2 == 0 ? _evenPos : _oddPos;
        for (int i = 0; i < posList.Count; i++)
        {
            HexaNearUpData(pos + posList[i]);
        }
        Destroy(hexa);
    }

    public void ShowAddItemPopup(Vector2 showPos, int itemid)
    {
        GetItemPopup itemPopup = null;
        ItemData itemData = MainGameDataSo.Instance.ItemDatas[itemid];

        foreach (var popup in _itemsPopups)
        {
            if (!popup.gameObject.activeSelf)
            {
                itemPopup = popup;
                break;
            }
        }

        if (itemPopup == null)
        {
            itemPopup = Instantiate(_itemPopupPrefab, showPos, Quaternion.identity, transform.GetChild(0));
            itemPopup.Init(itemData.ItemSprite);
            _itemsPopups.Add(itemPopup);
            return;
        }
        itemPopup.transform.position = showPos;
        itemPopup.gameObject.SetActive(true);
        itemPopup.Init(itemData.ItemSprite);
    }

    public Vector2 GetGridePos(IHexaGridElement element, Vector3 mousePos, Vector3 befoPos)
    {
        Vector3Int cellPos = MousePosToGridPos(mousePos);
        if (_gridPositions.ContainsKey(cellPos) && !ReferenceEquals(_gridPositions[cellPos], null))
        {
            return befoPos;
        }

        // 이전 자리의 주변 블록 재설정
        //_gridPositions[_grid.WorldToCell(befoPos)] = null;
        _gridPositions.Remove(_grid.WorldToCell(befoPos));
        HexaNearRemove(_grid.WorldToCell(befoPos), element);
        // 다음 자리의 주변 블록 재설정
        _gridPositions[cellPos] = element;

        List<Vector3Int> posList = cellPos.y % 2 == 0 ? _evenPos : _oddPos;
        for (int i = 0; i < posList.Count; i++)
        {
            HexaNearUpData(cellPos + posList[i]);
        }
        HexaNearUpData(cellPos);

        return _grid.CellToWorld(cellPos);
    }

    public Vector3Int MousePosToGridPos(Vector3 mousePos)
    {
        Vector2 posPos = _camera.ScreenToWorldPoint(Input.mousePosition);
        if (posPos.y < -4.7f) posPos.y = -4.7f;
        if (posPos.y > 4.7) posPos.y = 4.7f;
        if (posPos.x > 8.4f) posPos.x = 8.4f;
        if (posPos.x < -8.4f) posPos.x = -8.4f;
        Vector3Int result = _grid.WorldToCell(posPos);
        result.z = 0;
        return result;
    }

    public Vector2 MousePosToGridWorldPos(Vector3 mousePos)
    {
        return _grid.CellToWorld(MousePosToGridPos(mousePos));
    }

    private void HexaNearRemove(Vector3Int center, IHexaGridElement remove)
    {
        List<Vector3Int> posList = center.y % 2 == 0 ? _evenPos : _oddPos;
        for (int i = 0; i < posList.Count; i++)
        {
            if (_gridPositions.TryGetValue(center + posList[i], out IHexaGridElement near) && !ReferenceEquals(near, null))
                near.RemoveNearHexa(remove);
        }
    }

    private void HexaNearUpData(Vector3Int center)
    {
        // 딕셔너리에 없거나, 해당 좌표가 null 이면 리턴
        if (!_gridPositions.ContainsKey(center) || ReferenceEquals(_gridPositions[center], null)) return;

        List<Vector3Int> posList = center.y % 2 == 0 ? _evenPos : _oddPos;
        IHexaGridElement[] near = new IHexaGridElement[posList.Count];
        for (int i = 0; i < posList.Count; i++)
        {
            _gridPositions.TryGetValue(center + posList[i], out near[i]);
        }
        _gridPositions[center].SetNearHexa(near);
    }


    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.yellow;

    //    Vector2 posPos = _camera.ScreenToWorldPoint(Input.mousePosition);
    //    if (posPos.y < -4.7f) posPos.y = -4.7f;
    //    if (posPos.y > 4.7) posPos.y = 4.7f;
    //    if (posPos.x > 8.4f) posPos.x = 8.4f;
    //    if (posPos.x < -8.4f) posPos.x = -8.4f;

    //    Vector3Int cellPos = _grid.WorldToCell(posPos);
    //    cellPos.z = 0;

    //    List<Vector3Int> pointList = cellPos.y % 2 == 0 ? _evenPos : _oddPos;

    //    Gizmos.DrawLine(_grid.CellToWorld(cellPos + pointList[0]), _grid.CellToWorld(cellPos + pointList[1]));
    //    Gizmos.DrawLine(_grid.CellToWorld(cellPos + pointList[1]), _grid.CellToWorld(cellPos + pointList[2]));
    //    Gizmos.DrawLine(_grid.CellToWorld(cellPos + pointList[2]), _grid.CellToWorld(cellPos + pointList[3]));
    //    Gizmos.DrawLine(_grid.CellToWorld(cellPos + pointList[3]), _grid.CellToWorld(cellPos + pointList[4]));
    //    Gizmos.DrawLine(_grid.CellToWorld(cellPos + pointList[4]), _grid.CellToWorld(cellPos + pointList[5]));
    //    Gizmos.DrawLine(_grid.CellToWorld(cellPos + pointList[5]), _grid.CellToWorld(cellPos + pointList[0]));
    //}
}
