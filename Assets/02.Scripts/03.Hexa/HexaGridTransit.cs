using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexaGridTransit : MonoBehaviour, IHexaGridElement, IHexaGridInItem
{
    [field: SerializeField] public HexaElementDataSO Data { get; private set; }
    public Vector2 Pos => transform.position;
    private HexaGridManager _manger;
    private IHexaGridElement[] _nearHexa = new IHexaGridElement[6];

    public ProduceRecipe CurRecipe { get; private set; } = null;
    public Dictionary<int, int> MaterialItemCount { get; private set; } = new Dictionary<int, int>();
    public Dictionary<int, int> ProductItemCount { get; private set; } = new Dictionary<int, int>();
    public bool CanGetMaterial(int index)
    {
        index = index > 2 ? index - 3 : index + 3;
        return OutputVec == index;
    }
    public byte InputVec { get; private set; } = 0;
    public byte OutputVec { get; private set; } = 3;
    public int CurItemid { get; private set; } = -1;

    public System.Action InfoUpData;

    public void Init(HexaElementDataSO data, HexaSaveData saveData)
    {
        Data = data;
        _manger = HexaGridManager.Instance;
        //SetReciepe(0);
        transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_TopColor", Data.TopHexaColor);
        transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_MiddleColor", Data.BottomHexaColor);
        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = Data.HexaSubIcon1;
        transform.GetChild(3).transform.localPosition = Vector3.zero;
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = Data.HexaSubIcon2;
        transform.GetChild(3).GetComponent<SpriteRenderer>().color = Color.white;

        // 세이브 데이터 적용
        if (ReferenceEquals(saveData, null)) return;
        SetVec(saveData.InVec, saveData.OutVec);
        ProductItemCount.Clear();
        if (saveData.HexaProductItemCode.Count == 0)
        {
            CurItemid = -1;
            ProductItemCount.Add(CurItemid, 0);
        }
        else
        {
            CurItemid = saveData.HexaProductItemCode[0];
            ProductItemCount.Add(CurItemid, saveData.HexaProductItemCount[0]);
        }
    }

    public void SetVec(byte input, byte output)
    {
        InputVec = input; OutputVec = output;

        transform.GetChild(2).transform.eulerAngles = Vector3.back * (InputVec * 60);
        transform.GetChild(3).transform.eulerAngles = Vector3.back * (OutputVec * 60 + 180);
    }

    public void HexaUpdate()
    {
        GetMaterialToNear();
    }

    public void GetMaterialToNear()
    {
        // 블록 내에 아이템이 있다면 리턴
        if (ProductItemCount.TryGetValue(CurItemid, out int value) && value > 0) return;

        ProductItemCount.Clear();
        CurItemid = -1;
        ProductItemCount.Add(CurItemid, 0);
        InfoUpData?.Invoke();

        IHexaGridElement near = _nearHexa[InputVec];
        if (ReferenceEquals(near, null)) return;
        if (!(near is IHexaGridInItem hexa)) return;
        if (!hexa.CanGetMaterial(InputVec)) return;

        List<int> keys = new List<int>(hexa.ProductItemCount.Keys);
        if (keys.Count == 0) return;

        foreach (int key in keys)
        {
            if (hexa.ProductItemCount[key] > 0)
            {
                hexa.ProductItemCount[key]--;
                CurItemid = key;
                ProductItemCount.Clear();
                ProductItemCount.Add(CurItemid, 1);
                _manger.ShowMoveItemEffect((near.Pos), transform.position, key);
                InfoUpData?.Invoke();
                return;
            }
        }
    }

    public void ClearPurduct() => ProductItemCount.Clear();

    public void SetNearHexa(IHexaGridElement[] hexas)
    {
        _nearHexa = hexas;
    }

    public void RemoveNearHexa(IHexaGridElement hexa)
    {
        for (int i = 0; i < _nearHexa.Length; i++)
        {
            if (_nearHexa[i] == hexa)
            {
                _nearHexa[i] = null;
                break;
            }
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            MainUIManager.Instance.HexaTransitInfoPanel.ShowPanel(this);
        }
    }
    private void OnMouseDrag()
    {
        transform.position = _manger.GetGridePos(this, Input.mousePosition, transform.position);
    }

    public HexaSaveData SaveData()
    {
        HexaSaveData saveData = new HexaSaveData();

        saveData.InVec = InputVec;
        saveData.OutVec = OutputVec;
        if (CurItemid != -1)
        {
            saveData.HexaProductItemCode.Add(CurItemid);
            saveData.HexaProductItemCount.Add(1);
        }
        return saveData;
    }
}
