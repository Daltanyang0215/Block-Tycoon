using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexaGridProduct : MonoBehaviour ,IHexaGridElement
{

    [field: SerializeField] public HexaElementDataSO Data { get; private set; }
    private HexaGridManager _manger;
    private IHexaGridElement[] _nearHexa = new IHexaGridElement[6];

    public ProduceRecipe CurRecipe { get; private set; } = null;
    public Dictionary<ItemType, int> MaterialItemCount { get; private set; }
    public Dictionary<ItemType, int> ProductItemCount { get; private set; }
    private bool _isCanProduce;
    private bool _isBefoCanProduce;
    private bool _isChangeCondition;

    private Material _fillMaterial;
    private float _filltimer = 1;
    private float _fillAmount;
    private float _fillMut;

    public System.Action<int> InfoUpData;

    public void Init(HexaElementDataSO data)
    {
        Data = data;
        SetReciepe(0);

        transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_TopColor", Data.TopHexaColor);
        transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_MiddleColor", Data.BottomHexaColor);
        transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetColor("_TopColor", Data.TopGaugeColor);
        transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetColor("_MiddleColor", Data.BottomGaugeColor);
        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = Data.HexaIcon;
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = Data.HexaIcon;
    }

    public void SetReciepe(int index)
    {
        //TODO ���߿� �����ǿ����� ��� ���� �ʿ�
        if (Data.ProduceRecipe.Count == 0) return;
        CurRecipe = Data.ProduceRecipe[index];
        _fillMut = CurRecipe?.ProduceTime != 0 ? 1 / CurRecipe.ProduceTime : 0;
        MaterialItemCount = new Dictionary<ItemType, int>();
        ProductItemCount = new Dictionary<ItemType, int>();
        foreach (ItemPair material in CurRecipe.MaterailItemPairs)
        {
            MaterialItemCount.Add(material.ProduceItemType, 0);
        }
        foreach (ItemPair product in CurRecipe.ProduceItemPairs)
        {
            ProductItemCount.Add(product.ProduceItemType, 0);
        }
    }

    public void SetNearHexa(IHexaGridElement[] hexas)
    {
        _nearHexa = hexas;
        _isChangeCondition = true;
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
        _isChangeCondition = true;
    }

    private void Start()
    {
        _manger = HexaGridManager.Instance;
        _fillMaterial = transform.Find("Gauge").GetComponent<SpriteRenderer>().materials[0];
        _fillMaterial.SetFloat("_CutRange", 0);
        SetReciepe(0);
        // TODO ���߿� �����ߵ�
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_TopColor", Data.TopHexaColor);
            transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_MiddleColor", Data.BottomHexaColor);
            transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetColor("_TopColor", Data.TopGaugeColor);
            transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetColor("_MiddleColor", Data.BottomGaugeColor);
            transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = Data.HexaIcon;
            transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = Data.HexaIcon;
        }
    }

    public void HexaUpdate()
    {
        if (!CheckCanProduce()) return;

        _fillAmount += Time.deltaTime * _fillMut;
        if (_fillAmount > _filltimer)
        {
            _fillAmount -= _filltimer;
            _isCanProduce = false;
            for (int i = 0; i < CurRecipe.ProduceItemPairs.Count; i++)
            {
                ItemPair item = CurRecipe.ProduceItemPairs[i];
                //_manger.ShowAddItemPopup(transform.position + (i * 0.35f) * Vector3.up + 0.15f * Vector3.up, item.ProduceItemType);
                //MainGameManager.Instance.AddItem(item.ProduceItemType, item.ProduceAmount);

                ProductItemCount[CurRecipe.ProduceItemPairs[i].ProduceItemType] += CurRecipe.ProduceItemPairs[i].ProduceAmount;
                InfoUpData?.Invoke(-1);
            }
        }
        _fillMaterial.SetFloat("_CutRange", _fillAmount / _filltimer);

    }

    public void GetMaterialToNear()
    {
        if (ReferenceEquals(CurRecipe, null)) return;
        foreach (ItemPair pair in CurRecipe.MaterailItemPairs)
        {
            if (MaterialItemCount[pair.ProduceItemType] == pair.ProduceAmount * MainGameDataSo.Instance.MatarialStorageCountMut) continue;

            foreach (IHexaGridElement near in _nearHexa)
            {
                if (ReferenceEquals(near, null)) continue;
                if(!(near is HexaGridProduct hexa)) continue;

                if (hexa.ProductItemCount.ContainsKey(pair.ProduceItemType) && hexa.ProductItemCount[pair.ProduceItemType] > 0)
                {
                    hexa.ProductItemCount[pair.ProduceItemType]--;
                    MaterialItemCount[pair.ProduceItemType]++;
                    InfoUpData?.Invoke(-1);
                    break;
                }
            }
        }
    }

    private bool CheckCanProduce()
    {
        if (ReferenceEquals(CurRecipe, null)) return false;
        if (_isCanProduce) return true;

        // ����ǰ�� �������� ����
        if (CheckProductCountIFMax()) return false;

        _isCanProduce = CheckNearHexaType();
        if (_isCanProduce == false) return false;

        // ��ᰡ �ʿ��� �� Ȯ��
        return CheckMaterialCount();
    }

    private bool CheckProductCountIFMax()
    {
        if (CurRecipe.ProduceItemPairs.Count > 0)
        {
            foreach (ItemPair pair in CurRecipe.ProduceItemPairs)
            {
                // ����ǰ�� ���� á�ٸ� ����
                if (ProductItemCount[pair.ProduceItemType] + pair.ProduceAmount > pair.ProduceAmount * MainGameDataSo.Instance.ProductStorageCountMut)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool CheckNearHexaType()
    {
        if (CurRecipe.NearHexaCondition == HexaType.None) return true;

        // ����ȭ ����, ���� ������ ��ȭ�� ���ٸ� ����
        if (!_isChangeCondition) return _isBefoCanProduce;

        // TODO ����ȭ �׽�Ʈ �� . ���߿� ������ ��
        //Debug.Log("��ȭ ����");

        foreach (IHexaGridElement near in _nearHexa)
        {
            if (ReferenceEquals(near, null)) continue;
            if (near.Data.HexaType == CurRecipe.NearHexaCondition)
            {
                _isCanProduce = true;
                break;
            }
        }
        _isChangeCondition = false;
        _isBefoCanProduce = _isCanProduce;

        return _isCanProduce;
    }

    private bool CheckMaterialCount()
    {
        if (CurRecipe.MaterailItemPairs.Count > 0)
        {
            foreach (ItemPair pair in CurRecipe.MaterailItemPairs)
            {
                // ��ᰡ ���ڸ��� ����
                if (MaterialItemCount[pair.ProduceItemType] < pair.ProduceAmount)
                {
                    _isCanProduce = false;
                    return false;
                }
            }

            foreach (ItemPair pair in CurRecipe.MaterailItemPairs)
            {
                // ��� �Һ�
                MaterialItemCount[pair.ProduceItemType] -= pair.ProduceAmount;
            }
        }
        _isCanProduce = true;
        return _isCanProduce;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!ReferenceEquals(CurRecipe, null))
            {
                _fillAmount += CurRecipe.ProduceClick;
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            MainUIManager.Instance.HexaInfoPanel.ShowPanel(this);
        }
    }

    private void OnMouseDrag()
    {
        transform.position = _manger.GetGridePos(this, Input.mousePosition, transform.position);
    }


}