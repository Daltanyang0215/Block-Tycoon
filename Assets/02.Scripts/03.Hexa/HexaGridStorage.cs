using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexaGridStorage : MonoBehaviour, IHexaGridElement
{
    [field: SerializeField] public HexaElementDataSO Data { get; private set; }
    private HexaGridManager _manger;
    private IHexaGridElement[] _nearHexa = new IHexaGridElement[6];

    private void Start()
    {
        _manger = HexaGridManager.Instance;
        //_fillMaterial = transform.Find("Gauge").GetComponent<SpriteRenderer>().materials[0];
        //_fillMaterial.SetFloat("_CutRange", 0);
        // TODO 나중에 지워야됨
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_TopColor", Data.TopHexaColor);
            transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_MiddleColor", Data.BottomHexaColor);
            //transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetColor("_TopColor", Data.TopGaugeColor);
            //transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetColor("_MiddleColor", Data.BottomGaugeColor);
            transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = Data.HexaIcon;
            transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = Data.HexaIcon;
        }
    }

    public void Init(HexaElementDataSO data)
    {
        Data = data;

        transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_TopColor", Data.TopHexaColor);
        transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_MiddleColor", Data.BottomHexaColor);
        //transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetColor("_TopColor", Data.TopGaugeColor);
        //transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetColor("_MiddleColor", Data.BottomGaugeColor);
        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = Data.HexaIcon;
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = Data.HexaIcon;
    }
    public void HexaUpdate()
    {
        int count = 0;
        foreach (IHexaGridElement near in _nearHexa)
        {
            if (ReferenceEquals(near, null)) continue;
            if (!(near is HexaGridProduct hexa)) continue;
            if (ReferenceEquals(hexa.CurRecipe, null)) continue;

            foreach (ItemPair pair in hexa.CurRecipe.ProduceItemPairs)
            {
                if (hexa.ProductItemCount[pair.ProduceItemType] <= 0) continue;

                _manger.ShowAddItemPopup(transform.position + (count * 0.35f) * Vector3.up + 0.15f * Vector3.up, pair.ProduceItemType);
                hexa.ProductItemCount[pair.ProduceItemType]--;
                MainGameManager.Instance.AddItem(pair.ProduceItemType, 1);
                count++;
            }
            hexa.InfoUpData?.Invoke(-1);
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

}
