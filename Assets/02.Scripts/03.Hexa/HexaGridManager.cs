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

    [SerializeField] private HexaGridElement _elementPrefab;
    private Dictionary<Vector3Int, HexaGridElement> _gridPositions;


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
        _gridPositions = new Dictionary<Vector3Int, HexaGridElement>();
        _itemsPopups = new List<GetItemPopup>();
        for (int i = 0; i < 15; i++)
        {
            _itemsPopups.Add(Instantiate(_itemPopupPrefab, transform.GetChild(0)));
            _itemsPopups[i].gameObject.SetActive(false);
        }
    }

    [ContextMenu("TestFunc")]
    public void TestFunc()
    {
        foreach (HexaGridElement element in transform.GetComponentsInChildren<HexaGridElement>())
        {
            _gridPositions.TryAdd(_grid.WorldToCell(element.transform.position), element);
        }
    }

    public void ShowAddItemPopup(Vector2 showPos, ItemType type)
    {
        GetItemPopup itemPopup = null;

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
            itemPopup.Init(MainGameDataSo.Instance.GetItemSprite(type));
            _itemsPopups.Add(itemPopup);
            return;
        }
        itemPopup.transform.position = showPos;
        itemPopup.gameObject.SetActive(true);
        itemPopup.Init(MainGameDataSo.Instance.GetItemSprite(type));
    }

    public Vector2 GetGridePos(HexaGridElement element, Vector3 mousePos, Vector3 befoPos, ref HexaGridElement[] nearHexa)
    {
        Vector2 posPos = _camera.ScreenToWorldPoint(Input.mousePosition);
        if (posPos.y < -4.7f) posPos.y = -4.7f;
        if (posPos.y > 4.7) posPos.y = 4.7f;
        if (posPos.x > 8.4f) posPos.x = 8.4f;
        if (posPos.x < -8.4f) posPos.x = -8.4f;

        Vector3Int cellPos = _grid.WorldToCell(posPos);
        cellPos.z = 0;
        if (_gridPositions.ContainsKey(cellPos) && !ReferenceEquals(_gridPositions[cellPos], null))
        {
            return befoPos;
        }

        _gridPositions[_grid.WorldToCell(befoPos)] = null;
        _gridPositions[cellPos] = element;

        List<Vector3Int> posList = cellPos.y % 2 == 0 ? _evenPos : _oddPos;
        for (int i = 0; i < posList.Count; i++)
        {
            _gridPositions.TryGetValue(cellPos + posList[i], out nearHexa[i]);
        }

        return _grid.CellToWorld(cellPos);
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
