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
            transform.Find("Gauge").GetComponent<SpriteRenderer>().materials[0].SetFloat("_CutRange", 0);
            //transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetColor("_TopColor", Data.TopGaugeColor);
            //transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetColor("_MiddleColor", Data.BottomGaugeColor);
            transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = Data.HexaIcon;
            transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = Data.HexaIcon;
        }
    }

    public void Init(HexaElementDataSO data, HexaSaveData saveData)
    {
        Data = data;

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
            if (hexa.ProductItemCount.Count == 0) continue;
            List<int> keys = new List<int>(hexa.ProductItemCount.Keys);
            foreach (int key in keys)
            {
                if (!hexa.CanGetMaterial(i) || hexa.ProductItemCount[key] <= 0) continue;
                _manger.ShowAddItemPopup(transform.position + (count * 0.35f) * Vector3.up + 0.15f * Vector3.up, key);
                hexa.ProductItemCount[key]--;
                MainGameManager.Instance.AddItem(key, 1);
                count++;
            }
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

}
