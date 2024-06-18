using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexaGridStorage : MonoBehaviour, IHexaGridElement
{
    [field: SerializeField] public HexaElementDataSO Data { get; private set; }
    public Vector2 Pos => transform.position;
    private HexaGridManager _manger;
    private IHexaGridElement[] _nearHexa = new IHexaGridElement[6];

    public void Init(HexaElementDataSO data, HexaSaveData saveData)
    {
        Data = data;

        _manger = HexaGridManager.Instance;

        transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_TopColor", Data.TopHexaColor);
        transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_MiddleColor", Data.BottomHexaColor);
        //transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetColor("_TopColor", Data.TopGaugeColor);
        //transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetColor("_MiddleColor", Data.BottomGaugeColor);
        transform.Find("Gauge").GetComponent<SpriteRenderer>().materials[0].SetFloat("_CutRange", 0);
        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = Data.HexaIcon;
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = Data.HexaIcon;
    }
    public void HexaUpdate()
    {
        int count = 0;
        for (int i = 0; i < _nearHexa.Length; i++)
        {
            IHexaGridElement near = _nearHexa[i];
            if (ReferenceEquals(near, null)) continue;
            if (!(near is IHexaGridInItem hexa)) continue;
            //if (ReferenceEquals(hexa.CurRecipe, null)) continue;
            if (hexa.ProductItemCount == 0) continue;
            if (!hexa.CanGetMaterial(i) || hexa.ProductItemCount <= 0) continue;
            _manger.ShowAddItemPopup(transform.position + (count * 0.35f) * Vector3.up + 0.15f * Vector3.up, hexa.CurProductItem.ItemID);
            hexa.ProductItemCount--;
            //MainGameManager.Instance.AddItem(hexa.CurProductItem.ItemID, 1);
            MainGameManager.Instance.AddMoney(hexa.CurProductItem.ItemPrice);
            _manger.ShowMoveItemEffect((near.Pos), transform.position, hexa.CurProductItem.ItemID);
            count++;
            (hexa as HexaGridProduct)?.InfoUpData?.Invoke(-1);
        }
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

    public void SetNearHexa(IHexaGridElement[] hexas)
    {
        _nearHexa = hexas;
    }

    private void OnMouseDrag()
    {
        transform.position = _manger.GetGridePos(this, Input.mousePosition, transform.position);
    }

    public void HexaUpgrade() { }
}
