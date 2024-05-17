using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexaGridTransit : MonoBehaviour, IHexaGridElement, IHexaGridInItem
{
    [field: SerializeField] public HexaElementDataSO Data { get; private set; }
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

    private void Start()
    {
        _manger = HexaGridManager.Instance;
        SetReciepe(0);
        // TODO 나중에 지워야됨
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_TopColor", Data.TopHexaColor);
            transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_MiddleColor", Data.BottomHexaColor);
            transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetColor("_TopColor", Data.TopGaugeColor);
            transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetColor("_MiddleColor", Data.BottomGaugeColor);
            transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = Data.HexaSubIcon1;
            transform.GetChild(3).transform.localPosition = Vector3.zero;
            transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = Data.HexaSubIcon2;
            transform.GetChild(3).GetComponent<SpriteRenderer>().color = Color.white;
        }
    }

    public void Init(HexaElementDataSO data)
    {
        Data = data;
        SetReciepe(0);
        transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_TopColor", Data.TopHexaColor);
        transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_MiddleColor", Data.BottomHexaColor);
        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = Data.HexaSubIcon1;
        transform.GetChild(3).transform.localPosition = Vector3.zero;
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = Data.HexaSubIcon2;
        transform.GetChild(3).GetComponent<SpriteRenderer>().color = Color.white;
    }

    public void SetReciepe(int index)
    {
        //TODO 나중에 레시피에따른 기능 변경 필요
        if (Data.ProduceRecipe.Count == 0) return;
        if (CurRecipe == Data.ProduceRecipe[index]) return;

        CurRecipe = Data.ProduceRecipe[index];
        MaterialItemCount.Clear();
        ProductItemCount.Clear();
        foreach (ItemPair product in CurRecipe.ProduceItemPairs)
        {
            ProductItemCount.Add(product.ItemID, 0);
        }
    }
    public void SetVec(byte input, byte output)
    {
        InputVec = input; OutputVec = output;

        transform.GetChild(2).transform.eulerAngles = Vector3.back * (InputVec * 60);
        transform.GetChild(3).transform.eulerAngles = Vector3.back * (OutputVec * 60+180);
    }
    public void HexaUpdate()
    {
        GetMaterialToNear();
    }

    public void GetMaterialToNear()
    {
        if (ReferenceEquals(CurRecipe, null)) return;
        foreach (ItemPair pair in CurRecipe.ProduceItemPairs)
        {
            if (ProductItemCount[pair.ItemID] > 0) continue;

            IHexaGridElement near = _nearHexa[InputVec];
            if (ReferenceEquals(near, null)) continue;
            if (!(near is IHexaGridInItem hexa)) continue;

            if (hexa.CanGetMaterial(InputVec) && hexa.ProductItemCount.ContainsKey(pair.ItemID) && hexa.ProductItemCount[pair.ItemID] > 0)
            {
                hexa.ProductItemCount[pair.ItemID]--;
                ProductItemCount[pair.ItemID]++;
                break;
            }
        }
    }

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
        if (Input.GetMouseButtonDown(2))
        {

        }
        if (Input.GetMouseButtonDown(1))
        {
            MainUIManager.Instance.HexaTransitInfoPanel.ShowPanel(this);
        }
    }
    private void OnMouseDrag()
    {
        transform.position = _manger.GetGridePos(this, Input.mousePosition, transform.position);
    }


}
