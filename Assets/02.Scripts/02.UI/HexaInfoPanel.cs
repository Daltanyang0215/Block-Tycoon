using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class HexaInfoPanel : MonoBehaviour
{
    private HexaGridProduct _curHexaElement;

    [SerializeField] private InfoElement _infoElementPrefab;
    [SerializeField] private LocalizeStringEvent _hexaName;
    [SerializeField] private TMP_Dropdown _recipeDropdown;
    [SerializeField] private Transform _productParent;
    [SerializeField] private Transform _materialParent;

    [SerializeField] private Transform _destoryButton;
    private List<int> _curHexaProductItemIDs = new List<int>();

    public void ShowPanel(HexaGridProduct curElenemt)
    {

        transform.GetComponentInParent<Canvas>().enabled = true;

        Vector2 pos = Camera.main.WorldToScreenPoint(curElenemt.transform.position);
        Vector2 viewPos = Camera.main.WorldToViewportPoint(curElenemt.transform.position);
        if (viewPos.x > .75f)
        {
            pos.x -= 550 * (Screen.width / 1920f);
        }
        if (viewPos.y > .85f)
        {
            pos.y -= 125 * (Screen.height / 1080f);
        }
        if (viewPos.y < .25f)
        {
            pos.y += 200 * (Screen.height / 1080f);
        }
        transform.position = pos;

        _curHexaElement = curElenemt;
        _curHexaElement.InfoUpData = UpdateRecipe;

        _hexaName.StringReference.SetReference("Hexa", _curHexaElement.Data.name);
        _recipeDropdown.ClearOptions();

        foreach (InfoElement infoElement in transform.GetComponentsInChildren<InfoElement>())
        {
            infoElement.gameObject.SetActive(false);
        }
        _curHexaProductItemIDs.Clear();
        _curHexaProductItemIDs.Add(0);
        List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
        list.Add(new TMP_Dropdown.OptionData(MainUIManager.Instance.GetLocalString("Item", "NoSetting"),
                                                 MainGameDataSo.Instance.ItemDatas[0].ItemSprite));
        foreach (ItemData item in MainGameDataSo.Instance.GetCanProductItemList(curElenemt.Data))
        {
            list.Add(new TMP_Dropdown.OptionData(MainUIManager.Instance.GetLocalString("Item", item.ItemName),
                                                 item.ItemSprite));
            _curHexaProductItemIDs.Add(item.ItemID);
        }

        if (list.Count > 0)
        {
            _recipeDropdown.AddOptions(list);
            _recipeDropdown.value = curElenemt.CurProductItem == null ?
                0 :
                _curHexaProductItemIDs.FindIndex(x => x == curElenemt.CurProductItem.ItemID);
            UpdateRecipe();
        }

        //_destoryButton.gameObject.SetActive(_curHexaElement.Data.HexaType == HexaType.Produce);
        _destoryButton.gameObject.SetActive(true);

    }


    public void HidePanel()
    {
        transform.GetComponentInParent<Canvas>().enabled = false;
        _curHexaElement.InfoUpData = null;
    }

    public void UpdateRecipe(int index = -1)
    {
        if (index >= 0)
        {
            _curHexaElement.SetReciepe(_curHexaProductItemIDs[index]);
        }
        // 모든 요소 비활성화
        foreach (InfoElement infoElement in transform.GetComponentsInChildren<InfoElement>())
        {
            infoElement.gameObject.SetActive(false);
        }

        if (ReferenceEquals(_curHexaElement.CurProductItem, null) || _curHexaElement.CurProductItem.ItemID == 0) return;

        // 주위 필요한 블록에 대한 정보 표시
        if (_curHexaElement.CurProductItem.ProduceRecipe.NearHexaCondition != HexaType.None)
        {
            InfoElement element = FindDisableInfoElement(false);
            HexaElementDataSO data = MainGameDataSo.Instance.HexaDatas.Find(x => x.HexaType == _curHexaElement.CurProductItem.ProduceRecipe.NearHexaCondition);
            element.Init(data.HexaIcon, data.BottomHexaColor);
            element.UpDateSlider(_curHexaElement.CheckNearHexaTypeToUI() ? 1 : 0, 1, false);
        }

        // 생산 시간 정보 TODO 나중에 기존거 지워야됨
        //if (_curHexaElement.CurRecipe.ProduceTime != 0)
        //{
        //    InfoElement element = FindDisableInfoElement(false);
        //    element.Init(MainGameDataSo.Instance.ProcessTimerImage, Color.black);
        //    element.UpDateSlider(0, 1, true, _curHexaElement.GetProduceTime.ToString("#.#") + "s");
        //}
        if (_curHexaElement.CurProductItem.ProduceRecipe.ProduceTime != 0)
        {
            InfoElement element = FindDisableInfoElement(false);
            element.Init(MainGameDataSo.Instance.ProcessTimerImage, Color.black);
            element.UpDateSlider(0, 1, true, _curHexaElement.GetProduceTime.ToString("#.#") + "s");
        }



        //// 생산 클릭 정보 TODO 클릭으로 인한 생산을 제거 할 예정으로 지워야 됨
        //if (_curHexaElement.CurRecipe.ProduceClick != 0)
        //{
        //    InfoElement element = FindDisableInfoElement(false);
        //    element.Init(MainGameDataSo.Instance.ProcessClickImage, Color.black);
        //    element.UpDateSlider(0, 1, true, (_curHexaElement.CurRecipe.ProduceClick * 100).ToString() + "%");
        //} 

        // 각 요소 필요한 만큼 활성화
        foreach (ItemPair item in _curHexaElement.CurProductItem.ProduceRecipe.MaterailItemPairs)
        {
            InfoElement element = FindDisableInfoElement(false);
            element.Init(item.ItemID);
            element.UpDateSlider(_curHexaElement.MaterialItemCount[item.ItemID], item.Amount * MainGameDataSo.Instance.MatarialStorageCountMut);
        }
        InfoElement productElement = FindDisableInfoElement(true);
        productElement.Init(_curHexaElement.CurProductItem.ItemID);
        productElement.UpDateSlider(_curHexaElement.ProductItemCount, MainGameDataSo.Instance.ProductStorageCountMut);
    }

    private InfoElement FindDisableInfoElement(bool isProdunct)
    {
        Transform parent = isProdunct ? _productParent : _materialParent;

        for (int i = 0; i < parent.childCount; i++)
        {
            if (!parent.GetChild(i).gameObject.activeSelf)
            {
                parent.GetChild(i).gameObject.SetActive(true);
                return parent.GetChild(i).GetComponent<InfoElement>();
            }
        }
        return Instantiate(_infoElementPrefab, parent);
    }

    public void DestroySelectHexa()
    {
        HidePanel();
        HexaGridManager.Instance.DestoryHexaGrid(_curHexaElement.gameObject);
    }
}
