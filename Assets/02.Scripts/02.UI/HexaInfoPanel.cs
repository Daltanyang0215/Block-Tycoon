using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

public class HexaInfoPanel : MonoBehaviour
{
    private HexaGridProduct _curHexaElement;

    [SerializeField] private InfoElement _infoElementPrefab;
    [SerializeField] private LocalizeStringEvent _hexaName;
    [SerializeField] private TMP_Dropdown _recipeDropdown;
    [SerializeField] private Transform _productParent;
    [SerializeField] private Transform _materialParent;

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
        Locale currentLanguage = LocalizationSettings.SelectedLocale;
        List<string> list = new List<string>();
        for (int i = 0; i < _curHexaElement.Data.ProduceRecipe.Count; i++)
        {
            ProduceRecipe recipe = _curHexaElement.Data.ProduceRecipe[i];
            list.Add(LocalizationSettings.StringDatabase.GetLocalizedString("Item", recipe.RecipeName, currentLanguage));
        }

        if (list.Count > 0)
        {
            _recipeDropdown.AddOptions(list);
            _recipeDropdown.value = _curHexaElement.Data.ProduceRecipe.FindIndex(x => x == _curHexaElement.CurRecipe);
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
            element.UpDateSlider(_curHexaElement.CheckNearHexaTypeToUI() ? 1 : 0, 1, false);
        }

        // 생산 시간 정보
        if (_curHexaElement.CurRecipe.ProduceTime != 0)
        {
            InfoElement element = FindDisableInfoElement(false);
            element.Init(MainGameDataSo.Instance.ProcessTimerImage, Color.black);
            element.UpDateSlider(0, 1, true, _curHexaElement.CurRecipe.ProduceTime.ToString() + "s");
        }
        // 생산 클릭 정보
        if (_curHexaElement.CurRecipe.ProduceClick != 0)
        {
            InfoElement element = FindDisableInfoElement(false);
            element.Init(MainGameDataSo.Instance.ProcessClickImage, Color.black);
            element.UpDateSlider(0, 1, true, (_curHexaElement.CurRecipe.ProduceClick * 100).ToString() + "%");
        }

        // 각 요소 필요한 만큼 활성화
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
        HexaGridManager.Instance.DestoryHexaGrid(_curHexaElement.gameObject);
    }
}
