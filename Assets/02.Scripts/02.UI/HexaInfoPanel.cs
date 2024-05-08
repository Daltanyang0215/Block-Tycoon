using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HexaInfoPanel : MonoBehaviour
{
    private HexaGridElement _curHexaElement;

    [SerializeField] private InfoElement _infoElementPrefab;
    [SerializeField] private TMP_Text _hexaName;
    [SerializeField] private TMP_Dropdown _recipeDropdown;
    [SerializeField] private Transform _productParent;
    [SerializeField] private Transform _materialParent;

    public void ShowPanel(HexaGridElement curElenemt)
    {

        transform.GetComponentInParent<Canvas>().enabled = true;

        transform.position = Camera.main.WorldToScreenPoint(curElenemt.transform.position);

        _curHexaElement = curElenemt;
        _curHexaElement.InfoUpData = UpdateRecipe;

        _hexaName.text = _curHexaElement.Data.name;

        _recipeDropdown.ClearOptions();

        List<string> list = new List<string>();
        foreach (ProduceRecipe recipe in _curHexaElement.Data.ProduceRecipe)
        {
            list.Add(recipe.RecipeName);
        }

        if (list.Count > 0)
        {
            _recipeDropdown.AddOptions(list);   
            UpdateRecipe(0);
        }
    }

    public void HidePanel()
    {
        transform.GetComponentInParent<Canvas>().enabled = false;
        _curHexaElement.InfoUpData = null;
    }

    public void UpdateRecipe(int index = -1)
    {
        if (index > 0)
        {
            _curHexaElement.SetReciepe(index);
        }
        // 모든 요소 비활성화
        foreach (InfoElement infoElement in transform.GetComponentsInChildren<InfoElement>())
        {
            infoElement.gameObject.SetActive(false);
        }

        // 각 요서 필요한 만큼 활성화
        foreach (ItemPair item in _curHexaElement.CurRecipe.MaterailItemPairs)
        {
            InfoElement element = FindDisableInfoElement(false);
            element.Init(item.ProduceItemType);
            element.UpDateSlider(_curHexaElement.ItemCount[item.ProduceItemType], item.ProduceAmount * MainGameDataSo.Instance.MatarialStorageCountMut);
        }
        foreach (ItemPair item in _curHexaElement.CurRecipe.ProduceItemPairs)
        {
            InfoElement element = FindDisableInfoElement(true);
            element.Init(item.ProduceItemType);
            element.UpDateSlider(_curHexaElement.ItemCount[item.ProduceItemType], item.ProduceAmount * MainGameDataSo.Instance.ProductStorageCountMut);
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

}
