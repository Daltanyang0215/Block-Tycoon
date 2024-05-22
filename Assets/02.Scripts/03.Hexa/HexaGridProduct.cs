using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexaGridProduct : MonoBehaviour, IHexaGridElement, IHexaGridInItem
{

    [field: SerializeField] public HexaElementDataSO Data { get; private set; }
    public Vector2 Pos => transform.position;
    private HexaGridManager _manger;
    private IHexaGridElement[] _nearHexa = new IHexaGridElement[6];

    public ProduceRecipe CurRecipe { get; private set; } = null;
    public Dictionary<int, int> MaterialItemCount { get; private set; } = new Dictionary<int, int>();
    public Dictionary<int, int> ProductItemCount { get; private set; } = new Dictionary<int, int>();
    public bool CanGetMaterial(int index) => true;
    private bool _isCanProduce;
    private bool _isBefoCanProduce;
    private bool _isChangeCondition;

    private Material _fillMaterial;
    private float _filltimer = 1;
    private float _fillAmount;
    private float _fillMut;

    public System.Action<int> InfoUpData;

    public void Init(HexaElementDataSO data, HexaSaveData saveData)
    {
        _manger = HexaGridManager.Instance;
        _fillMaterial = transform.Find("Gauge").GetComponent<SpriteRenderer>().materials[0];
        _fillMaterial.SetFloat("_CutRange", 0);

        Data = data;
        SetReciepe(0);

        transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_TopColor", Data.TopHexaColor);
        transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_MiddleColor", Data.BottomHexaColor);
        transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetColor("_TopColor", Data.TopGaugeColor);
        transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetColor("_MiddleColor", Data.BottomGaugeColor);
        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = Data.HexaIcon;
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = Data.HexaIcon;


        // 세이브 데이터 적용
        if (ReferenceEquals(saveData, null)) return;
        SetReciepe(saveData.CurRecipeIndex);
        _fillAmount = saveData.FillAmount;
        if (_fillAmount != 0) _isCanProduce = true;

        for (int i = 0; i < saveData.HexaMaterialItemCode.Count; i++)
        {
            MaterialItemCount[saveData.HexaMaterialItemCode[i]] = saveData.HexaMaterialItemCount[i];
        }
        for (int i = 0; i < saveData.HexaProductItemCode.Count; i++)
        {
            ProductItemCount[saveData.HexaProductItemCode[i]] = saveData.HexaProductItemCount[i];
        }
    }

    public void SetReciepe(int index)
    {
        //TODO 나중에 레시피에따른 기능 변경 필요
        if (Data.ProduceRecipe.Count == 0)
        {
            transform.GetChild(4).gameObject.SetActive(false);
            return;
        }
        if (CurRecipe == Data.ProduceRecipe[index]) return;

        CurRecipe = Data.ProduceRecipe[index];
        _fillAmount = 0;
        _fillMut = CurRecipe?.ProduceTime != 0 ? 1 / CurRecipe.ProduceTime : 0;
        MaterialItemCount.Clear();
        ProductItemCount.Clear();
        foreach (ItemPair material in CurRecipe.MaterailItemPairs)
        {
            MaterialItemCount.Add(material.ItemID, 0);
        }
        foreach (ItemPair product in CurRecipe.ProduceItemPairs)
        {
            ProductItemCount.Add(product.ItemID, 0);
            transform.GetChild(4).gameObject.SetActive(true);
            ItemData setitem = MainGameDataSo.Instance.ItemDatas[product.ItemID];
            transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = setitem.ItemSprite;
            transform.GetChild(4).GetComponent<SpriteRenderer>().color = setitem.ItemColor;
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
                //_manger.RemoveNearLine(transform.position, hexa.Pos);
                _nearHexa[i] = null;
                break;
            }
        }
        _isChangeCondition = true;
    }

    public void HexaUpdate()
    {
        if (!CheckCanProduce())
        {
            _fillAmount = 0;
            _fillMaterial.SetFloat("_CutRange", 0);
            return;
        }

        _fillAmount += Time.deltaTime * _fillMut;
        if (_fillAmount > _filltimer)
        {
            _fillAmount -= _filltimer;
            _isCanProduce = false;
            for (int i = 0; i < CurRecipe.ProduceItemPairs.Count; i++)
            {
                ItemPair item = CurRecipe.ProduceItemPairs[i];

                ProductItemCount[CurRecipe.ProduceItemPairs[i].ItemID] += CurRecipe.ProduceItemPairs[i].Amount;
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
            if (MaterialItemCount[pair.ItemID] == pair.Amount * MainGameDataSo.Instance.MatarialStorageCountMut) continue;

            for (int i = 0; i < _nearHexa.Length; i++)
            {
                IHexaGridElement near = _nearHexa[i];
                if (ReferenceEquals(near, null)) continue;
                if (!(near is IHexaGridInItem hexa)) continue;

                if (hexa.CanGetMaterial(i) && hexa.ProductItemCount.ContainsKey(pair.ItemID) && hexa.ProductItemCount[pair.ItemID] > 0)
                {
                    hexa.ProductItemCount[pair.ItemID]--;
                    MaterialItemCount[pair.ItemID]++;
                    _manger.ShowMoveItemEffect((near.Pos), transform.position, pair.ItemID);
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

        // 생산품이 가득차면 리턴
        if (CheckProductCountIFMax()) return false;

        _isCanProduce = CheckNearHexaType();
        if (_isCanProduce == false) return false;

        // 재료가 필요한 지 확인
        return CheckMaterialCount();
    }

    private bool CheckProductCountIFMax()
    {
        if (CurRecipe.ProduceItemPairs.Count > 0)
        {
            foreach (ItemPair pair in CurRecipe.ProduceItemPairs)
            {
                // 생산품이 가득 찼다면 리턴
                if (ProductItemCount[pair.ItemID] + pair.Amount > pair.Amount * MainGameDataSo.Instance.ProductStorageCountMut)
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

        // 최적화 목적, 주위 블록의 변화가 없다면 리턴
        if (!_isChangeCondition) return _isBefoCanProduce;

        // TODO 최적화 테스트 용 . 나중에 지워야 됨
        //Debug.Log("변화 감지");

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
    public bool CheckNearHexaTypeToUI()
    {
        if (CurRecipe.NearHexaCondition == HexaType.None) return true;

        // TODO 최적화 테스트 용 . 나중에 지워야 됨
        //Debug.Log("변화 감지");

        foreach (IHexaGridElement near in _nearHexa)
        {
            if (ReferenceEquals(near, null)) continue;
            if (near.Data.HexaType == CurRecipe.NearHexaCondition)
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckMaterialCount()
    {
        if (CurRecipe.MaterailItemPairs.Count > 0)
        {
            foreach (ItemPair pair in CurRecipe.MaterailItemPairs)
            {
                // 재료가 모자르면 리턴
                if (MaterialItemCount[pair.ItemID] < pair.Amount)
                {
                    _isCanProduce = false;
                    return false;
                }
            }

            foreach (ItemPair pair in CurRecipe.MaterailItemPairs)
            {
                // 재료 소비
                MaterialItemCount[pair.ItemID] -= pair.Amount;
            }
            InfoUpData?.Invoke(-1);
        }
        _isCanProduce = true;
        return _isCanProduce;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!ReferenceEquals(CurRecipe, null) && _isCanProduce)
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
    public HexaSaveData SaveData()
    {
        if (CurRecipe == null) return null;
        HexaSaveData saveData = new HexaSaveData();
        saveData.CurRecipeIndex = Data.ProduceRecipe.FindIndex(x => x.Equals(CurRecipe));
        saveData.FillAmount = _fillAmount;

        foreach (KeyValuePair<int, int> pair in MaterialItemCount)
        {
            saveData.HexaMaterialItemCode.Add(pair.Key);
            saveData.HexaMaterialItemCount.Add(pair.Value);
        }
        foreach (KeyValuePair<int, int> pair in ProductItemCount)
        {
            saveData.HexaProductItemCode.Add(pair.Key);
            saveData.HexaProductItemCount.Add(pair.Value);
        }

        return saveData;
    }

}
