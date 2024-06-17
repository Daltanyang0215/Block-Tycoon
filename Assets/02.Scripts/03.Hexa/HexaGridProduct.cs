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
    private readonly float _filltimer = 1;
    private float _fillAmount;
    private float _fillMut;
    private float _fillTimeUpgrade = 1;

    private Material _gaugeMaterial;
    private float _boosterGauge;
    private bool _isBooster;

    public float GetProducePerTime => Data.ProducePerTimeBonus * _fillTimeUpgrade;
    public float GetProduceTime => CurRecipe.ProduceTime / (Data.ProducePerTimeBonus * _fillTimeUpgrade);

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

        // ���̺� ������ ����
        #region Applay Save
        if (ReferenceEquals(saveData, null)) return;
        SetReciepe(saveData.CurRecipeIndex);
        _fillAmount = saveData.FillAmount;
        if (_fillAmount != 0) _isCanProduce = true;
        _boosterGauge = saveData.BoosterGauge;
        _isBooster = saveData.IsBooster;
        _gaugeMaterial.SetColor("_FillColor", _isBooster ? Data.BoosterMaxColor : Data.BoostingColor);

        for (int i = 0; i < saveData.HexaMaterialItemCode.Count; i++)
        {
            MaterialItemCount[saveData.HexaMaterialItemCode[i]] = saveData.HexaMaterialItemCount[i];
        }
        for (int i = 0; i < saveData.HexaProductItemCode.Count; i++)
        {
            ProductItemCount[saveData.HexaProductItemCode[i]] = saveData.HexaProductItemCount[i];
        }
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

            ProductItemCount[CurRecipe.ProduceitemID]++;
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

        if (Data.ProduceRecipe.Count != 0)
            _fillMut = CurRecipe?.ProduceTime != 0 ? GetProducePerTime / CurRecipe.ProduceTime : 0;
    }

    #endregion

    #region IHexaItem
    public void SetReciepe(int index)
    {
        if (Data.ProduceRecipe.Count == 0)
        {
            transform.GetChild(4).gameObject.SetActive(false);
            return;
        }
        if (CurRecipe == Data.ProduceRecipe[index]) return;

        CurRecipe = Data.ProduceRecipe[index];
        _fillAmount = 0;
        _fillMut = CurRecipe?.ProduceTime != 0 ? GetProducePerTime / CurRecipe.ProduceTime : 0;
        MaterialItemCount.Clear();
        ProductItemCount.Clear();
        foreach (ItemPair material in CurRecipe.MaterailItemPairs)
        {
            MaterialItemCount.Add(material.ItemID, 0);
        }

        ProductItemCount.Add(CurRecipe.ProduceitemID, 0);
        transform.GetChild(4).gameObject.SetActive(true);
        ItemData setitem = MainGameDataSo.Instance.ItemDatas[CurRecipe.ProduceitemID];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = setitem.ItemSprite;
        transform.GetChild(4).GetComponent<SpriteRenderer>().color = setitem.ItemColor;
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

        // ����ǰ�� �������� ����
        if (CheckProductCountIFMax()) return false;

        _isCanProduce = CheckNearHexaType();
        if (_isCanProduce == false) return false;

        // ��ᰡ �ʿ��� �� Ȯ��
        return CheckMaterialCount();
    }
    private bool CheckProductCountIFMax()
    {
        // ����ǰ�� ���� á�ٸ� ����
        if (ProductItemCount[CurRecipe.ProduceitemID] + 1 > MainGameDataSo.Instance.ProductStorageCountMut)
        {
            return true;
        }
        return false;
    }
    private bool CheckNearHexaType()
    {
        if (CurRecipe.NearHexaCondition == HexaType.None) return true;

        // ����ȭ ����, ���� ����� ��ȭ�� ���ٸ� ����
        if (!_isChangeCondition) return _isBefoCanProduce;

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
                // ��ᰡ ���ڸ��� ����
                if (MaterialItemCount[pair.ItemID] < pair.Amount)
                {
                    _isCanProduce = false;
                    return false;
                }
            }

            foreach (ItemPair pair in CurRecipe.MaterailItemPairs)
            {
                // ��� �Һ�
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
        if (Input.GetMouseButtonDown(0)) // Ŭ������ �ν��� ����
        {
            if (!ReferenceEquals(CurRecipe, null) && _isCanProduce && !_isBooster)
            {
                _boosterGauge += MainGameDataSo.Instance.ProductAddBoostingValue;
                if (_boosterGauge >= MainGameDataSo.Instance.ProductBoosterMaxValue)
                {
                    // �ν��� ���� �� �÷� ����� Ŭ�� �ȵǰ� ��ġ
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
        if (CurRecipe == null) return null;
        HexaSaveData saveData = new HexaSaveData();
        saveData.CurRecipeIndex = Data.ProduceRecipe.FindIndex(x => x.Equals(CurRecipe));
        saveData.FillAmount = _fillAmount;
        saveData.BoosterGauge = _boosterGauge;
        saveData.IsBooster = _isBooster;

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
