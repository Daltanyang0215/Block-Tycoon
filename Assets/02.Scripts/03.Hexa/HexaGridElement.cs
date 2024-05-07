using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexaGridElement : MonoBehaviour
{

    [field: SerializeField] public HexaElementDataSO Data { get; private set; }
    private HexaGridManager _manger;
    private HexaGridElement[] _nearHexa = new HexaGridElement[6];

    private ProduceRecipe CurRecipe { get; set; }
    private Dictionary<ItemType, int> _itemCount;
    private bool _isCanProduce;
    private bool _isBefoCanProduce;
    private bool _isChangeCondition;

    private Material _fillMaterial;
    private float _filltimer = 1;
    private float _fillAmount;
    private float _fillMut;

    public void Init(HexaElementDataSO newData)
    {
        Data = newData;
        SetReciepe();
        _fillMut = CurRecipe.ProduceTime == 0 ? 0 : 1 / CurRecipe.ProduceTime;

        transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_TopColor", Data.TopHexaColor);
        transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_MiddleColor", Data.BottomHexaColor);
        transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetColor("_TopColor", Data.TopGaugeColor);
        transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetColor("_MiddleColor", Data.BottomGaugeColor);
        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = Data.HexaIcon;
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = Data.HexaIcon;
    }

    public void SetReciepe()
    {
        //TODO ���߿� �����ǿ����� ��� ���� �ʿ�
        CurRecipe = Data.ProduceRecipe;
        _itemCount = new Dictionary<ItemType, int>();
        foreach (ItemPair material in CurRecipe.MaterailItemPairs)
        {
            _itemCount.Add(material.ProduceItemType, 0);
        }
        foreach (ItemPair product in CurRecipe.ProduceItemPairs)
        {
            _itemCount.Add(product.ProduceItemType, 0);
        }
    }

    private void Start()
    {
        _manger = HexaGridManager.Instance;
        _fillMaterial = transform.Find("Gauge").GetComponent<SpriteRenderer>().materials[0];
        SetReciepe();
        // TODO ���߿� �����ߵ�
        {
            _fillMut = CurRecipe.ProduceTime == 0 ? 0 : 1 / CurRecipe.ProduceTime;

            transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_TopColor", Data.TopHexaColor);
            transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_MiddleColor", Data.BottomHexaColor);
            transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetColor("_TopColor", Data.TopGaugeColor);
            transform.GetChild(1).GetComponent<SpriteRenderer>().material.SetColor("_MiddleColor", Data.BottomGaugeColor);
            transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = Data.HexaIcon;
            transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = Data.HexaIcon;
        }
    }

    private void Update()
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
                _manger.ShowAddItemPopup(transform.position + (i * 0.35f) * Vector3.up + 0.15f * Vector3.up, item.ProduceItemType);
                MainGameManager.Instance.AddItem(item.ProduceItemType, item.ProduceAmount);
            }
        }
        _fillMaterial.SetFloat("_CutRange", _fillAmount / _filltimer);
    }

    private bool CheckCanProduce()
    {
        if (_isCanProduce) return true;

        // ���� ��� ������ ���� �ƴҶ� ���� ����� Ȯ��
        if (CurRecipe.NearHexaCondition != HexaType.None)
        {
            // ����ȭ ����, ���� ����� ��ȭ�� ���ٸ� ����
            if (!_isChangeCondition) return _isBefoCanProduce;
            
            // TODO ����ȭ �׽�Ʈ �� . ���߿� ������ ��
            Debug.Log("��ȭ ����");

            _isCanProduce = false;
            foreach (HexaGridElement near in _nearHexa)
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
        }
        else
        {
            _isCanProduce = true;
        }
        //TODO : ���߿� ���� ���� Ȯ�� �ؾ� ��
        return _isCanProduce;
    }

    private void OnMouseDown()
    {
        _fillAmount += CurRecipe.ProduceClick;
    }

    private void OnMouseDrag()
    {
        transform.position = _manger.GetGridePos(this, Input.mousePosition, transform.position, ref _nearHexa);
        // TODO ���߿� �ֺ� ��� ������ �����̺z �ϴ� ����� ���� �� �ʿ� ����. �̵����� ����ȭ �� ����
        _isChangeCondition = true;
    }


}
