using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

public class HexaInfoPanel : MonoBehaviour
{
    private HexaGridProduct _curHexaElement;

    [SerializeField] private InfoElement _infoElementPrefab;
    [SerializeField] private TMP_Text _hexaName;
    [SerializeField] private TMP_Dropdown _recipeDropdown;
    [SerializeField] private Transform _productParent;
    [SerializeField] private Transform _materialParent;

    public void ShowPanel(HexaGridProduct curElenemt)
    {

        transform.GetComponentInParent<Canvas>().enabled = true;

        transform.position = Camera.main.WorldToScreenPoint(curElenemt.transform.position);

        _curHexaElement = curElenemt;
        _curHexaElement.InfoUpData = UpdateRecipe;

        _hexaName.text = _curHexaElement.Data.name;

        _recipeDropdown.ClearOptions();

        foreach (InfoElement infoElement in transform.GetComponentsInChildren<InfoElement>())
        {
            infoElement.gameObject.SetActive(false);
        }

        List<string> list = new List<string>();
        for (int i = 0; i < _curHexaElement.Data.ProduceRecipe.Count; i++)
        {
            ProduceRecipe recipe = _curHexaElement.Data.ProduceRecipe[i];
            list.Add(recipe.RecipeName);


        }

        if (list.Count > 0)
        {
            _recipeDropdown.AddOptions(list);
            _recipeDropdown.value = list.FindIndex(x => x == _curHexaElement.CurRecipe.RecipeName);
            UpdateRecipe();
        }
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
            _curHexaElement.SetReciepe(index);
        }
        // 모든 요소 비활성화
        foreach (InfoElement infoElement in transform.GetComponentsInChildren<InfoElement>())
        {
            infoElement.gameObject.SetActive(false);
        }

        // 주위 필요한 블록에 대한 정보 표시
        if (_curHexaElement.CurRecipe.NearHexaCondition != HexaType.None)
        {
            InfoElement element = FindDisableInfoElement(false);
            HexaElementDataSO data = MainGameDataSo.Instance.HexaDatas.Find(x => x.HexaType == _curHexaElement.CurRecipe.NearHexaCondition);
            element.Init(data.HexaIcon, data.BottomHexaColor);
            element.UpDateSlider(0, 1, false);
        }

        // 각 요서 필요한 만큼 활성화
        foreach (ItemPair item in _curHexaElement.CurRecipe.MaterailItemPairs)
        {
            InfoElement element = FindDisableInfoElement(false);
            element.Init(item.ItemID);
            element.UpDateSlider(_curHexaElement.MaterialItemCount[item.ItemID], item.Amount * MainGameDataSo.Instance.MatarialStorageCountMut);
        }
        foreach (ItemPair item in _curHexaElement.CurRecipe.ProduceItemPairs)
        {
            InfoElement element = FindDisableInfoElement(true);
            element.Init(item.ItemID);
            element.UpDateSlider(_curHexaElement.ProductItemCount[item.ItemID], item.Amount * MainGameDataSo.Instance.ProductStorageCountMut);
        }
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
        HexaGridManager.Instance.DestoryHexaGrid(_curHexaElement);
    }
}
