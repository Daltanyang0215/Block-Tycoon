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
        //TODO 나중에 레시피에따른 기능 변경 필요
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
        // TODO 나중에 지워야됨
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

        // 주위 블록 조건이 논이 아닐때 주위 블록을 확인
        if (CurRecipe.NearHexaCondition != HexaType.None)
        {
            // 최적화 목적, 주위 블록의 변화가 없다면 리턴
            if (!_isChangeCondition) return _isBefoCanProduce;
            
            // TODO 최적화 테스트 용 . 나중에 지워야 됨
            Debug.Log("변화 감지");

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
        //TODO : 나중에 생산 조건 확인 해야 됨
        return _isCanProduce;
    }

    private void OnMouseDown()
    {
        _fillAmount += CurRecipe.ProduceClick;
    }

    private void OnMouseDrag()
    {
        transform.position = _manger.GetGridePos(this, Input.mousePosition, transform.position, ref _nearHexa);
        // TODO 나중에 주변 블록 정보를 업데이틑 하는 방법을 변경 할 필요 있음. 이동에도 최적화 할 예정
        _isChangeCondition = true;
    }


}
