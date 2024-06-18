using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexaGridProduct : MonoBehaviour, IHexaGridElement, IHexaGridInItem
{

    [field: SerializeField] public HexaElementDataSO Data { get; private set; }
    public Vector2 Pos => transform.position;
    private HexaGridManager _manger;
    private IHexaGridElement[] _nearHexa = new IHexaGridElement[6];

    public ItemData CurProductItem { get; private set; } = null;
    public Dictionary<int, int> MaterialItemCount { get; private set; } = new Dictionary<int, int>();
    public int ProductItemCount { get; set; }
    public bool CanGetMaterial(int index) => true;
    private bool _isCanProduce;
    private bool _isBefoCanProduce;
    private bool _isChangeCondition;

    private Material _fillMaterial;
    private readonly float _filltimer = 1;
    private float _fillAmount;
    private float _fillMut;
    private float _fillTimeUpgrade = 1;

    private Material _gaugeMaterial;
    private float _boosterGauge;
    private bool _isBooster;

    public float GetProducePerTime => Data.ProducePerTimeBonus * _fillTimeUpgrade;
    public float GetProduceTime => CurProductItem.ProduceRecipe.ProduceTime / (Data.ProducePerTimeBonus * _fillTimeUpgrade);

    public System.Action<int> InfoUpData;

    private ParticleSystem _particle;

    #region IHexaGrid
    public void Init(HexaElementDataSO data, HexaSaveData saveData)
    {
        _manger = HexaGridManager.Instance;
        _fillMaterial = transform.Find("Gauge").GetComponent<SpriteRenderer>().materials[0];
        _fillMaterial.SetFloat("_CutRange", 0);
        _gaugeMaterial = transform.Find("GridIcon").GetComponent<SpriteRenderer>().materials[0];
        _gaugeMaterial.SetFloat("_FillRange", 0);
        Data = data;
        SetReciepe(0);
        HexaUpgrade();
        transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_TopColor", Data.TopHexaColor);
        transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_MiddleColor", Data.BottomHexaColor);
        transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetColor("_TopColor", Data.TopGaugeColor);
        transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetColor("_MiddleColor", Data.BottomGaugeColor);

        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = Data.HexaIcon;
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = Data.HexaIcon;
        if (data.ProcessParticle != null)
        {
            _particle = Instantiate(data.ProcessParticle, transform.position, Quaternion.identity, transform);
        }

        // 세이브 데이터 적용
        #region Applay Save
        if (ReferenceEquals(saveData, null)) return;
        SetReciepe(saveData.CurProductItemID);
        _fillAmount = saveData.FillAmount;
        if (_fillAmount != 0) _isCanProduce = true;
        _boosterGauge = saveData.BoosterGauge;
        _isBooster = saveData.IsBooster;
        _gaugeMaterial.SetColor("_FillColor", _isBooster ? Data.BoosterMaxColor : Data.BoostingColor);

        for (int i = 0; i < saveData.HexaMaterialItemCode.Count; i++)
        {
            MaterialItemCount[saveData.HexaMaterialItemCode[i]] = saveData.HexaMaterialItemCount[i];
        }
        ProductItemCount = saveData.HexaProductItemCount;
        #endregion
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

        _fillAmount += Time.deltaTime * _fillMut *
            (_isBooster ? MainGameDataSo.Instance.ProductBoosterMaxMut : (_boosterGauge > 0 ? MainGameDataSo.Instance.ProductBoostingMut : 1));

        if (_fillAmount > _filltimer)
        {
            _fillAmount -= _filltimer;
            _isCanProduce = false;

            ProductItemCount++;
            _particle?.Play();
            InfoUpData?.Invoke(-1);
        }
        _fillMaterial.SetFloat("_CutRange", _fillAmount / _filltimer);

        _boosterGauge -= Time.deltaTime;
        if (_boosterGauge < 0)
        {
            _boosterGauge = 0;
            _gaugeMaterial.SetColor("_FillColor", Data.BoostingColor);
            _isBooster = false;
        }
        _gaugeMaterial.SetFloat("_FillRange", _boosterGauge / MainGameDataSo.Instance.ProductBoosterMaxValue);
    }

    public void HexaUpgrade()
    {
        _fillTimeUpgrade = 1;

        List<int> upgradDatas = MainGameManager.Instance.Upgrades[Data.GetID];

        for (int i = 0; i < Data.UpgradePairs.Count; i++)
        {
            if (upgradDatas[i] == 0) continue;

            switch (Data.UpgradePairs[i].Type)
            {
                case HexaUpgradeType.AddPerSec:
                    _fillTimeUpgrade *= 1 + (Data.UpgradePairs[i].Prices[upgradDatas[i] - 1].Value / 100);
                    break;
                default:
                    break;
            }
        }

        _fillMut = ReferenceEquals(CurProductItem, null) ? 0 : GetProducePerTime / CurProductItem.ProduceRecipe.ProduceTime;
    }

    #endregion

    #region IHexaItem
    public void SetReciepe(int itemid = 0)
    {
        if (itemid == 0)
        {
            transform.GetChild(4).gameObject.SetActive(false);
            return;
        }
        if (CurProductItem.ItemID == itemid) return;

        _fillAmount = 0;
        _fillMut = ReferenceEquals(CurProductItem, null) ? 0 : GetProducePerTime / CurProductItem.ProduceRecipe.ProduceTime;
        MaterialItemCount.Clear();
        ProductItemCount = 0;
        foreach (ItemPair material in CurProductItem.ProduceRecipe.MaterailItemPairs)
        {
            MaterialItemCount.Add(material.ItemID, 0);
        }

        transform.GetChild(4).gameObject.SetActive(true);
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = CurProductItem.ItemSprite;
        transform.GetChild(4).GetComponent<SpriteRenderer>().color = CurProductItem.ItemColor;
    }
    public void GetMaterialToNear()
    {
        if (ReferenceEquals(CurProductItem, null)) return;
        foreach (ItemPair pair in CurProductItem.ProduceRecipe.MaterailItemPairs)
        {
            if (MaterialItemCount[pair.ItemID] == pair.Amount * MainGameDataSo.Instance.MatarialStorageCountMut) continue;

            for (int i = 0; i < _nearHexa.Length; i++)
            {
                IHexaGridElement near = _nearHexa[i];
                if (ReferenceEquals(near, null)) continue;
                if (!(near is IHexaGridInItem hexa)) continue;

                if (hexa.CanGetMaterial(i) && hexa.CurProductItem.ItemID == pair.ItemID && hexa.ProductItemCount > 0)
                {
                    hexa.ProductItemCount--;
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
        if (ReferenceEquals(CurProductItem, null)) return false;
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
        // 생산품이 가득 찼다면 리턴
        if (ProductItemCount >= MainGameDataSo.Instance.ProductStorageCountMut)
        {
            return true;
        }
        return false;
    }
    private bool CheckNearHexaType()
    {
        if (CurProductItem.ProduceRecipe.NearHexaCondition == HexaType.None) return true;

        // 최적화 목적, 주위 블록의 변화가 없다면 리턴
        if (!_isChangeCondition) return _isBefoCanProduce;

        foreach (IHexaGridElement near in _nearHexa)
        {
            if (ReferenceEquals(near, null)) continue;
            if (near.Data.HexaType == CurProductItem.ProduceRecipe.NearHexaCondition)
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
        if (CurProductItem.ProduceRecipe.NearHexaCondition == HexaType.None) return true;

        foreach (IHexaGridElement near in _nearHexa)
        {
            if (ReferenceEquals(near, null)) continue;
            if (near.Data.HexaType == CurProductItem.ProduceRecipe.NearHexaCondition)
            {
                return true;
            }
        }
        return false;
    }
    private bool CheckMaterialCount()
    {
        if (CurProductItem.ProduceRecipe.MaterailItemPairs.Count > 0)
        {
            foreach (ItemPair pair in CurProductItem.ProduceRecipe.MaterailItemPairs)
            {
                // 재료가 모자르면 리턴
                if (MaterialItemCount[pair.ItemID] < pair.Amount)
                {
                    _isCanProduce = false;
                    return false;
                }
            }

            foreach (ItemPair pair in CurProductItem.ProduceRecipe.MaterailItemPairs)
            {
                // 재료 소비
                MaterialItemCount[pair.ItemID] -= pair.Amount;
            }
            InfoUpData?.Invoke(-1);
        }
        _isCanProduce = true;
        return _isCanProduce;
    }
    #endregion

    private void OnMouseOver()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    if (!ReferenceEquals(CurRecipe, null) && _isCanProduce)
        //    {
        //        _fillAmount += CurRecipe.ProduceClick;
        //    }
        //}
        if (Input.GetMouseButtonDown(0)) // 클릭으로 부스팅 충전
        {
            if (!ReferenceEquals(CurProductItem, null) && _isCanProduce && !_isBooster)
            {
                _boosterGauge += MainGameDataSo.Instance.ProductAddBoostingValue;
                if (_boosterGauge >= MainGameDataSo.Instance.ProductBoosterMaxValue)
                {
                    // 부스팅 완충 시 컬러 변경및 클릭 안되게 조치
                    _boosterGauge = MainGameDataSo.Instance.ProductBoosterMaxValue;
                    _gaugeMaterial.SetColor("_FillColor", Data.BoosterMaxColor);
                    _isBooster = true;
                }
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
        if (CurProductItem == null) return null;
        HexaSaveData saveData = new HexaSaveData();
        saveData.CurProductItemID = CurProductItem.ItemID;
        saveData.FillAmount = _fillAmount;
        saveData.BoosterGauge = _boosterGauge;
        saveData.IsBooster = _isBooster;

        foreach (KeyValuePair<int, int> pair in MaterialItemCount)
        {
            saveData.HexaMaterialItemCode.Add(pair.Key);
            saveData.HexaMaterialItemCount.Add(pair.Value);
        }
        saveData.HexaProductItemCode = CurProductItem.ItemID;
        saveData.HexaProductItemCount = ProductItemCount;

        return saveData;
    }


}
